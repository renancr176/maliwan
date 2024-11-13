namespace Maliwan.Application.Commands.MaliwanContext.StockCommands;

public class UpdateStockCommand : BaseStockCommand
{
    public Guid Id { get; set; }

    public UpdateStockCommand()
    {
    }

    public UpdateStockCommand(Guid id, int inputQuantity, DateTime inputDate, decimal purchasePrice, bool active = true, int? idSize = null, int? idColor = null)
        : base(inputQuantity, inputDate, purchasePrice, active, idSize, idColor)
    {
        Id = id;
    }
}