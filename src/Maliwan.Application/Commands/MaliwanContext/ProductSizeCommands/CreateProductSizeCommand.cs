using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;

public class CreateProductSizeCommand : Command<ProductSizeModel?>
{
    public string Name { get; set; }
    public string Sku { get; set; }

    public CreateProductSizeCommand()
    {
    }

    public CreateProductSizeCommand(string name, string sku)
    {
        Name = name;
        Sku = sku;
    }
}