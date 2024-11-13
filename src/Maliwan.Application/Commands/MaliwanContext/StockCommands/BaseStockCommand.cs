using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.StockCommands;

public class BaseStockCommand : Command<StockModel?>
{
    public int? IdSize { get; set; }
    public int? IdColor { get; set; }
    public int InputQuantity { get; set; }
    public DateTime InputDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public bool Active { get; set; }

    public BaseStockCommand()
    {
    }

    public BaseStockCommand(int inputQuantity, DateTime inputDate, decimal purchasePrice, bool active = true, int? idSize = null, int? idColor = null)
    {
        IdSize = idSize;
        IdColor = idColor;
        InputQuantity = inputQuantity;
        InputDate = inputDate;
        PurchasePrice = purchasePrice;
        Active = active;
    }
}