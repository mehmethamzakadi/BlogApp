using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace BlogApp.API.Middlewares
{
    public static class MigratorMiddleware
    {
        public static IApplicationBuilder UseDbMigrator(this IApplicationBuilder app, IConfiguration configuration)
        {
            #region Database Migrate Ediliyor
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
                dataContext.Database.Migrate();
            }
            #endregion

            UseMsSqlSeriLog(configuration);
            //UsePostgreSqlSeriLog(configuration);

            return app;
        }

        private static void UseMsSqlSeriLog(IConfiguration configuration)
        {
            var mssqlConnectionString = configuration.GetConnectionString("BlogAppMsSqlConnectionString");
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .MSSqlServer(
                    connectionString: mssqlConnectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true, })
                .CreateLogger();
        }

        private static void UsePostgreSqlSeriLog(IConfiguration configuration)
        {
            var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString");
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
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    needAutoCreateTable: true,
                    useCopy: false
                    )
                .CreateLogger();
        }
    }


}
