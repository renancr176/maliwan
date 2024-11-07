using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Commands.MaliwanContext.CustomerCommands;

public class UpdateCustomerCommand : CreateCustomerCommand
{
    public Guid Id { get; set; }

    public UpdateCustomerCommand()
    {
    }

    public UpdateCustomerCommand(Guid id, string name, string document, CustomerTypeEnum type)
        : base(name, document, type)
    {
        Id = id;
    }
}