using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class SubcategoryValidator : EntityValidator<Subcategory>, ISubcategoryValidator
{

    #region Errors codes with message

    public const string CategoryNotExists = "Message from translation file.";

    public const string NameIsRequired = "Message from translation file.";
    public const string NameMinLength = "Message from translation file.";
    public const string NameMaxLength = "Message from translation file.";
    public const string NameAlreadyExists = "Message from translation file.";

    public const string SkuIsRequired = "Message from translation file.";
    public const string SkuMinLength = "Message from translation file.";
    public const string SkuMaxLength = "Message from translation file.";
    public const string SkuAlreadyExists = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<SubcategoryValidator> _localizer;
    private readonly ISubcategoryRepository _subcategoryRepository;
    private readonly ICategoryRepository _categoryRepository;

    public SubcategoryValidator(IStringLocalizer<SubcategoryValidator> localizer,
        ISubcategoryRepository subcategoryRepository, ICategoryRepository categoryRepository)
    {
        _localizer = localizer;
        _subcategoryRepository = subcategoryRepository;
        _categoryRepository = categoryRepository;

        RuleFor(e => e.IdCategory)
            .MustAsync(CategoryExistsAsync)
            .WithErrorCode(nameof(CategoryNotExists))
            .WithMessage(_localizer.GetString(nameof(CategoryNotExists)));

        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .MinimumLength(3)
            .WithErrorCode(nameof(NameMinLength))
            .WithMessage(_localizer.GetString(nameof(NameMinLength)).ToString().Replace("#MinLenght", "3"))
            .MaximumLength(255)
            .WithErrorCode(nameof(NameMaxLength))
            .WithMessage(_localizer.GetString(nameof(NameMaxLength)).ToString().Replace("#MaxLenght", "255"))
            .MustAsync(UniqueNameAsync)
            .WithErrorCode(nameof(NameAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(NameAlreadyExists)));

        RuleFor(e => e.Sku)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(SkuIsRequired))
            .WithMessage(_localizer.GetString(nameof(SkuIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(SkuIsRequired))
            .WithMessage(_localizer.GetString(nameof(SkuIsRequired)))
            .MinimumLength(1)
            .WithErrorCode(nameof(SkuMinLength))
            .WithMessage(_localizer.GetString(nameof(SkuMinLength)).ToString().Replace("#MinLenght", "3"))
            .MaximumLength(5)
            .WithErrorCode(nameof(SkuMaxLength))
            .WithMessage(_localizer.GetString(nameof(SkuMaxLength)).ToString().Replace("#MaxLenght", "255"))
            .MustAsync(UniqueSkuAsync)
            .WithErrorCode(nameof(SkuAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(SkuAlreadyExists)));
    }

    private async Task<bool> UniqueSkuAsync(Subcategory entity, string sku, CancellationToken arg3)
    {
        return !await _subcategoryRepository.AnyAsync(e =>
            e.Id != entity.Id && e.Sku.Trim().ToLower() == sku.Trim().ToLower() && e.IdCategory == entity.IdCategory);
    }

    private async Task<bool> UniqueNameAsync(Subcategory entity, string name, CancellationToken arg3)
    {
        return !await _subcategoryRepository.AnyAsync(e =>
            e.Id != entity.Id && e.Name.Trim().ToLower() == name.Trim().ToLower() && e.IdCategory == entity.IdCategory);
    }

    private async Task<bool> CategoryExistsAsync(int idCategory, CancellationToken arg2)
    {
        return await _categoryRepository.AnyAsync(e => e.Id == idCategory);
    }
}