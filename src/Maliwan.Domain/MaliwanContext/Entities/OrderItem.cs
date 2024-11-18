using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

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

    public OrderItem(Guid idStock, int quantity, decimal unitPrice, decimal discount)
    {
        IdStock = idStock;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
    }

    public OrderItem(int idOrder, Guid idStock, int quantity, decimal unitPrice, decimal discount)
        : this(idStock, quantity, unitPrice, discount)
    {
        IdOrder = idOrder;
    }
}