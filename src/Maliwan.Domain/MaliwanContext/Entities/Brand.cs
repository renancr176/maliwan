using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Brand : EntityIntId
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual ICollection<Product> Products { get; set; }

    #endregion

    public Brand()
    {
    }

    public Brand(string name, string sku, bool active)
    {
        Name = name;
        Sku = sku;
        Active = active;
    }
}