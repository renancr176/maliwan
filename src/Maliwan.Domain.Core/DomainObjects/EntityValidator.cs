using FluentValidation;
using FluentValidation.Results;

namespace Maliwan.Domain.Core.DomainObjects;

public abstract class EntityValidator<TEntity> : AbstractValidator<TEntity>, IEntityValidator<TEntity>
    where TEntity : Entity
{
    public ValidationResult ValidationResult { get; protected set; }
    public async Task<bool> IsValidAsync(TEntity entity)
    {
        ValidationResult = await ValidateAsync(entity);
        return ValidationResult.IsValid;
    }
}