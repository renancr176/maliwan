using Bogus;

namespace Maliwan.Test.Fixtures;

[CollectionDefinition(nameof(EntityColletion))]
public class EntityColletion : ICollectionFixture<EntityFixture>
{ }

public class EntityFixture : IDisposable
{
    public Faker Faker => new Faker("pt_BR");
    public BrandFixture BrandFixture { get; set; }
    public CategoryFixture CategoryFixture { get; set; }
    public CustomerFixture CustomerFixture { get; set; }
    public GenderFixture GenderFixture { get; set; }
    public PaymentMethodFixture PaymentMethodFixture { get; set; }
    public ProductFixture ProductFixture { get; set; }
    public ProductColorFixture ProductColorFixture { get; set; }
    public ProductSizeFixture ProductSizeFixture { get; set; }
    public SubcategoryFixture SubcategoryFixture { get; set; }

    public EntityFixture()
    {
        BrandFixture = new BrandFixture();
        CategoryFixture = new CategoryFixture();
        CustomerFixture = new CustomerFixture();
        GenderFixture = new GenderFixture();
        PaymentMethodFixture = new PaymentMethodFixture();
        ProductFixture = new ProductFixture();
        ProductColorFixture = new ProductColorFixture();
        ProductSizeFixture = new ProductSizeFixture();
        SubcategoryFixture = new SubcategoryFixture();
    }

    public void Dispose()
    {
        BrandFixture.Dispose();
        CategoryFixture.Dispose();
        CustomerFixture.Dispose();
        GenderFixture.Dispose();
        PaymentMethodFixture.Dispose();
        ProductFixture.Dispose();
        ProductColorFixture.Dispose();
        ProductSizeFixture.Dispose();
        SubcategoryFixture.Dispose();
    }
}