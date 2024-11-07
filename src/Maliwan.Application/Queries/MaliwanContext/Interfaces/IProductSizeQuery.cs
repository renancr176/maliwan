using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IProductSizeQuery
{
    Task<ProductSizeModel?> GetByIdAsync(int id);
    Task<IEnumerable<ProductSizeModel>?> GetAllAsync();
    Task<PagedResponse<ProductSizeModel>?> SearchAsync(ProductSizeSearchRequest request);
}