using Bogus;
using Maliwan.Domain.Maliwan.Entities;

namespace Maliwan.Test.Fixtures;

public class CategoryFixture : IDisposable
{
    public Faker Faker { get; private set; }

    public CategoryFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public Category Valid()
    {
        Faker = new Faker("pt_BR");
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