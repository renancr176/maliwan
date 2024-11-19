using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;

public class DeleteOrderPaymentCommand : Command
{
    public Guid Id { get; set; }
}