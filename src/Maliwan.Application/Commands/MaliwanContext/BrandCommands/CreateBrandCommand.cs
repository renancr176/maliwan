using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.BrandCommands;

public class CreateBrandCommand : Command<BrandModel?>
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }

    public CreateBrandCommand()
    {
    }

    public CreateBrandCommand(string name, string sku, bool active)
    {
        Name = name;
        Sku = sku;
        Active = active;
    }
}