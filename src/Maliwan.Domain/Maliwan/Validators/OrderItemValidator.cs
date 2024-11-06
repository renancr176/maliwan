using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Maliwan.Domain.Maliwan.Validators;

public class OrderItemValidator : EntityValidator<OrderItem>, IOrderItemValidator
{
    #region Errors codes with message

    public const string StockNotExists = "Message from translation file.";
    public const string StockNotUniqueInOrder = "Message from translation file.";

    public const string QuantityMinVal = "Message from translation file.";
    public const string QuantityMaxVal = "Message from translation file.";

    //public const string UnitPriceMinVal = "Message from translation file.";
    public const string UnitPriceInvalidVal = "Message from translation file.";

    public const string DiscountMinVal = "Message from translation file.";
    public const string DiscountMaxVal = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<OrderItemValidator> _localizer;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IStockRepository _stockRepository;

    public OrderItemValidator(IStringLocalizer<OrderItemValidator> localizer, IOrderItemRepository orderItemRepository,
        IStockRepository stockRepository)
    {
        _localizer = localizer;
        _orderItemRepository = orderItemRepository;
        _stockRepository = stockRepository;

        RuleFor(e => e.IdStock)
            .Cascade(CascadeMode.Stop)
            .MustAsync(StockExistsAsync)
            .WithErrorCode(nameof(StockNotExists))
            .WithMessage(_localizer.GetString(nameof(StockNotExists)))
            .MustAsync(UniqueStockInOrderAsync)
            .WithErrorCode(nameof(StockNotUniqueInOrder))
            .WithMessage(_localizer.GetString(nameof(StockNotUniqueInOrder)));

        RuleFor(e => e.Quantity)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(nameof(QuantityMinVal))
            .WithMessage(_localizer.GetString(nameof(QuantityMinVal)).ToString().Replace("#MinVal", "0"))
            .MustAsync(HaveQuantityInStockAsync)
            .WithErrorCode(nameof(QuantityMaxVal))
            .WithMessage(GetQuantityMaxValMessage);

        RuleFor(e => e.UnitPrice)
            //.GreaterThan(0)
            //.WithErrorCode(nameof(UnitPriceMinVal))
            //.WithMessage(_localizer.GetString(nameof(UnitPriceMinVal)).ToString().Replace("#MinVal", (0M).ToString("C", new CultureInfo("pt-BR"))))
            .MustAsync(SameProductUnitPriceAsync)
            .WithErrorCode(nameof(UnitPriceInvalidVal))
            .WithMessage(GetUnitPriceInvalidValMessage);

        RuleFor(e => e.Discount)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(nameof(DiscountMinVal))
            .WithMessage(e => GetDiscountMinValMessage(e, 0))
            .LessThanOrEqualTo(e => e.Total)
            .WithErrorCode(nameof(DiscountMaxVal))
            .WithMessage(GetDiscountMaxValMessage);
    }

    private string GetDiscountMinValMessage(OrderItem entity, decimal minVal)
    {
        var message = _localizer.GetString(nameof(DiscountMinVal)).ToString();

        Product product = null;
        Task.Run(async () =>
        {
            product = (await _stockRepository.GetByIdAsync(entity.IdStock, new[] { nameof(Stock.Product) }))?.Product;
        }).Wait();

        return message
            .Replace("#ProductName", product?.Name ?? "")
            .Replace("#MinVal", minVal.ToCurrencyString());
    }

    private string GetDiscountMaxValMessage(OrderItem entity)
    {
        var message = _localizer.GetString(nameof(DiscountMaxVal)).ToString();

        Product product = null;
        Task.Run(async () =>
        {
            product = (await _stockRepository.GetByIdAsync(entity.IdStock, new[] { nameof(Stock.Product) }))?.Product;
        }).Wait();

        return message
            .Replace("#ProductName", product?.Name ?? "")
            .Replace("#MaxVal", entity.Total.ToCurrencyString());
    }

    private async Task<bool> SameProductUnitPriceAsync(OrderItem entity, decimal unitPrice, CancellationToken arg3)
    {
        var stock = await GetStockAsync(
            entity.IdStock, 
            new[] { nameof(Stock.Product) });

        return unitPrice == (stock?.Product?.UnitPrice ?? 0M);
    }

    private async Task<bool> UniqueStockInOrderAsync(OrderItem entity, Guid idStock, CancellationToken arg3)
    {
        return !await _orderItemRepository.AnyAsync(e =>
            e.Id != entity.Id && e.IdOrder == entity.IdOrder && e.IdStock == idStock);
    }

    private async Task<bool> HaveQuantityInStockAsync(OrderItem entity, int quantity, CancellationToken arg3)
    {
        var stock = await GetStockAsync(
            entity.IdStock,
            new[] {nameof(Stock.OrderItems)});

        return quantity <= (stock?.CurrentQuantity ?? 0);
    }

    private async Task<bool> StockExistsAsync(OrderItem entity, Guid idStock, CancellationToken arg3)
    {
        return await _stockRepository.AnyAsync(e => e.Id == idStock);
    }

    #region No Validation Methods
    
    private async Task<Stock?> GetStockAsync(Guid idStock, IEnumerable<string> includes = null)
    {
        return await _stockRepository.FirstOrDefaultAsync(e => e.Id == idStock, includes);
    }

    #endregion

    #region Get Dinamic Message

    private string GetUnitPriceInvalidValMessage(OrderItem entity)
    {
        var message = _localizer.GetString(nameof(UnitPriceInvalidVal)).ToString();
        Stock? stock = null;
        Task.Run(async () =>
        {
            stock = await GetStockAsync(
                entity.IdStock, 
                new[] { nameof(Stock.Product) });
        }).Wait();
        return message
            .Replace("#ProductName", stock?.Product?.Name ?? "")
            .Replace("#ProductUnitPrice", (stock?.Product?.UnitPrice ?? 0M).ToString("C", new CultureInfo("pt-BR")));
    }

    private string GetQuantityMaxValMessage(OrderItem entity)
    {
        var message = _localizer.GetString(nameof(QuantityMaxVal)).ToString();
        Stock? stock = null;
        Task.Run(async () =>
        {
            var stock = await GetStockAsync(
                entity.IdStock,
                new[]
                {
                    nameof(Stock.Product),
                    nameof(Stock.OrderItems)
                });
        }).Wait();

        return message
            .Replace("#ProductName", stock?.Product?.Name ?? "")
            .Replace(
            "#StockCurrentQuantity",
            (stock?.CurrentQuantity ?? 0) > 0 ? (stock?.CurrentQuantity ?? 0).ToString() : "");
    }

    #endregion
}