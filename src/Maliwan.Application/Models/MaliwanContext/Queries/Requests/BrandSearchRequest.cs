using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class BrandSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public bool? Active { get; set; }

    public BrandSearchRequest()
    {
    }

    public BrandSearchRequest(string? name = null, bool? active = null)
    {
        Name = name;
        Active = active;
    }
}