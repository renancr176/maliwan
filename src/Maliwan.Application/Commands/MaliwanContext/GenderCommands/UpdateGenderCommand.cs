namespace Maliwan.Application.Commands.MaliwanContext.GenderCommands;

public class UpdateGenderCommand : CreateGenderCommand
{
    public int Id { get; set; }

    public UpdateGenderCommand()
    {
    }

    public UpdateGenderCommand(int id, string name, string sku)
        : base(name, sku)
    {
        Id = id;
    }
}