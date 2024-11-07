using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class ProductSizeSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? Sku { get; set; }

    public ProductSizeSearchRequest()
    {
    }

    public ProductSizeSearchRequest(string? name = null, string? sku = null)
    {
        Name = name;
        Sku = sku;
    }
}