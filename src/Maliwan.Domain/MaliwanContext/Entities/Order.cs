using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Order : EntityIntId
{
    public Guid IdCustomer { get; set; }
    public DateTime SellDate { get; set; } = DateTime.UtcNow;

    #region Virtual Properties
    
    public virtual decimal Total => TotalFunc(this);
    public virtual decimal TotalDiscount => TotalDiscountFunc(this);
    public virtual decimal Subtotal => SubtotalFunc(this);
    public virtual decimal TotalPaid => TotalPaidFunc(this);
    public virtual decimal OutstandingBalance => OutstandingBalanceFunc(this);

    #endregion

    #region Predicates

    public Func<Order, decimal> TotalFunc = e => (e.OrderItems?.Sum(e => e.Total) ?? 0M);
    public Func<Order, decimal> TotalDiscountFunc = e => (e.OrderItems?.Sum(e => e.Discount) ?? 0M);
    public Func<Order, decimal> SubtotalFunc = e => (e.TotalFunc(e) - e.TotalDiscountFunc(e));
    public Func<Order, decimal> TotalPaidFunc = e => (e.OrderPayments?.Sum(e => e.AmountPaid) ?? 0M);
    public Func<Order, decimal> OutstandingBalanceFunc = e => (e.Subtotal - e.TotalPaidFunc(e));

    #endregion

    #region Relationships

    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderPayment> OrderPayments { get; set; }

    #endregion

    public Order()
    {
    }

    public Order(Guid idCustomer, ICollection<OrderItem> orderItems)
    {
        IdCustomer = idCustomer;
        OrderItems = orderItems;
    }
}