using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class ResetPasswordCommand : Command<string?>
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string RememberPhrase { get; set; }

    public ResetPasswordCommand()
    {
    }

    public ResetPasswordCommand(string userName, string token, string newPassword, string rememberPhrase)
    {
        UserName = userName;
        Token = token;
        NewPassword = newPassword;
        RememberPhrase = rememberPhrase;
    }
}