using System.Linq.Expressions;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Stock : Entity
{
    public Guid IdProduct { get; set; }
    public int? IdSize { get; set; }
    public int? IdColor { get; set; }
    public int InputQuantity { get; set; }
    public DateTime InputDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public bool Active { get; set; } = true;

    #region Virtual properties
    
    public virtual string Sku => $@"{Product?.Brand?.Sku ?? "00"}{Product?.Subcategory?.Sku ?? "00"}{Product?.Gender?.Sku ?? "00"}{Size?.Sku ?? "00"}{Color?.Sku ?? "00"}{Product?.Sku ?? "00"}";
    public virtual int CurrentQuantity => CurrentQuantityFunc(this);
    public virtual StockLevelEnum StockLevel => StockLevelFunc(this);

    #endregion

    #region Predicates

    public Func<Stock, int> CurrentQuantityFunc = e => (e.InputQuantity - e.OrderItems.Where(x => !x.DeletedAt.HasValue).Sum(x => x.Quantity));
    public Func<Stock, StockLevelEnum> StockLevelFunc = e => e.CurrentQuantityFunc(e) >= 3
        ? (e.CurrentQuantityFunc(e) >= (2 * (e.InputQuantity / 3))
            ? StockLevelEnum.High
            : (e.CurrentQuantityFunc(e) > (e.InputQuantity / 3) ? StockLevelEnum.Medium : StockLevelEnum.Low))
        : StockLevelEnum.Low;

    #endregion

    #region Relationships

    public virtual Product Product { get; set; }
    public virtual ProductSize Size { get; set; }
    public virtual ProductColor Color { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

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