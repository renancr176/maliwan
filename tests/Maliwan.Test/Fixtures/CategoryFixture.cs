using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;

namespace Maliwan.Test.Fixtures;

public class CategoryFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public Category Valid()
    {
        return new Category($"Categoria {Faker.Random.String2(Faker.Random.Int(1, 5))}", true);
    }

    public Category Invalid()
    {
        return new Category(String.Empty, false);
    }

    public void Dispose()
    {
    }
}