using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class ProductColorValidator : EntityValidator<ProductColor>, IProductColorValidator
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

    public const string BgColorIsRequired = "Message from translation file.";
    public const string BgColorIsInvalid = "Message from translation file.";

    public const string TextColorIsRequired = "Message from translation file.";
    public const string TextColorIsInvalid = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<ProductColorValidator> _localizer;
    private readonly IProductColorRepository _productColorRepository;

    public ProductColorValidator(IStringLocalizer<ProductColorValidator> localizer, IProductColorRepository productColorRepository)
    {
        _localizer = localizer;
        _productColorRepository = productColorRepository;

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
            .WithMessage(_localizer.GetString(nameof(NameMinLength)).ToString().Replace("#MinLength", "3"))
            .MaximumLength(255)
            .WithErrorCode(nameof(NameMaxLength))
            .WithMessage(_localizer.GetString(nameof(NameMaxLength)).ToString().Replace("#MaxLength", "255"))
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
            .WithMessage(_localizer.GetString(nameof(SkuMinLength)).ToString().Replace("#MinLength", "1"))
            .MaximumLength(5)
            .WithErrorCode(nameof(SkuMaxLength))
            .WithMessage(_localizer.GetString(nameof(SkuMaxLength)).ToString().Replace("#MaxLength", "5"))
            .MustAsync(UniqueSkuAsync)
            .WithErrorCode(nameof(SkuAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(SkuAlreadyExists)));

        RuleFor(e => e.BgColor)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(BgColorIsRequired))
            .WithMessage(_localizer.GetString(nameof(BgColorIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(BgColorIsRequired))
            .WithMessage(_localizer.GetString(nameof(BgColorIsRequired)))
            .Must(ValidColor)
            .WithErrorCode(nameof(BgColorIsInvalid))
            .WithMessage(_localizer.GetString(nameof(BgColorIsInvalid)));

        RuleFor(e => e.TextColor)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(TextColorIsRequired))
            .WithMessage(_localizer.GetString(nameof(TextColorIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(TextColorIsRequired))
            .WithMessage(_localizer.GetString(nameof(TextColorIsRequired)))
            .Must(ValidColor)
            .WithErrorCode(nameof(TextColorIsInvalid))
            .WithMessage(_localizer.GetString(nameof(TextColorIsInvalid)));
    }

    private bool ValidColor(string color)
    {
        return color.IsValidHexColor();
    }

    private async Task<bool> UniqueSkuAsync(ProductColor entity, string sku, CancellationToken arg3)
    {
        return !await _productColorRepository.AnyAsync(
            e => e.Id != entity.Id && e.Sku.Trim().ToLower() == sku.Trim().ToLower());
    }

    private async Task<bool> UniqueNameAsync(ProductColor entity, string name, CancellationToken arg3)
    {
        return !await _productColorRepository.AnyAsync(e =>
            e.Id != entity.Id && e.Name.Trim().ToLower() == name.Trim().ToLower());
    }
}