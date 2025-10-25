using Microsoft.Extensions.Configuration;

namespace BlogApp.Persistence.DatabaseInitializer;

public interface IDbInitializer
{
    Task EnsurePostgreSqlSerilogTableAsync(IConfiguration configuration, CancellationToken cancellationToken = default);
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
