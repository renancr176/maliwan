using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class BrandRepository : MaliwanRepositoryIntId<Brand>, IBrandRepository
{
    public BrandRepository(MaliwanDbContext context) : base(context)
    {
    }
}