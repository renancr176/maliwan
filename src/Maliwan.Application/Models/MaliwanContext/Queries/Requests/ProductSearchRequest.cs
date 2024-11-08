using Maliwan.Domain.Core.Requests;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class ProductSearchRequest : PagedRequest
{
    public int? IdBrand { get; set; }
    public int? IdCategory { get; set; }
    public int? IdSubcategory { get; set; }
    public int? IdGender { get; set; }
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sku { get; set; }
    public bool? Active { get; set; }

    public ProductSearchRequest()
    {
    }

    public ProductSearchRequest(int? idBrand = null, int? idCategory = null, int? idSubcategory = null,
        int? idGender = null, string? name = null, decimal? minPrice = null, decimal? maxPrice = null,
        string? sku = null, bool? active = null)
    {
        IdBrand = idBrand;
        IdCategory = idCategory;
        IdSubcategory = idSubcategory;
        IdGender = idGender;
        Name = name;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        Sku = sku;
        Active = active;
    }
}