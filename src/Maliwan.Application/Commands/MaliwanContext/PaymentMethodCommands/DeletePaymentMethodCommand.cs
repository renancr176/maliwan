using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;

public class DeletePaymentMethodCommand : Command
{
    public int Id { get; set; }
}