namespace Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;

public class UpdatePaymentMethodCommand : CreatePaymentMethodCommand
{
    public int Id { get; set; }

    public UpdatePaymentMethodCommand()
    {
    }

    public UpdatePaymentMethodCommand(int id, string name, bool active)
        : base(name, active)
    {
        Id = id;
    }
}