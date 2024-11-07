namespace Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;

public class UpdateProductSizeCommand : CreateProductSizeCommand
{
    public int Id { get; set; }

    public UpdateProductSizeCommand()
    {
    }

    public UpdateProductSizeCommand(int id, string name, string sku)
        : base(name, sku)
    {
        Id = id;
    }
}