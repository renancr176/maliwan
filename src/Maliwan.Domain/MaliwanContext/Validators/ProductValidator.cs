using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class ProductValidator : EntityValidator<Product>, IProductValidator
{
    #region Errors codes with message

    public const string BrandNotExists = "Message from translation file.";

    public const string SubcategoryNotExists = "Message from translation file.";

    public const string GenderNotExists = "Message from translation file.";

    public const string NameIsRequired = "Message from translation file.";
    public const string NameMinLength = "Message from translation file.";
    public const string NameMaxLength = "Message from translation file.";
    public const string NameAlreadyExists = "Message from translation file.";

    public const string UnitPriceMinVal = "Message from translation file.";

    public const string SkuIsRequired = "Message from translation file.";
    public const string SkuMinLength = "Message from translation file.";
    public const string SkuMaxLength = "Message from translation file.";
    public const string SkuAlreadyExists = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<ProductValidator> _localizer;
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ISubcategoryRepository _subcategoryRepository;
    private readonly IGenderRepository _genderRepository;
    private readonly IStockRepository _stockRepository;

    public ProductValidator(IStringLocalizer<ProductValidator> localizer, IProductRepository productRepository,
        IBrandRepository brandRepository, ISubcategoryRepository subcategoryRepository,
        IGenderRepository genderRepository, IStockRepository stockRepository)
    {
        _localizer = localizer;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _subcategoryRepository = subcategoryRepository;
        _genderRepository = genderRepository;
        _stockRepository = stockRepository;

        RuleFor(e => e.IdBrand)
            .MustAsync(BrandExistsAsync)
            .WithErrorCode(nameof(BrandNotExists))
            .WithMessage(_localizer.GetString(nameof(BrandNotExists)));

        RuleFor(e => e.IdSubcategory)
            .MustAsync(SubcategoryExistsAsync)
            .WithErrorCode(nameof(SubcategoryNotExists))
            .WithMessage(_localizer.GetString(nameof(SubcategoryNotExists)));

        RuleFor(e => e.IdGender)
            .MustAsync(GenderExistsAsync)
            .WithErrorCode(nameof(GenderNotExists))
            .WithMessage(_localizer.GetString(nameof(GenderNotExists)));

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

        RuleFor(e => e.UnitPrice)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(nameof(UnitPriceMinVal))
            .WithErrorCode(_localizer.GetString(nameof(UnitPriceMinVal)).ToString().Replace("#MinVal", "0"))
            .MustAsync(SellingPriceGreaterThanPurchasingPriceAsync)
            .WithErrorCode(nameof(UnitPriceMinVal))
            .WithMessage(GetUnitPriceMinValMessage);

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

    private string GetUnitPriceMinValMessage(Product entity)
    {
        var message = _localizer.GetString(nameof(UnitPriceMinVal)).ToString();
        var stockMaxPurchasePrice = 0M;
        Task.Run(async () =>
        {
            stockMaxPurchasePrice = await GetStockMaxPurchasePriceAsync(entity);
        }).Wait();
        return message.Replace("#MinVal", stockMaxPurchasePrice.ToCurrencyString());
    }

    private async Task<bool> UniqueSkuAsync(Product entity, string sku, CancellationToken arg3)
    {
        return !await _productRepository.AnyAsync(
            e => e.Id != entity.Id 
            && e.Sku.Trim().ToLower() == sku.Trim().ToLower()
            && e.IdBrand == entity.IdBrand
            && e.IdSubcategory == entity.IdSubcategory
            && e.IdGender == entity.IdGender);
    }

    private async Task<bool> UniqueNameAsync(Product entity, string name, CancellationToken arg3)
    {
        return !await _productRepository.AnyAsync(e =>
            e.Id != entity.Id 
            && e.Name.Trim().ToLower() == name.Trim().ToLower() 
            && e.IdBrand == entity.IdBrand
            && e.IdSubcategory == entity.IdSubcategory
            && e.IdGender == entity.IdGender);
    }

    private async Task<bool> SellingPriceGreaterThanPurchasingPriceAsync(Product entity, decimal unitPrice, CancellationToken arg3)
    {
        var stockMaxPurchasePrice = await GetStockMaxPurchasePriceAsync(entity);
        return unitPrice > stockMaxPurchasePrice;
    }

    private async Task<bool> GenderExistsAsync(int? idGender, CancellationToken arg2)
    {
        return !idGender.HasValue || await _genderRepository.AnyAsync(e => e.Id == idGender);
    }

    private async Task<bool> SubcategoryExistsAsync(int idSubcategory, CancellationToken arg2)
    {
        return await _subcategoryRepository.AnyAsync(e => e.Id == idSubcategory);
    }

    private async Task<bool> BrandExistsAsync(int idBrand, CancellationToken arg2)
    {
        return await _brandRepository.AnyAsync(e => e.Id == idBrand);
    }

    private async Task<decimal> GetStockMaxPurchasePriceAsync(Product entity)
    {
        var stocks = await _stockRepository.FindAsync(e => e.IdProduct == entity.Id && e.CurrentQuantity > 0,
            new[] { nameof(Stock.OrderItems) });
        return (stocks?.Max(e => e.PurchasePrice) ?? 0M);
    }
}