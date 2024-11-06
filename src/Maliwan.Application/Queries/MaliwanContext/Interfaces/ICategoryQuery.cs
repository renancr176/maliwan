using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface ICategoryQuery
{
    Task<CategoryModel?> GetByIdAsync(int id);
    Task<IEnumerable<CategoryModel>?> GetAllAsync();
    Task<PagedResponse<CategoryModel>?> SearchAsync(CategorySearchRequest request);
}