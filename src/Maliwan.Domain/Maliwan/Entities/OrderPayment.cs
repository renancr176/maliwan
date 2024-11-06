using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class OrderPayment : Entity
{
    public int IdOrder { get; set; }
    public int IdPaymentMethod { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    #region Relationships

    public virtual Order Order { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }

    #endregion

    public OrderPayment()
    {
    }

    public OrderPayment(int idOrder, int idPaymentMethod, decimal amountPaid, DateTime paymentDate)
    {
        IdOrder = idOrder;
        IdPaymentMethod = idPaymentMethod;
        AmountPaid = amountPaid;
        PaymentDate = paymentDate;
    }
}