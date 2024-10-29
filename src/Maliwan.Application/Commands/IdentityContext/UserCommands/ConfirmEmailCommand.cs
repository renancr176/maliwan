using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class ConfirmEmailCommand : Command
{
    public string UserName { get; set; }
    public string EmailConfirmationKey { get; set; }
}