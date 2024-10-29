using Maliwan.Application.Models.IdentityContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignUpCommand : Command<UserModel?>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string RememberPhrase { get; set; }
}