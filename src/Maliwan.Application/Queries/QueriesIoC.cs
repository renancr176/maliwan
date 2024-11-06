using Maliwan.Application.Queries.MaliwanContext;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Queries;

public static class QueriesIoC
{
    public static void AddQueries(this IServiceCollection services)
    {
        services.AddMaliwanContextQueries();
    }
}