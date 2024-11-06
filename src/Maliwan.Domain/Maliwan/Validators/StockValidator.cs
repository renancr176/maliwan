using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.Maliwan.Validators;

public class StockValidator : EntityValidator<Stock>, IStockValidator
{
    #region Errors codes with message

    public const string ProductNotExists = "Message from translation file.";

    public const string SizeNotExists = "Message from translation file.";

    public const string ColorNotExists = "Message from translation file.";

    public const string InputQuantityMinVal = "Message from translation file.";

    public const string InputDateMinVal = "Message from translation file.";
    public const string InputDateMaxVal = "Message from translation file.";

    public const string PurchasePriceMinVal = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<StockValidator> _localizer;
    private readonly IStockRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductSizeRepository _productSizeRepository;
    private readonly IProductColorRepository _productColorRepository;

    public StockValidator(IStringLocalizer<StockValidator> localizer, IStockRepository stockRepository)
    {
        _localizer = localizer;
        _stockRepository = stockRepository;

        RuleFor(e => e.IdProduct)
            .MustAsync(ProductExistsAsync)
            .WithErrorCode(nameof(ProductNotExists))
            .WithMessage(_localizer.GetString(nameof(ProductNotExists)));

        When(e => e.IdSize.HasValue, () =>
        {
            RuleFor(e => e.IdSize)
                .MustAsync(SizeExistsAsync)
                .WithErrorCode(nameof(SizeNotExists))
                .WithMessage(_localizer.GetString(nameof(SizeNotExists)));
        });

        When(e => e.IdColor.HasValue, () =>
        {
            RuleFor(e => e.IdColor)
                .MustAsync(ColorExistsAsync)
                .WithErrorCode(nameof(ColorNotExists))
                .WithMessage(_localizer.GetString(nameof(ColorNotExists)));
        });

        RuleFor(e => e.InputQuantity)
            .GreaterThan(0)
            .WithErrorCode(nameof(InputQuantityMinVal))
            .WithMessage(_localizer.GetString(nameof(InputQuantityMinVal)).ToString().Replace("#MinVal", "0"));

        RuleFor(e => e.InputDate)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(DateTime.MinValue)
            .WithErrorCode(nameof(InputDateMinVal))
            .WithMessage(_localizer.GetString(nameof(InputDateMinVal)).ToString().Replace("#MinVal", DateTime.MinValue.FromUtcToBrTimeZone().ToString("G")))
            .LessThan(DateTime.UtcNow.AddMinutes(30))
            .WithErrorCode(nameof(InputDateMaxVal))
            .WithMessage(_localizer.GetString(nameof(InputDateMaxVal)).ToString().Replace("#MaxVal", DateTime.UtcNow.AddMinutes(30).FromUtcToBrTimeZone().ToString("G")));

        RuleFor(e => e.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(nameof(PurchasePriceMinVal))
            .WithMessage(_localizer.GetString(nameof(PurchasePriceMinVal)).ToString().Replace("#MinVal", (0M).ToCurrencyString()));
    }

    private async Task<bool> ColorExistsAsync(int? idColor, CancellationToken arg2)
    {
        return !idColor.HasValue || await _productColorRepository.AnyAsync(e => e.Id == idColor.Value);
    }

    private async Task<bool> SizeExistsAsync(int? idSize, CancellationToken arg2)
    {
        return !idSize.HasValue || await _productSizeRepository.AnyAsync(e => e.Id == idSize.Value);
    }

    private async Task<bool> ProductExistsAsync(Guid idProduct, CancellationToken arg2)
    {
        return await _productRepository.AnyAsync(e => e.Id == idProduct);
    }
}