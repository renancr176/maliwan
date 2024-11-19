using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;

public class CreateOrderPaymentCommand : Command<OrderPaymentModel?>
{
    public int IdOrder { get; set; }
    public int IdPaymentMethod { get; set; }
    public decimal AmountPaid { get; set; }

    public CreateOrderPaymentCommand()
    {
    }

    public CreateOrderPaymentCommand(int idOrder, int idPaymentMethod, decimal amountPaid)
    {
        IdOrder = idOrder;
        IdPaymentMethod = idPaymentMethod;
        AmountPaid = amountPaid;
    }
}