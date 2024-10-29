using Maliwan.Domain.Core.Enums;
using Maliwan.Infra.Data.Contexts.IdentityDb.Seeders.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Maliwan.Infra.Data.Contexts.IdentityDb.Seeders;

public class RoleSeed : IRoleSeed
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleSeed(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        foreach (var role in Enum.GetValues<RoleEnum>())
        {
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
            }
        }
    }
}