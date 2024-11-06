namespace Maliwan.Application.Models.MaliwanContext;

public class PaymentMethodModel : EntityIntIdModel
{
    public string Name { get; set; }
    public bool Active { get; set; }
}