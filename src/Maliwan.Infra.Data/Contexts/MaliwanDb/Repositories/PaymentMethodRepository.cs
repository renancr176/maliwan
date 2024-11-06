using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class PaymentMethodRepository : MaliwanRepositoryIntId<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(MaliwanDbContext context) : base(context)
    {
    }
}