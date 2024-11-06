using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class Gender : EntityIntId
{
    public string Name { get; set; }
    public string Sku { get; set; }

    #region Relationships

    public virtual ICollection<Product> Products { get; set; }

    #endregion

    public Gender()
    {
    }

    public Gender(string name, string sku)
    {
        Name = name;
        Sku = sku;
    }
}