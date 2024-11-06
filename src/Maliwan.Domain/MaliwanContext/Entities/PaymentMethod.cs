using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class PaymentMethod : EntityIntId
{
    public string Name { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual ICollection<OrderPayment> OrderPayments { get; set; }

    #endregion

    public PaymentMethod()
    {
    }

    public PaymentMethod(string name, bool active)
    {
        Name = name;
        Active = active;
    }
}