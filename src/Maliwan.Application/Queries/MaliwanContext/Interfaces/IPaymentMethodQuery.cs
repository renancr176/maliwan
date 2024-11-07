using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface IPaymentMethodQuery
{
    Task<PaymentMethodModel?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentMethodModel>?> GetAllAsync();
    Task<PagedResponse<PaymentMethodModel>?> SearchAsync(PaymentMethodSearchRequest request);
}