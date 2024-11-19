using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class ProductFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public Product Valid(int? idBrand = null, int? idSubcategory = null, int? idGender = null)
    {
        var name = Faker.Commerce.ProductName();
        var sku = name.GetSku();
        return new Product(idBrand ?? 1, idSubcategory ?? 1, idGender ?? 1, name, decimal.Round(Faker.Random.Decimal(10M, 200M), 2, MidpointRounding.AwayFromZero), sku,
            true);
    }

    public Product Invalid()
    {
        return new Product(0, 0, 0, String.Empty, 0M, String.Empty, false);
    }

    public void Dispose()
    {
    }
}