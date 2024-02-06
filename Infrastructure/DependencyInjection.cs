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
        IConfigurationRoot config;

        try
        {
            // This is for local development
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation", "env");
            config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("postgres.json")
                .AddEnvironmentVariables()
                .Build();
        }

        // If the above fails, then we are in a docker container
        catch (Exception)
        {
            // This is for docker container environment
            const string basePath = "/app/env";
            config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("postgres.json")
                .AddEnvironmentVariables()
                .Build();
        }

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
        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
        });
    }

    private static void ConfigureCachingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICachingService, CachingService>();
    }
}