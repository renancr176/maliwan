﻿using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class OrderRepository : MaliwanRepositoryIntId<Order>, IOrderRepository
{
    public OrderRepository(MaliwanDbContext context) : base(context)
    {
    }
}