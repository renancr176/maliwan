using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.GenderCommands;

public class CreateGenderCommand : Command<GenderModel?>
{
    public string Name { get; set; }
    public string Sku { get; set; }

    public CreateGenderCommand()
    {
    }

    public CreateGenderCommand(string name, string sku)
    {
        Name = name;
        Sku = sku;
    }
}