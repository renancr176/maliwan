using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.CustomerCommands;

public class CreateCustomerCommand : Command<CustomerModel?>
{
    public string Name { get; set; }
    public string Document { get; set; }

    public CreateCustomerCommand()
    {
    }

    public CreateCustomerCommand(string name, string document)
    {
        Name = name;
        Document = document;
    }
}