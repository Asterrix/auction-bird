using Infrastructure.Caching;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureLayer(this IServiceCollection serviceCollection)
    {
        serviceCollection.ConfigureDatabaseConnection();
        serviceCollection.ConfigureRedis();
        serviceCollection.ConfigureCachingServices();
    }

    public static void AddInfrastructureLayer(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureSerilog();
    }

    private static void ConfigureDatabaseConnection(this IServiceCollection serviceCollection)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "env"))
            .AddJsonFile("postgres.json")
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();
        });
    }

    private static void ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void ConfigureRedis(this IServiceCollection serviceCollection)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "env"))
            .AddJsonFile("redis.json")
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetSection("Redis").Value;
        });
    }

    private static void ConfigureCachingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICachingService, CachingService>();
    }
}