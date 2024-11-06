using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderPaymentRepository : MaliwanRepository<OrderPayment>, IOrderPaymentRepository
{
    public OrderPaymentRepository(MaliwanDbContext context) : base(context)
    {
    }
}