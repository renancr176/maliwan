using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class OrderValidator : EntityValidator<Order>, IOrderValidator
{
    #region Errors codes with message

    public const string CustomerNotExists = "Message from translation file.";

    public const string SellMinVal = "Message from translation file.";
    public const string SellMaxVal = "Message from translation file.";

    public const string OrdermItemsDuplicates = "Message from translation file.";

    public const string TotalDiscountMaxVal = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<OrderValidator> _localizer;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderPaymentValidator _orderPaymentValidator;
    private readonly IOrderItemValidator _orderItemValidator;

    public OrderValidator(IStringLocalizer<OrderValidator> localizer, IOrderRepository orderRepository,
        ICustomerRepository customerRepository, IOrderPaymentValidator orderPaymentValidator,
        IOrderItemValidator orderItemValidator)
    {
        _localizer = localizer;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _orderPaymentValidator = orderPaymentValidator;
        _orderItemValidator = orderItemValidator;

        RuleForEach(e => e.OrderItems)
            .SetValidator(_orderItemValidator);

        RuleForEach(e => e.OrderPayments)
            .SetValidator(_orderPaymentValidator);

        RuleFor(e => e.IdCustomer)
            .MustAsync(CustomerExistsAsync)
            .WithErrorCode(nameof(CustomerNotExists))
            .WithMessage(_localizer.GetString(nameof(CustomerNotExists)));

        RuleFor(e => e.SellDate)
            .GreaterThan(DateTime.MinValue)
            .WithErrorCode(nameof(SellMinVal))
            .WithMessage(_localizer.GetString(nameof(SellMinVal)).ToString().Replace("#MinVal", DateTime.MinValue.FromUtcToBrTimeZone().ToString("G")))
            .LessThan(DateTime.UtcNow.AddMinutes(30))
            .WithErrorCode(nameof(SellMaxVal))
            .WithMessage(_localizer.GetString(nameof(SellMaxVal)).ToString().Replace("#MaxVal", DateTime.UtcNow.FromUtcToBrTimeZone().AddMinutes(30).ToString("G")));

        RuleFor(e => e.OrderItems)
            .MustAsync(NoDuplicateStockItemsAsync)
            .WithErrorCode(nameof(OrdermItemsDuplicates))
            .WithMessage(_localizer.GetString(nameof(OrdermItemsDuplicates)));

        When(e => e.Total > 0M, () =>
        {
            RuleFor(e => e.TotalDiscount)
                .Must(TotalDiscountLessThanOrEqualsToTotal)
                .WithErrorCode(nameof(TotalDiscountMaxVal))
                .WithMessage(e => _localizer.GetString(nameof(TotalDiscountMaxVal)).ToString().Replace("#MaxVal", e.Total.ToCurrencyString()));
        });
    }

    private bool TotalDiscountLessThanOrEqualsToTotal(Order entity, decimal totalDiscount)
    {
        return totalDiscount <= entity.Total;
    }

    private async Task<bool> NoDuplicateStockItemsAsync(Order entity, ICollection<OrderItem> orderItems, CancellationToken arg3)
    {
        var stockIds = orderItems.Select(e => e.IdStock).ToList();
        if (await _orderRepository.AnyAsync(e => e.Id == entity.Id))
        {
            stockIds.AddRange(
                (await _orderRepository.FirstOrDefaultAsync(e => e.Id == entity.Id, new[] { nameof(Order.OrderItems) }))
                ?.OrderItems?.Where(e => !orderItems.Select(x => x.Id).Contains(e.Id))?.Select(e => e.IdStock) ?? new List<Guid>());
        }

        return !stockIds
            .GroupBy(x => x)
            .Any(g => g.Count() > 1);
    }

    private async Task<bool> CustomerExistsAsync(Guid idCustomer, CancellationToken arg2)
    {
        return await _customerRepository.AnyAsync(e => e.Id == idCustomer);
    }
}