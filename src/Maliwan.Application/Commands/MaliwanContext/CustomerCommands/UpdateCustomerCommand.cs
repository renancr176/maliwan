namespace Maliwan.Application.Commands.MaliwanContext.CustomerCommands;

public class UpdateCustomerCommand : CreateCustomerCommand
{
    public Guid Id { get; set; }

    public UpdateCustomerCommand()
    {
    }

    public UpdateCustomerCommand(Guid id, string name, string document)
        : base(name, document)
    {
        Id = id;
    }
}