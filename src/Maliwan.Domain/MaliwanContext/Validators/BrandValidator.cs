using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class BrandValidator : EntityValidator<Brand>, IBrandValidator
{
    #region Errors codes with message

    public const string NameIsRequired = "Message from translation file.";
    public const string NameMinLength = "Message from translation file.";
    public const string NameMaxLength = "Message from translation file.";
    public const string NameAlreadyExists = "Message from translation file.";

    public const string SkuIsRequired = "Message from translation file.";
    public const string SkuMinLength = "Message from translation file.";
    public const string SkuMaxLength = "Message from translation file.";
    public const string SkuAlreadyExists = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<BrandValidator> _localizer;
    private readonly IBrandRepository _brandRepository;

    public BrandValidator(IStringLocalizer<BrandValidator> localizer, IBrandRepository brandRepository)
    {
        _localizer = localizer;
        _brandRepository = brandRepository;

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
            .WithMessage(_localizer.GetString(nameof(SkuMinLength)).ToString().Replace("#MinLenght", "1"))
            .MaximumLength(5)
            .WithErrorCode(nameof(SkuMaxLength))
            .WithMessage(_localizer.GetString(nameof(SkuMaxLength)).ToString().Replace("#MaxLenght", "5"))
            .MustAsync(UniqueSkuAsync)
            .WithErrorCode(nameof(SkuAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(SkuAlreadyExists)));
    }

    private async Task<bool> UniqueSkuAsync(Brand entity, string sku, CancellationToken arg3)
    {
        return !await _brandRepository.AnyAsync(
            e => e.Id != entity.Id && e.Sku.Trim().ToLower() == sku.Trim().ToLower());
    }

    private async Task<bool> UniqueNameAsync(Brand entity, string name, CancellationToken arg3)
    {
        return !await _brandRepository.AnyAsync(e =>
            e.Id != entity.Id && e.Name.Trim().ToLower() == name.Trim().ToLower());
    }
}