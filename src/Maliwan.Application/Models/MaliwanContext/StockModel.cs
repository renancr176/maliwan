using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Models.MaliwanContext;

public class StockModel : EntityModel
{
    public Guid IdProduct { get; set; }
    public int? IdSize { get; set; }
    public int? IdColor { get; set; }
    public int InputQuantity { get; set; }
    public DateTime InputDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public bool Active { get; set; }
    public int CurrentQuantity { get; set; }
    public string Sku { get; set; }
    public StockLevelEnum StockLevel { get; set; }
    public ProductModel Product { get; set; }
    public ProductSizeModel Size { get; set; }
    public ProductColorModel Color { get; set; }
}