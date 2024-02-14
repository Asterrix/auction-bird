using System.Reflection;
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

    private static void ConfigureItems(this IServiceCollection services)
    {
        services.AddTransient<ICacheKeyBuilder<ListItemsQuery>, ListItemsCacheKeyBuilder>();
        services.AddTransient<IFilter<ListItemsQuery, ISpecification<Item>>, ListItemsFilter>();
        services.AddTransient<ITimeRemainingCalculator, TimeRemainingCalculator>();
    }
}