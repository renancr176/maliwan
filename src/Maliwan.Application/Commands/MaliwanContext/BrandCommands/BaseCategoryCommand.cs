using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.BrandCommands;

public class BaseCategoryCommand : Command<CategoryModel?>
{
    public string Name { get; set; }
    public bool Active { get; set; }

    public BaseCategoryCommand()
    {
    }

    public BaseCategoryCommand(string name, bool active)
    {
        Name = name;
        Active = active;
    }
}