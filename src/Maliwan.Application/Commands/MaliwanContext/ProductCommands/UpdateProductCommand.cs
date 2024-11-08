namespace Maliwan.Application.Commands.MaliwanContext.ProductCommands;

public class UpdateProductCommand : CreateProductCommand
{
    public Guid Id { get; set; }

    public UpdateProductCommand()
    {
    }

    public UpdateProductCommand(Guid id, int idBrand, int idSubcategory, string name, decimal unitPrice, string sku, bool active, int? idGender = null)
        : base(idBrand, idSubcategory, name, unitPrice, sku, active, idGender)
    {
        Id = id;
    }
}