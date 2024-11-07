using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class SubcategoryFixture : IDisposable
{
    public Faker Faker { get; private set; }

    public SubcategoryFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public Subcategory Valid()
    {
        Faker = new Faker("pt_BR");
        var name = $"Subcategoria {Faker.Random.String2(Faker.Random.Int(1, 5))}";
        return new Subcategory(1, name, name.GetSku(), true);
    }

    public Subcategory Invalid()
    {
        return new Subcategory(0, String.Empty, String.Empty, false);
    }

    public void Dispose()
    {
    }
}