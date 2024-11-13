using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class BrandFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");

    public Brand Valid()
    {
        var name = Faker.Company.CompanyName();
        var sku = name.GetSku();
        return new Brand(name, sku, true);
    }

    public Brand Invalid()
    {
        return new Brand(String.Empty, String.Empty, false);
    }

    public void Dispose()
    {
    }
}