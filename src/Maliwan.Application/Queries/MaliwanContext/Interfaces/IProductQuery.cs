using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IProductQuery
{
    Task<ProductModel?> GetByIdAsync(Guid id);
    Task<PagedResponse<ProductModel>?> SearchAsync(ProductSearchRequest request);
}