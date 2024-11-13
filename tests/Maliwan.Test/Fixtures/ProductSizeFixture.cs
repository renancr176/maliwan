using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class ProductSizeFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public ProductSize Valid()
    {
        var name = $"Tamanho {Faker.Random.String2(Faker.Random.Int(1,5))}";
        var sku = name.GetSku();
        return new ProductSize(name, sku);
    }

    public ProductSize Invalid()
    {
        return new ProductSize(String.Empty, String.Empty);
    }

    public void Dispose()
    {
    }
}