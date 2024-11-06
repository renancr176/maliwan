using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderItemRepository : MaliwanRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(MaliwanDbContext context) : base(context)
    {
    }
}