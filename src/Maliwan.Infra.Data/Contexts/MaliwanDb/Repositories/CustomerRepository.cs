using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class CustomerRepository : MaliwanRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(MaliwanDbContext context) : base(context)
    {
    }
}