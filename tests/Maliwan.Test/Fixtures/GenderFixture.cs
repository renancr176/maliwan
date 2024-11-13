using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class GenderFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public Gender Valid()
    {
        var name = $"Gender {Faker.Random.String2(Faker.Random.Int(1, 5))}";
        var sku = name.GetSku();
        return new Gender(name, sku);
    }

    public Gender Invalid()
    {
        return new Gender(String.Empty, String.Empty);
    }

    public void Dispose()
    {
    }
}