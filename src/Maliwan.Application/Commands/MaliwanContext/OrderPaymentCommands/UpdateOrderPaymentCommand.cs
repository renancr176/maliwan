using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;

public class UpdateOrderPaymentCommand : Command<OrderPaymentModel?>
{
    public Guid Id { get; set; }
    public decimal AmountPaid { get; set; }

    public UpdateOrderPaymentCommand()
    {
    }

    public UpdateOrderPaymentCommand(Guid id, decimal amountPaid)
    {
        Id = id;
        AmountPaid = amountPaid;
    }
}