using Maliwan.Application.Commands.IdentityContext;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Commands;

public static class CommandsIoC
{
    public static void AddCommands(this IServiceCollection services)
    {
        services.AddIdentityContextCommands();
    }
}