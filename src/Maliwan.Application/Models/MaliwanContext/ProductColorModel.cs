namespace Maliwan.Application.Models.MaliwanContext;

public class ProductColorModel : EntityIntIdModel
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string BgColor { get; set; }
    public string TextColor { get; set; }
}