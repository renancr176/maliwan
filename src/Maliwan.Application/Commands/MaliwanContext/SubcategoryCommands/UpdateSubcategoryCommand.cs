namespace Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

public class UpdateSubcategoryCommand : CreateSubcategoryCommand
{
    public int Id { get; set; }

    public UpdateSubcategoryCommand()
    {
    }

    public UpdateSubcategoryCommand(int id, int idCategory, string name, string sku, bool active)
        : base(idCategory, name, sku, active)
    {
        Id = id;
    }
}