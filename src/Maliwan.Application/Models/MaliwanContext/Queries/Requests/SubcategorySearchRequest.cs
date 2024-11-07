using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class SubcategorySearchRequest : PagedRequest
{
    public int? IdCategory { get; set; }
    public string? Name { get; set; }
    public string? Sku { get; set; }
    public bool? Active { get; set; }

    public SubcategorySearchRequest()
    {
    }

    public SubcategorySearchRequest(int? idCategory = null, string? name = null, string? sku = null, bool? active = null)
    {
        IdCategory = idCategory;
        Name = name;
        Sku = sku;
        Active = active;
    }
}