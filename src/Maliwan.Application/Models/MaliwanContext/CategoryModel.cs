namespace Maliwan.Application.Models.MaliwanContext;

public class CategoryModel : EntityIntIdModel
{
    public string Name { get; set; }
    public bool Active { get; set; }
    public IEnumerable<SubcategoryModel> SubCategories { get; set; }
}