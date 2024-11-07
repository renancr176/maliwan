using Bogus;

namespace Maliwan.Test.Fixtures;

[CollectionDefinition(nameof(EntityColletion))]
public class EntityColletion : ICollectionFixture<EntityFixture>
{ }

public class EntityFixture : IDisposable
{
    public Faker Faker { get; private set; }
    public BrandFixture BrandFixture { get; set; }
    public CategoryFixture CategoryFixture { get; set; }
    public GenderFixture GenderFixture { get; set; }
    public PaymentMethodFixture PaymentMethodFixture { get; set; }
    public SubcategoryFixture SubcategoryFixture { get; set; }

    public EntityFixture()
    {
        Faker = new Faker("pt_BR");
        BrandFixture = new BrandFixture();
        CategoryFixture = new CategoryFixture();
        GenderFixture = new GenderFixture();
        PaymentMethodFixture = new PaymentMethodFixture();
        SubcategoryFixture = new SubcategoryFixture();
    }

    public void Dispose()
    {
        BrandFixture.Dispose();
        CategoryFixture.Dispose();
        GenderFixture.Dispose();
        PaymentMethodFixture.Dispose();
        SubcategoryFixture.Dispose();
    }
}