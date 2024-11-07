namespace Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;

public class UpdateProductColorCommand : CreateProductColorCommand
{
    public int Id { get; set; }

    public UpdateProductColorCommand()
    {
    }

    public UpdateProductColorCommand(int id, string name, string sku, string bgColor, string textColor)
        : base(name, sku, bgColor, textColor)
    {
        Id = id;
    }
}