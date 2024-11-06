using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class BrandRepository : MaliwanRepositoryIntId<Brand>, IBrandRepository
{
    public BrandRepository(MaliwanDbContext context) : base(context)
    {
    }
}