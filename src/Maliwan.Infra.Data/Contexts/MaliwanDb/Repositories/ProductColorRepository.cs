using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductColorRepository : MaliwanRepositoryIntId<ProductColor>, IProductColorRepository
{
    public ProductColorRepository(MaliwanDbContext context) : base(context)
    {
    }
}