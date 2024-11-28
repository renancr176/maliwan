using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface ISubcategoryQuery
{
    Task<SubcategoryModel?> GetByIdAsync(int id);
    Task<IEnumerable<SubcategoryModel>?> GetAllAsync(int? idCategory = null, bool? active = null);
    Task<PagedResponse<SubcategoryModel>?> SearchAsync(SubcategorySearchRequest request);
}