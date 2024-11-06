using Maliwan.Domain.Core.Data;
using Maliwan.Domain.Core.DomainObjects;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb;

public abstract class MaliwanRepository<TEntity> : Repository<MaliwanDbContext, TEntity>
    where TEntity : Entity
{
    protected MaliwanRepository(MaliwanDbContext context)
        : base(context)
    {
    }
}

public abstract class MaliwanRepositoryIntId<TEntity> : RepositoryIntId<MaliwanDbContext, TEntity>
    where TEntity : EntityIntId
{
    protected MaliwanRepositoryIntId(MaliwanDbContext context)
        : base(context)
    {
    }
}