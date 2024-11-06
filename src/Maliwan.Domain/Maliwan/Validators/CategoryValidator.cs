using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.Maliwan.Validators;

public class CategoryValidator : EntityValidator<Category>, ICategoryValidator
{
    #region Errors codes with message

    public const string NameIsRequired = "Message from translation file.";
    public const string NameMinLength = "Message from translation file.";
    public const string NameMaxLength = "Message from translation file.";
    public const string NameAlreadyExists = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<CategoryValidator> _localizer;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryValidator(IStringLocalizer<CategoryValidator> localizer, ICategoryRepository categoryRepository)
    {
        _localizer = localizer;
        _categoryRepository = categoryRepository;

        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .MaximumLength(3)
            .WithErrorCode(nameof(NameMinLength))
            .WithMessage(_localizer.GetString(nameof(NameMinLength)).ToString().Replace("#MinLenght", "3"))
            .MaximumLength(255)
            .WithErrorCode(nameof(NameMaxLength))
            .WithMessage(_localizer.GetString(nameof(NameMaxLength)).ToString().Replace("#MaxLength", "255"))
            .MustAsync(UniqueNameAsync)
            .WithErrorCode(nameof(NameAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(NameAlreadyExists)));
    }

    private async Task<bool> UniqueNameAsync(Category entity, string name, CancellationToken arg3)
    {
        return !await _categoryRepository.AnyAsync(e =>
            e.Id != entity.Id && e.Name.Trim().ToLower() == name.Trim().ToLower());
    }
}