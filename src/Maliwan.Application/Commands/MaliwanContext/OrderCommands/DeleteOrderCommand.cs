using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.OrderCommands;

public class DeleteOrderCommand : Command
{
    public int Id { get; set; }
}