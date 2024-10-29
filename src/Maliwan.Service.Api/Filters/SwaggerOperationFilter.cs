using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Maliwan.Service.Api.Filters;

public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation?.Parameters == null
            || !operation.Parameters.Any())
        {
            return;
        }

        var parametersWithPropertiesToIgnore = context.ApiDescription
            .ActionDescriptor.Parameters.Where(p =>
                p.ParameterType.GetProperties()
                    .Any(t => t.GetCustomAttribute<IgnoreDataMemberAttribute>() != null));

        foreach (var parameter in parametersWithPropertiesToIgnore)
        {
            var ignoreDataMemberProperties = parameter.ParameterType.GetProperties()
                .Where(t => t.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
                .Select(p => p.Name);

            operation.Parameters = operation.Parameters.Where(p => !ignoreDataMemberProperties.Contains(p.Name))
                .ToList();
        }
    }
}