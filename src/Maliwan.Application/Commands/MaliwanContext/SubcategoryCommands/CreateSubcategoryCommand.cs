using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

public class CreateSubcategoryCommand : Command<SubcategoryModel?>
{
    public int IdCategory { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }

    public CreateSubcategoryCommand()
    {
    }

    public CreateSubcategoryCommand(int idCategory, string name, string sku, bool active)
    {
        IdCategory = idCategory;
        Name = name;
        Sku = sku;
        Active = active;
    }
}