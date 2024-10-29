using Maliwan.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Services;

public static class ServicesIoC
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IFileManagerService, FileManagerService>();
    }
}
