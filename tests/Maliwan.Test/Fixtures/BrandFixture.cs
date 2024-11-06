using System.Text.RegularExpressions;
using Bogus;
using Maliwan.Domain.Maliwan.Entities;

namespace Maliwan.Test.Fixtures;

public class BrandFixture : IDisposable
{
    public Faker Faker { get; private set; }

    public BrandFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public string GetSku(string name)
    {
        var normalizedName = Regex.Replace(Regex.Replace(name, @"[^a-zA-Z0-9 \s+]", ""), @"\s+", " ");
        return (normalizedName.Trim().Split(" ").Length > 1
            ? $"{normalizedName.Trim().Split(" ")[0].ToCharArray()[0].ToString()}{normalizedName.Trim().Split(" ")[1].ToCharArray()[0].ToString()}"
            : $"{Faker.PickRandom(normalizedName.Trim().ToCharArray()).ToString()}{Faker.PickRandom(normalizedName.Trim().ToCharArray()).ToString()}")
            .ToUpper();
    }

    public Brand Valid()
    {
        Faker = new Faker("pt_BR");
        var name = Faker.Company.CompanyName();
        var sku = GetSku(name);
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