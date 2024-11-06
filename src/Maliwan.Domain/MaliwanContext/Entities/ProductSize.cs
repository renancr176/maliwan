using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class ProductSize : EntityIntId
{
    public string Name { get; set; }
    public string Sku { get; set; }

    #region Relationships

    public virtual ICollection<Stock> Stocks { get; set; }

    #endregion

    public ProductSize()
    {
    }

    public ProductSize(string name, string sku)
    {
        Name = name;
        Sku = sku;
    }
}