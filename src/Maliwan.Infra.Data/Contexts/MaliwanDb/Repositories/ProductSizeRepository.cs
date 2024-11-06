using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductSizeRepository : MaliwanRepositoryIntId<ProductSize>, IProductSizeRepository
{
    public ProductSizeRepository(MaliwanDbContext context) : base(context)
    {
    }
}