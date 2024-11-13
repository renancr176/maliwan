using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;

namespace Maliwan.Test.Fixtures;

public class StockFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public Stock Valid(Guid? idProduct = null, int? idSize = null, int? idColor = null)
    {
        return new Stock(idProduct ?? Guid.NewGuid(), idSize ?? 1, idColor ?? 1, Faker.Random.Int(1, 20),
            DateTime.UtcNow, Faker.Random.Decimal(9M, 99M));
    }

    public Stock Invalid()
    {
        return new Stock(Guid.Empty, 0, 0, 0, DateTime.MinValue, 0, false);
    }

    public void Dispose()
    {
    }
}