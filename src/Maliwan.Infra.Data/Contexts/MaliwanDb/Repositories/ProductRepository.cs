using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class ProductRepository : MaliwanRepository<Product>, IProductRepository
{
    public ProductRepository(MaliwanDbContext context) : base(context)
    {
    }
}