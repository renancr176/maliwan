using Maliwan.Domain.Core.Requests;
using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class CustomerSearchRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? Document { get; set; }
    public CustomerTypeEnum? Type { get; set; }

    public CustomerSearchRequest()
    {
    }

    public CustomerSearchRequest(string? name = null, string? document = null, CustomerTypeEnum? type = null)
    {
        Name = name;
        Document = document;
        Type = type;
    }
}