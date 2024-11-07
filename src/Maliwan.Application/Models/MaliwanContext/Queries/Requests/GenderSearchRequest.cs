using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class GenderSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? Sku { get; set; }

    public GenderSearchRequest()
    {
    }

    public GenderSearchRequest(string? name = null, string? sku = null)
    {
        Name = name;
        Sku = sku;
    }
}