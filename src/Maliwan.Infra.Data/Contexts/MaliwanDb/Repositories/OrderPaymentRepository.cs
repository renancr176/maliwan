using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderPaymentRepository : MaliwanRepository<OrderPayment>, IOrderPaymentRepository
{
    public OrderPaymentRepository(MaliwanDbContext context) : base(context)
    {
    }
}