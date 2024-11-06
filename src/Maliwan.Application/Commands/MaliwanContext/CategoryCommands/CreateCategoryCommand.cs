using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

namespace Maliwan.Application.Commands.MaliwanContext.CategoryCommands;

public class CreateCategoryCommand : BaseCategoryCommand
{
    public IEnumerable<CreateSubcategoryCommand>? Subcategories { get; set; }

    public CreateCategoryCommand()
    {
    }

    public CreateCategoryCommand(string name, bool active, IEnumerable<CreateSubcategoryCommand>? subcategories = null)
        : base(name, active)
    {
        Subcategories = subcategories;
    }
}