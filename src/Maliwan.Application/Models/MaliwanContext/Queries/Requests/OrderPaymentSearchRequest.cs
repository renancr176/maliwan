using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class OrderPaymentSearchRequest : PagedRequest
{
    public int? IdOrder { get; set; }
    public int? IdPaymentMethod { get; set; }
    public decimal? AmountPaidMin { get; set; }
    public decimal? AmountPaidMax { get; set; }
    public DateTime? PaymentDate { get; set; }

    public OrderPaymentSearchRequest()
    {
    }

    public OrderPaymentSearchRequest(int? idOrder = null, int? idPaymentMethod = null, decimal? amountPaidMin = null, decimal? amountPaidMax= null, DateTime? paymentDate = null)
    {
        IdOrder = idOrder;
        IdPaymentMethod = idPaymentMethod;
        AmountPaidMin = amountPaidMin;
        AmountPaidMax = amountPaidMax;
        PaymentDate = paymentDate;
    }
}