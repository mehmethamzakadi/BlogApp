using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace BlogApp.Persistence.DatabaseInitializer;

public interface IDbInitializer
{
    Task CreateMsSqlSeriLogTable(IConfiguration configuration);
    Task CreatePostgreSqlSeriLogTable(IConfiguration configuration);
    Task DatabaseInitializer(IApplicationBuilder app, IConfiguration configuration);
}
