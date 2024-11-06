using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.Maliwan.Entities;

public class Category : EntityIntId
{
    public string Name { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual ICollection<Subcategory> SubCategories { get; set; }

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