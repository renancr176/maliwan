using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class CategorySearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public bool? Active { get; set; }

    public CategorySearchRequest()
    {
    }

    public CategorySearchRequest(string? name = null, bool? active = null)
    {
        Name = name;
        Active = active;
    }
}