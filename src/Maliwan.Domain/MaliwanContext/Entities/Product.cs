using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Domain.MaliwanContext.Entities;

public class Product : Entity
{
    public int IdBrand { get; set; }
    public int IdSubcategory { get; set; }
    public int? IdGender { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; } = true;

    #region Relationships

    public virtual Brand Brand { get; set; }
    public virtual Subcategory Subcategory { get; set; }
    public virtual Gender Gender { get; set; }
    public virtual ICollection<Stock> Stocks { get; set; }

    #endregion

    public Product()
    {
    }

    public Product(int idBrand, int idSubcategory, int? idGender, string name, decimal unitPrice, string sku, bool active)
    {
        IdBrand = idBrand;
        IdSubcategory = idSubcategory;
        IdGender = idGender;
        Name = name;
        UnitPrice = unitPrice;
        Sku = sku;
        Active = active;
    }
}