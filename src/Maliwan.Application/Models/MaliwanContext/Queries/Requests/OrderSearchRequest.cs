using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class OrderSearchRequest : PagedRequest
{
    public Guid? IdCustomer { get; set; }
    public DateTime? SellDate { get; set; }

    public OrderSearchRequest()
    {
    }

    public OrderSearchRequest(Guid? idCustomer = null, DateTime? sellDate = null)
    {
        IdCustomer = idCustomer;
        SellDate = sellDate;
    }
}