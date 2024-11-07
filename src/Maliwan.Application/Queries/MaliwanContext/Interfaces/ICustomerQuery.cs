using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.Queries.MaliwanContext.Interfaces;

public interface ICustomerQuery
{
    Task<CustomerModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerModel>?> GetAllAsync();
    Task<PagedResponse<CustomerModel>?> SearchAsync(CustomerSearchRequest request);
}