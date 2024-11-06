namespace Maliwan.Application.Models.MaliwanContext;

public class ProductModel : EntityModel
{
    public int IdBrand { get; set; }
    public int IdSubcategory { get; set; }
    public int? IdGender { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }
    public BrandModel Brand { get; set; }
    public SubcategoryModel Subcategory { get; set; }
    public GenderModel Gender { get; set; }
    public IEnumerable<StockModel> Stocks { get; set; }
}