using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignInCommand : Command<SignInResponseModel?>
{
    public string UserName { get; set; }
    public string Password { get; set; }
}