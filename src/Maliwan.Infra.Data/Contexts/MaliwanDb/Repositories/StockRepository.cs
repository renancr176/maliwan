using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;

public class StockRepository : MaliwanRepository<Stock>, IStockRepository
{
    public StockRepository(MaliwanDbContext context) : base(context)
    {
    }
}