using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IOrderPaymentQuery
{
    Task<OrderPaymentModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderPaymentModel>?> GetByIdOrderAsync(int idOrder);
    Task<PagedResponse<OrderPaymentModel>?> SearchAsync(OrderPaymentSearchRequest request);
}