using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class ProductColorSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? Sku { get; set; }

    public ProductColorSearchRequest()
    {
    }

    public ProductColorSearchRequest(string? name = null, string? sku = null)
    {
        Name = name;
        Sku = sku;
    }
}