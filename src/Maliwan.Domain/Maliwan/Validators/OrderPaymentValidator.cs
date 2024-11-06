using System.Globalization;
using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.Maliwan.Validators;

public class OrderPaymentValidator : EntityValidator<OrderPayment>, IOrderPaymentValidator
{
    #region Errors codes with message

    public const string PaymentMethodNotExists = "Message from translation file.";
    public const string PaymentMethodDuplicate = "Message from translation file.";

    public const string AmountPaidMinVal = "Message from translation file.";
    public const string AmountPaidMaxVal = "Message from translation file.";

    public const string PaymentDateIsRequired = "Message from translation file.";
    public const string PaymentDateMinVal = "Message from translation file.";
    public const string PaymentDateMaxVal = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<OrderPaymentValidator> _localizer;
    private readonly IOrderPaymentRepository _orderPaymentRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderPaymentValidator(IStringLocalizer<OrderPaymentValidator> localizer,
        IOrderPaymentRepository orderPaymentRepository, IPaymentMethodRepository paymentMethodRepository,
        IOrderRepository orderRepository)
    {
        _localizer = localizer;
        _orderPaymentRepository = orderPaymentRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _orderRepository = orderRepository;

        RuleFor(e => e.IdPaymentMethod)
            .MustAsync(PaymentMethodExistsAsync)
            .WithErrorCode(nameof(PaymentMethodNotExists))
            .WithMessage(_localizer.GetString(nameof(PaymentMethodNotExists)))
            .MustAsync(NotHaveDuplicatePaymentsAsync)
            .WithErrorCode(nameof(PaymentMethodDuplicate))
            .WithMessage(_localizer.GetString(nameof(PaymentMethodDuplicate)));

        RuleFor(e => e.AmountPaid)
            .GreaterThan(0)
            .WithErrorCode(nameof(AmountPaidMinVal))
            .WithMessage(_localizer.GetString(nameof(AmountPaidMinVal)).ToString().Replace("#MinVal", (0M).ToCurrencyString()))
            .MustAsync(LessOrEqualsToOrderOutstandingBalanceAsync)
            .WithErrorCode(nameof(AmountPaidMaxVal))
            .WithMessage(GetAmountPaidMaxValMessage);

        RuleFor(e => e.PaymentDate)
            .NotNull()
            .WithErrorCode(nameof(PaymentDateIsRequired))
            .WithMessage(_localizer.GetString(nameof(PaymentDateIsRequired)))
            .GreaterThan(DateTime.MinValue)
            .WithErrorCode(nameof(PaymentDateMinVal))
            .WithMessage(_localizer.GetString(nameof(PaymentDateMinVal)).ToString().Replace("#MinVal", DateTime.MinValue.FromUtcToBrTimeZone().ToString("G")))
            .LessThan(DateTime.UtcNow.AddMinutes(30))
            .WithErrorCode(nameof(PaymentDateMaxVal))
            .WithMessage(_localizer.GetString(nameof(PaymentDateMaxVal)).ToString().Replace("#MaxVal", DateTime.UtcNow.AddMinutes(30).FromUtcToBrTimeZone().ToString("G")));
    }

    private string GetAmountPaidMaxValMessage(OrderPayment entity)
    {
        var message = _localizer.GetString(nameof(AmountPaidMaxVal)).ToString();
        var orderOutstandingBalance = 0M;
        Task.Run(async () =>
        {
            orderOutstandingBalance = await GetOrderOutstandingBalanceAsync(entity);
        }).Wait();
        return message.Replace("#MaxVal", orderOutstandingBalance.ToCurrencyString());
    }

    private async Task<bool> LessOrEqualsToOrderOutstandingBalanceAsync(OrderPayment entity, decimal amountPaid, CancellationToken arg3)
    {
        var orderOutstandingBalance = await GetOrderOutstandingBalanceAsync(entity);

        return amountPaid <= orderOutstandingBalance;
    }

    private async Task<bool> NotHaveDuplicatePaymentsAsync(OrderPayment entity, int idPaymentMethod, CancellationToken arg3)
    {
        return !await _orderPaymentRepository.AnyAsync(e =>
            e.Id != entity.Id && e.IdOrder == entity.IdOrder && e.IdPaymentMethod == idPaymentMethod &&
            e.AmountPaid == entity.AmountPaid && e.PaymentDate == entity.PaymentDate);
    }

    private async Task<bool> PaymentMethodExistsAsync(int idPaymentMethod, CancellationToken arg2)
    {
        return await _paymentMethodRepository.AnyAsync(e => e.Id == idPaymentMethod && e.Active);
    }

    private async Task<decimal> GetOrderOutstandingBalanceAsync(OrderPayment entity)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(e => e.Id == entity.IdOrder, new[] { nameof(Order.OrderPayments) });

        if (order != null && order.OrderPayments.Any(e => e.Id == entity.Id))
        {
            order.OrderPayments.Remove(order.OrderPayments.First(e => e.Id == entity.Id));
        }

        return order?.OutstandingBalance ?? 0M;
    }
}