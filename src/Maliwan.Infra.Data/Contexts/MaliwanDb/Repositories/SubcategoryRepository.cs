using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class SubcategoryRepository : MaliwanRepositoryIntId<Subcategory>, ISubcategoryRepository
{
    public SubcategoryRepository(MaliwanDbContext context) : base(context)
    {
    }
}