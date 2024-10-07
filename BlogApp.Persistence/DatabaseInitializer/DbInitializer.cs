
using BlogApp.Persistence.Contexts;
using Microsoft.AspNetCore.Builder;
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
    public Task DatabaseInitializer(IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        dataContext.Database.Migrate();

        return Task.CompletedTask;
    }

    public Task CreatePostgreSqlSeriLogTable(IConfiguration configuration)
    {
        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString") ?? string.Empty;
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
        };

        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .PostgreSQL(
                connectionString: postgreSqlConnectionString,
                tableName: "Logs",
                columnOptions: columnWriters,
                restrictedToMinimumLevel: LogEventLevel.Information,
                needAutoCreateTable: true,
                useCopy: false
                )
            .CreateLogger();

        return Task.CompletedTask;
    }


}
