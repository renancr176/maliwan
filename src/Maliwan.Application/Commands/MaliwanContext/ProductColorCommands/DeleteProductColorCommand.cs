using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;

public class DeleteProductColorCommand : Command
{
    public int Id { get; set; }
}