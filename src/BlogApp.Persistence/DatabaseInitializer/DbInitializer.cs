using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace BlogApp.Persistence.DatabaseInitializer;

public sealed class DbInitializer : IDbInitializer
{
    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        
        var dataContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        await dataContext.Database.MigrateAsync(cancellationToken);

        // Permission'ları seed et
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PermissionSeeder>>();
        var permissionSeeder = new PermissionSeeder(dataContext, roleManager, logger);
        await permissionSeeder.SeedPermissionsAsync();
    }

    public Task EnsurePostgreSqlSerilogTableAsync(IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;

        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString")
            ?? throw new InvalidOperationException("PostgreSQL bağlantı dizesi yapılandırılmalıdır.");

        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "log") }
        };

        using Logger logger = new LoggerConfiguration()
            .WriteTo
            .PostgreSQL(
                connectionString: postgreSqlConnectionString,
                tableName: "Logs",
                columnOptions: columnWriters,
                restrictedToMinimumLevel: LogEventLevel.Information,
                needAutoCreateTable: true,
                useCopy: false)
            .CreateLogger();

        logger.Information("Serilog PostgreSQL tablosu doğrulandı.");
        return Task.CompletedTask;
    }
}
