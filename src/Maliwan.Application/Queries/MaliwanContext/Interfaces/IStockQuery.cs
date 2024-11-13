using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IStockQuery
{
    Task<StockModel?> GetByIdAsync(Guid id);
    Task<PagedResponse<StockModel>?> SearchAsync(StockSearchRequest request);
}