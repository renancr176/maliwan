using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class CustomerRepository : MaliwanRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(MaliwanDbContext context) : base(context)
    {
    }
}