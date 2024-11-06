namespace Maliwan.Application.Models.MaliwanContext;

public class OrderModel : EntityIntIdModel
{
    public Guid IdCustomer { get; set; }
    public DateTime SellDate { get; set; }
    public decimal Total { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance { get; set; }
    public CustomerModel Customer { get; set; }
    public IEnumerable<OrderItemModel> OrderItems { get; set; }
    public IEnumerable<OrderPaymentModel> OrderPayments { get; set; }
}