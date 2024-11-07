using System.Drawing;
using Bogus;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Test.Extensions;

namespace Maliwan.Test.Fixtures;

public class ProductColorFixture : IDisposable
{
    public Faker Faker { get; private set; }

    public ProductColorFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public ProductColor Valid()
    {
        Faker = new Faker("pt_BR");
        
        var bgColor = Faker.Internet.Color(Faker.Random.Byte(), Faker.Random.Byte(), Faker.Random.Byte());
        var color = ColorTranslator.FromHtml(bgColor);
        var textColor = (color.R * 0.2126 + color.G * 0.7152 + color.B * 0.0722 < 255 / 2) ? "#FFFFFF" : "#000000";
        var name = $"Cor {Faker.Random.String2(Faker.Random.Int(1,5))}";
        var sku = name.GetSku();
        return new ProductColor(name, sku, bgColor, textColor);
    }

    public ProductColor Invalid()
    {
        return new ProductColor(String.Empty, String.Empty);
    }

    public void Dispose()
    {
    }
}