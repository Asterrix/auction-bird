using Application.Caching;
using Application.Features.Bidding;
using Application.Features.Categories.Queries;
using Application.Features.Items;
using Infrastructure.Caching;
using Infrastructure.Features.Bidding;
using Infrastructure.Features.Categories;
using Infrastructure.Features.Items;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
        serviceCollection.ConfigureRepositories();
    }

    public static void AddInfrastructureLayer(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureSerilog();
    }

    private static void ConfigureDatabaseConnection(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"))
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

    private static void ConfigureRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
        serviceCollection.AddScoped<IItemRepository, ItemRepository>();
        serviceCollection.AddScoped<IBiddingRepository, BiddingRepository>();
    }
}