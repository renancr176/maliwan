using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class GenderRepository : MaliwanRepositoryIntId<Gender>, IGenderRepository
{
    public GenderRepository(MaliwanDbContext context) : base(context)
    {
    }
}