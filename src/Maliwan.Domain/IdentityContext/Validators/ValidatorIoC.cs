using Maliwan.Domain.IdentityContext.Interfaces.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Domain.IdentityContext.Validators;

public static class ValidatorsIoC
{
    public static void AddIdentityValidators(this IServiceCollection services)
    {
        services.AddScoped<IUserValidator, UserValidator>();
        services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
    }
}