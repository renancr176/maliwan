using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.IdentityContext.Entities;

namespace Maliwan.Domain.IdentityContext.Interfaces.Validators;

public interface IRefreshTokenValidator : IEntityValidator<RefreshToken>
{
}