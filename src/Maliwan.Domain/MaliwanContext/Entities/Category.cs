using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Category : EntityIntId
{
    public string Name { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual ICollection<Subcategory> Subcategories { get; set; }

    #endregion

    public Category()
    {
    }

    public Category(string name, bool active)
    {
        Name = name;
        Active = active;
    }
}