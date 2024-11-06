using System.Drawing;
using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class ProductColor : EntityIntId
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string BgColor { get; set; } = "#FFFFFF";
    public string TextColor { get; set; } = "#000000";


    #region Relationships

    public virtual ICollection<Stock> Stocks { get; set; }

    #endregion

    public ProductColor()
    {
    }

    public ProductColor(string name, string sku, string bgColor = "#FFFFFF", string textColor = "#000000")
    {
        Name = name;
        Sku = sku;
        BgColor = bgColor;
        TextColor = textColor;
    }
}