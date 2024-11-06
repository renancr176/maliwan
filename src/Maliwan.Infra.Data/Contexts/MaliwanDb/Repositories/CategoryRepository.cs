using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class CategoryRepository : MaliwanRepositoryIntId<Category>, ICategoryRepository
{
    public CategoryRepository(MaliwanDbContext context) : base(context)
    {
    }
}