using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.StockCommands;

public class DeleteStockCommand : Command
{
    public Guid Id { get; set; }
}