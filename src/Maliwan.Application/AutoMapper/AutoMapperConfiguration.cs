using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.AutoMapper;

public static class AutoMapperConfiguration
{
    public static void AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddSingleton(RegisterMappings().CreateMapper());
    }

    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CommandToEntityMappingProfile>();
            cfg.AddProfile<EntityToModelMappingProfile>();
            cfg.AddProfile<ConvertObjectsProfile>();
        });
    }
}