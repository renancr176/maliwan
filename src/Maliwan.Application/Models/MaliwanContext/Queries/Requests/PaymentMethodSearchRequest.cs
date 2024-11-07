using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class PaymentMethodSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public bool? Active { get; set; }

    public PaymentMethodSearchRequest()
    {
    }

    public PaymentMethodSearchRequest(string? name = null, bool? active = null)
    {
        Name = name;
        Active = active;
    }
}