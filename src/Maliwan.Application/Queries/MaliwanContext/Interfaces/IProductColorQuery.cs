using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IProductColorQuery
{
    Task<ProductColorModel?> GetByIdAsync(int id);
    Task<IEnumerable<ProductColorModel>?> GetAllAsync();
    Task<PagedResponse<ProductColorModel>?> SearchAsync(ProductColorSearchRequest request);
}