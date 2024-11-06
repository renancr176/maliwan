namespace Maliwan.Application.Models.MaliwanContext;

public class BrandModel : EntityIntIdModel
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }
}