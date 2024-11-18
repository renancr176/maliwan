using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IOrderQuery
{
    Task<OrderModel?> GetByIdAsync(int id);
    Task<PagedResponse<OrderModel>?> SearchAsync(OrderSearchRequest request);
}