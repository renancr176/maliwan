using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderItemRepository : MaliwanRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(MaliwanDbContext context) : base(context)
    {
    }
}