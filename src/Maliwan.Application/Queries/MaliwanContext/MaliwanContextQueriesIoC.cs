using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Queries.MaliwanContext;

public static class MaliwanContextQueriesIoC
{
    public static void AddMaliwanContextQueries(this IServiceCollection services)
    {
        services.AddScoped<IBrandQuery, BrandQuery>();
        services.AddScoped<ICategoryQuery, CategoryQuery>();
    }
}