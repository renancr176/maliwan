using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class Order : EntityIntId
{
    public Guid IdCustomer { get; set; }
    public DateTime SellDate { get; set; } = DateTime.UtcNow;
    public decimal Total => (OrderItems?.Sum(e => e.Total) ?? 0M);
    public decimal TotalDiscount => (OrderItems?.Sum(e => e.Discount) ?? 0M);
    public decimal Subtotal => Total - TotalDiscount;
    public decimal TotalPaid => OrderPayments?.Sum(e => e.AmountPaid) ?? 0M;
    public decimal OutstandingBalance => (Subtotal - TotalPaid);

    #region Relationships

    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<OrderPayment> OrderPayments { get; set; }

    #endregion

    public Order()
    {
    }

    public Order(Guid idCustomer, DateTime sellDate)
    {
        IdCustomer = idCustomer;
        SellDate = sellDate;
    }
}