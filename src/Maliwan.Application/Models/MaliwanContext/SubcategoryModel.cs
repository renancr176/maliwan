namespace Maliwan.Application.Models.MaliwanContext;

public class SubcategoryModel : EntityIntIdModel
{
    public int IdCategory { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }
    public CategoryModel Category { get; set; }
}