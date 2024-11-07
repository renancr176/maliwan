using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.CustomerCommands;

public class DeleteCustomerCommand : Command
{
    public Guid Id { get; set; }
}