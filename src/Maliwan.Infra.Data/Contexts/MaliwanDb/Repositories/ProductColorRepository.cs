using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductColorRepository : MaliwanRepositoryIntId<ProductColor>, IProductColorRepository
{
    public ProductColorRepository(MaliwanDbContext context) : base(context)
    {
    }
}