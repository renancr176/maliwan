using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;
using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Commands.MaliwanContext.CustomerCommands;

public class CreateCustomerCommand : Command<CustomerModel?>
{
    public string Name { get; set; }
    public string Document { get; set; }
    public CustomerTypeEnum Type { get; set; }

    public CreateCustomerCommand()
    {
    }

    public CreateCustomerCommand(string name, string document, CustomerTypeEnum type)
    {
        Name = name;
        Document = document;
        Type = type;
    }
}