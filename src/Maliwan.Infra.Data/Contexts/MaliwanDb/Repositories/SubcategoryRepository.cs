using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class SubcategoryRepository : MaliwanRepositoryIntId<Subcategory>, ISubcategoryRepository
{
    public SubcategoryRepository(MaliwanDbContext context) : base(context)
    {
    }
}