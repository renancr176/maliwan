namespace Maliwan.Application.Commands.MaliwanContext.BrandCommands;

public class UpdateBrandCommand : CreateBrandCommand
{
    public int Id { get; set; }

    public UpdateBrandCommand()
    {
    }

    public UpdateBrandCommand(int id, string name, string sku, bool active)
        : base(name, sku, active)
    {
        Id = id;
    }
}