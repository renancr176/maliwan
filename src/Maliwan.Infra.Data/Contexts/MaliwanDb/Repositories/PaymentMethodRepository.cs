﻿using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class PaymentMethodRepository : MaliwanRepositoryIntId<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(MaliwanDbContext context) : base(context)
    {
    }
}