using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class CategoryRepository : MaliwanRepositoryIntId<Category>, ICategoryRepository
{
    public CategoryRepository(MaliwanDbContext context) : base(context)
    {
    }
}