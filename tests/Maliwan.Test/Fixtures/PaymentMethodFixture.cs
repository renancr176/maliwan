using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;

namespace Maliwan.Test.Fixtures;

public class PaymentMethodFixture : IDisposable
{
    public Faker Faker { get; private set; }

    public PaymentMethodFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public PaymentMethod Valid()
    {
        Faker = new Faker("pt_BR");
        return new PaymentMethod($"Forma de Pagamento {Faker.Random.String2(Faker.Random.Int(1,5))}",  true);
    }

    public PaymentMethod Invalid()
    {
        return new PaymentMethod(String.Empty, false);
    }

    public void Dispose()
    {
    }
}