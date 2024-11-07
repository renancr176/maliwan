using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;

public class DeleteProductSizeCommand : Command
{
    public int Id { get; set; }
}