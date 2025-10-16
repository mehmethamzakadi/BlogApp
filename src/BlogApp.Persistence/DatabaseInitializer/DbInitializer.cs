
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using Serilog;
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

        using ILogger logger = new LoggerConfiguration()
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
