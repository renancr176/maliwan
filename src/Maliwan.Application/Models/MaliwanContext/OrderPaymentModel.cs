namespace Maliwan.Application.Models.MaliwanContext;

public class OrderPaymentModel : EntityModel
{
    public int IdOrder { get; set; }
    public int IdPaymentMethod { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public virtual PaymentMethodModel PaymentMethod { get; set; }
}