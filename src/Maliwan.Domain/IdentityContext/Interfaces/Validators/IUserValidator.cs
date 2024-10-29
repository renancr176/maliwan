using FluentValidation;
using FluentValidation.Results;
using Maliwan.Domain.IdentityContext.Entities;

namespace Maliwan.Domain.IdentityContext.Interfaces.Validators;

public interface IUserValidator : IValidator<User>
{
    ValidationResult ValidationResult { get; }
    Task<bool> IsValidAsync(User entity);
}