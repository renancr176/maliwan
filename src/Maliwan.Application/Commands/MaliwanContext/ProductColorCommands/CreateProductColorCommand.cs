using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;

public class CreateProductColorCommand : Command<ProductColorModel?>
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string BgColor { get; set; }
    public string TextColor { get; set; }

    public CreateProductColorCommand()
    {
    }

    public CreateProductColorCommand(string name, string sku, string bgColor, string textColor)
    {
        Name = name;
        Sku = sku;
        BgColor = bgColor;
        TextColor = textColor;
    }
}