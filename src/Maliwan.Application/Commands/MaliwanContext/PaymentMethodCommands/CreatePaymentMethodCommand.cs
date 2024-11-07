using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;

public class CreatePaymentMethodCommand : Command<PaymentMethodModel?>
{
    public string Name { get; set; }
    public bool Active { get; set; }

    public CreatePaymentMethodCommand()
    {
    }

    public CreatePaymentMethodCommand(string name, bool active)
    {
        Name = name;
        Active = active;
    }
}