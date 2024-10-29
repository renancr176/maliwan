using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.IdentityContext.Entities;

namespace Maliwan.Application.Services.Interfaces;

public interface IUserService
{
    public Guid? UserId { get; }
    Task<User> CurrentUserAsync();
    Task<User> FindByUserName(string userName);
    Task<bool> CurrentUserHasRole(string roleName);
    Task<bool> CurrentUserHasRole(RoleEnum role);
    Task<bool> CurrentUserHasRoleAnyAsync(Func<RoleEnum?, bool> predicate);
    Task<bool> HasRole(Guid userId, string roleName);
    Task<bool> HasRole(Guid userId, RoleEnum role);
    Task<bool> SendEmailConfirmationAsync(Guid userId);

    #region Jwt

    Task<SignInResponseModel?> GetJwtAsync(User user);
    Task<SignInResponseModel?> RefreshTokenAsync(string encodedJwt, string refreshToken);

    #endregion
}