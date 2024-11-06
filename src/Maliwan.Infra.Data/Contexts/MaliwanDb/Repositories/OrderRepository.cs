using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderRepository : MaliwanRepositoryIntId<Order>, IOrderRepository
{
    public OrderRepository(MaliwanDbContext context) : base(context)
    {
    }
}