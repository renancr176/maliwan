using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class StockRepository : MaliwanRepository<Stock>, IStockRepository
{
    public StockRepository(MaliwanDbContext context) : base(context)
    {
    }
}