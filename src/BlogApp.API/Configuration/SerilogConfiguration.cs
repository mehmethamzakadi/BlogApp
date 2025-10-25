using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace BlogApp.API.Configuration;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var seqUrl = builder.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341";
        var environment = builder.Environment.EnvironmentName;

        // PostgreSQL için tablo yapılandırması
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter() },
            { "message_template", new MessageTemplateColumnWriter() },
            { "level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new LogEventSerializedColumnWriter() },
            { "props_test", new PropertiesColumnWriter() },
            { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.Raw) }
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // Enrichers - Context bilgilerini ekle
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "BlogApp")
            .Enrich.WithProperty("Environment", environment)

            // Console sink - Development için
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}"
            )

            // File sink - Her gün yeni dosya, 31 gün saklama
            .WriteTo.File(
                path: "logs/blogapp-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                fileSizeLimitBytes: 10 * 1024 * 1024 // 10 MB
            )

            // PostgreSQL sink - Structured logging için
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "Logs",
                columnOptions: columnWriters,
                needAutoCreateTable: true,
                restrictedToMinimumLevel: LogEventLevel.Information
            )

            // Seq sink - Development/Production log analizi için
            .WriteTo.Seq(
                serverUrl: seqUrl,
                restrictedToMinimumLevel: LogEventLevel.Debug,
                apiKey: builder.Configuration["Serilog:SeqApiKey"]
            )

            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
