using Maliwan.Domain.Core.Enums;
using System.Text.Json.Serialization;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class UserAddRoleCommand : Command
{
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IEnumerable<RoleEnum> Roles { get; set; }

    public UserAddRoleCommand()
    {
    }

    public UserAddRoleCommand(Guid userId, IEnumerable<RoleEnum> roles)
    {
        UserId = userId;
        Roles = roles;
    }
}