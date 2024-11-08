using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductCommands;

public class DeleteProductCommand : Command
{
    public Guid Id { get; set; }
}