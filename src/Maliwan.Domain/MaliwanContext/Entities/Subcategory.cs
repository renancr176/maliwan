﻿using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Subcategory : EntityIntId
{
    public int IdCategory { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual Category Category { get; set; }
    public virtual ICollection<Product> Products { get; set; }

    #endregion

    public Subcategory()
    {
    }

    public Subcategory(int idCategory, string name, string sku, bool active = true)
    {
        IdCategory = idCategory;
        Name = name;
        Sku = sku;
        Active = active;
    }
}