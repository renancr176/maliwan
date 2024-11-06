namespace Maliwan.Application.Models.MaliwanContext;

public class OrderItemModel : EntityModel
{
    public int IdOrder { get; set; }
    public Guid IdStock { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public StockModel Stock { get; set; }
}