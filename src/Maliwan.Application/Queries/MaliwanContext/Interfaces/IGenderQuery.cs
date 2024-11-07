using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IGenderQuery
{
    Task<GenderModel?> GetByIdAsync(int id);
    Task<IEnumerable<GenderModel>?> GetAllAsync();
    Task<PagedResponse<GenderModel>?> SearchAsync(GenderSearchRequest request);
}