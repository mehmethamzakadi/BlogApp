using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.DatabaseInitializer.Seeders;
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

        var context = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

        try
        {
            // 1. Migration'ları uygula
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migrations applied successfully");

            // 2. Seederleri oluştur ve sırala
            var seeders = new List<ISeeder>
            {
                new RoleSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<RoleSeeder>>()),
                new PermissionSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<PermissionSeeder>>()),
                new RolePermissionSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<RolePermissionSeeder>>()),
                new UserSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<UserSeeder>>()),
                new UserRoleSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<UserRoleSeeder>>()),
                new CategorySeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<CategorySeeder>>()),
                new PostSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<PostSeeder>>())
            };

            // Order'a göre sırala ve çalıştır
            var orderedSeeders = seeders.OrderBy(s => s.Order).ToList();

            logger.LogInformation("Starting database seeding with {SeederCount} seeders", orderedSeeders.Count);

            foreach (var seeder in orderedSeeders)
            {
                await seeder.SeedAsync(cancellationToken);
            }

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
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
