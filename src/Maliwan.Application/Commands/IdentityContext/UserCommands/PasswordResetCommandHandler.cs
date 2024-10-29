using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class PasswordResetCommand : Command<string>
{
    public string UserName { get; set; }

    public PasswordResetCommand()
    {
    }

    public PasswordResetCommand(string userName)
    {
        UserName = userName;
    }
}