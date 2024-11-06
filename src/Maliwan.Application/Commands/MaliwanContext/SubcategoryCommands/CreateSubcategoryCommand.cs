using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

public class CreateSubcategoryCommand : Command<SubcategoryModel?>
{
    public int IdCategory { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public bool Active { get; set; }
}