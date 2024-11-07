using Maliwan.Application.Commands.MaliwanContext.CustomerCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Responses;
using Maliwan.Service.Api.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Maliwan.Service.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class CustomerController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ICustomerQuery _customerQuery;

        public CustomerController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, ICustomerQuery customerQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _customerQuery = customerQuery;
        }

        /// <summary>
        /// Get Customer by Id
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            return Response(await _customerQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all Customers
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<CustomerModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _customerQuery.GetAllAsync());
        }

        /// <summary>
        /// Search Customers
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<CustomerModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] CustomerSearchRequest request)
        {
            return Response(await _customerQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Customer
        /// </summary>
        [HttpPut]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateCustomerCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Customer
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteCustomerCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
