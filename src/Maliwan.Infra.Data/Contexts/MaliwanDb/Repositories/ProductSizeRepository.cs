using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductSizeRepository : MaliwanRepositoryIntId<ProductSize>, IProductSizeRepository
{
    public ProductSizeRepository(MaliwanDbContext context) : base(context)
    {
    }
}