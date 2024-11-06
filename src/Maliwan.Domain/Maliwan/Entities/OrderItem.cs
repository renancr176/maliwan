using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class OrderItem : Entity
{
    public int IdOrder { get; set; }
    public Guid IdStock { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } = 0M;
    public decimal Total => (UnitPrice * Quantity);

    #region Relationships

    public virtual Order Order { get; set; }
    public virtual Stock Stock { get; set; }

    #endregion

    public OrderItem()
    {
    }

    public OrderItem(int idOrder, Guid idStock, int quantity, decimal unitPrice, decimal discount)
    {
        IdOrder = idOrder;
        IdStock = idStock;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
    }
}