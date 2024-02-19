using System.Reflection;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Application.Caching;
using Application.Features.Items.Queries.FindItem;
using Application.Features.Items.Queries.ListItems;
using Application.Filtration;
using Application.Specification;
using Domain.Items;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(DependencyInjection).Assembly;

    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.ConfigureMediatR();
        services.ConfigureFluentValidation();
        services.ConfigureAwsCognito();
        services.ConfigureHttpContextAccessor();
        services.ConfigureItems();
    }

    private static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly));
    }

    private static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly);
    }

    private static void ConfigureAwsCognito(this IServiceCollection services)
    {
        string key = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ??
                     throw new InvalidOperationException("AWS_ACCESS_KEY is not set");
        string secret = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ??
                        throw new InvalidOperationException("AWS_SECRET_KEY is not set");

        services.AddAWSService<IAmazonCognitoIdentityProvider>(
            new AWSOptions
            {
                Region = RegionEndpoint.EUWest3,
                Credentials = new BasicAWSCredentials(key, secret)
            });
    }
    
    private static void ConfigureHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }

    private static void ConfigureItems(this IServiceCollection services)
    {
        services.AddTransient<ICacheKeyBuilder<ListItemsQuery>, ListItemsCacheKeyBuilder>();
        services.AddTransient<IFilter<ListItemsQuery, ISpecification<Item>>, ListItemsFilter>();
        services.AddTransient<ITimeRemainingCalculator, TimeRemainingCalculator>();
    }
}