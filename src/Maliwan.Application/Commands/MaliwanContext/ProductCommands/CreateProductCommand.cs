using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductCommands;

public class CreateProductCommand : Command<ProductModel?>
{
    public int IdBrand { get; set; }
    public int IdSubcategory { get; set; }
    public int? IdGender { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }

    public CreateProductCommand()
    {
    }

    public CreateProductCommand(int idBrand, int idSubcategory, string name, decimal unitPrice, string sku, bool active, int? idGender = null)
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