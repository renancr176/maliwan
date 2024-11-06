using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Maliwan.Enums;

namespace Maliwan.Domain.Maliwan.Entities;

public class Stock : Entity
{
    public Guid IdProduct { get; set; }
    public int? IdSize { get; set; }
    public int? IdColor { get; set; }
    public int InputQuantity { get; set; }
    public DateTime InputDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public bool Active { get; set; } = true;
    public int CurrentQuantity => (
        InputQuantity - (
            OrderItems.Any() 
                ? OrderItems.Sum(x => x.Quantity) 
                : 0
            )
        );
    public string Sku => $@"{Product?.Brand?.Sku ?? "00"}
{Product?.Subcategory?.Sku ?? "00"}
{Product?.Gender?.Sku ?? "00"}
{Size?.Sku ?? "00"}
{Color?.Sku ?? "00"}
{Product?.Sku ?? "00"}";
    public StockLevelEnum StockLevel => CurrentQuantity >= 3
        ? (CurrentQuantity >= (2 * (InputQuantity / 3))
            ? StockLevelEnum.High
            : (CurrentQuantity > (InputQuantity / 3) ? StockLevelEnum.Medium : StockLevelEnum.Low))
        : StockLevelEnum.Low;

    #region Relationships

    public virtual Product Product { get; set; }
    public virtual ProductSize Size { get; set; }
    public virtual ProductColor Color { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }

    #endregion

    public Stock()
    {
    }

    public Stock(Guid idProduct, int? idSize, int? idColor, int inputQuantity, DateTime inputDate, decimal purchasePrice, bool active = true)
    {
        IdProduct = idProduct;
        IdSize = idSize;
        IdColor = idColor;
        InputQuantity = inputQuantity;
        InputDate = inputDate;
        PurchasePrice = purchasePrice;
        Active = active;
    }
}