using Maliwan.Application.Commands.MaliwanContext.BrandCommands;

namespace Maliwan.Application.Commands.MaliwanContext.CategoryCommands;

public class UpdateCategoryCommand : BaseCategoryCommand
{
    public int Id { get; set; }

    public UpdateCategoryCommand()
    {
    }

    public UpdateCategoryCommand(int id, string name, bool active)
        : base(name, active)
    {
        Id = id;
    }
}