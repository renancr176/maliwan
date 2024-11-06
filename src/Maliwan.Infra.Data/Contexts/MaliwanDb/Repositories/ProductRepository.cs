using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductRepository : MaliwanRepository<Product>, IProductRepository
{
    public ProductRepository(MaliwanDbContext context) : base(context)
    {
    }
}