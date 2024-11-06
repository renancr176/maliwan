using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class GenderRepository : MaliwanRepositoryIntId<Gender>, IGenderRepository
{
    public GenderRepository(MaliwanDbContext context) : base(context)
    {
    }
}