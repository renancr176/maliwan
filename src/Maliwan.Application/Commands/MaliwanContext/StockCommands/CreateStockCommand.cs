namespace Maliwan.Application.Commands.MaliwanContext.StockCommands;

public class CreateStockCommand : BaseStockCommand
{
    public Guid IdProduct { get; set; }
    
    public CreateStockCommand()
    {
    }

    public CreateStockCommand(Guid idProduct, int inputQuantity, DateTime inputDate, decimal purchasePrice, bool active = true, int? idSize = null, int? idColor = null)
        : base(inputQuantity, inputDate, purchasePrice, active, idSize, idColor)
    {
        IdProduct = idProduct;
    }
}