using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IBrandQuery
{
    Task<BrandModel?> GetByIdAsync(int id);
    Task<IEnumerable<BrandModel>?> GetAllAsync();
    Task<PagedResponse<BrandModel>?> SearchAsync(BrandSearchRequest request);
}