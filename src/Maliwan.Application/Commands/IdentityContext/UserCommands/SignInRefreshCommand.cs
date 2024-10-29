using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignInRefreshCommand : Command<SignInResponseModel?>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public SignInRefreshCommand()
    {
    }

    public SignInRefreshCommand(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}