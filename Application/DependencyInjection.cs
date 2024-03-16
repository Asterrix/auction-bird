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
using Firebase.Storage;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using File = System.IO.File;

namespace Application;

public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(DependencyInjection).Assembly;

    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.ConfigureMediatR();
        services.ConfigureFluentValidation();
        services.ConfigureAwsCognito();
        services.ConfigureStripe();
        services.ConfigureHttpContextAccessor();
        services.ConfigureFirebaseStorage();
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

    private static void ConfigureStripe(this IServiceCollection services)
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY") ??
                                     throw new InvalidOperationException("STRIPE_API_KEY is not set");

        StripeConfiguration.EnableTelemetry = false;
    }

    private static void ConfigureHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }

    private static void ConfigureFirebaseStorage(this IServiceCollection services)
    {
        string bucket = Environment.GetEnvironmentVariable("FIREBASE_STORAGE_BUCKET") ??
                        throw new InvalidOperationException("FIREBASE_STORAGE_BUCKET is not set");

        string key = File
            .ReadAllText("env/firebase.json")
            .Replace("\n", "");

        FirebaseStorage storage = new(bucket, new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(key)
        });

        services.AddSingleton(storage);
    }

    private static void ConfigureItems(this IServiceCollection services)
    {
        services.AddTransient<ICacheKeyBuilder<ListItemsQuery>, ListItemsCacheKeyBuilder>();
        services.AddTransient<IFilter<ListItemsQuery, ISpecification<Item>>, ListItemsFilter>();
        services.AddTransient<ITimeRemainingCalculator, TimeRemainingCalculator>();
    }
}