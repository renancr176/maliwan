using Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;
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
    public class OrderPaymentController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IOrderPaymentQuery _orderPaymentQuery;

        public OrderPaymentController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IOrderPaymentQuery orderPaymentQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _orderPaymentQuery = orderPaymentQuery;
        }

        /// <summary>
        /// Get OrderPayment by Id
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderPaymentModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            return Response(await _orderPaymentQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get OrderPayment by Order Id
        /// </summary>
        [HttpGet("Order/{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<OrderPaymentModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdOrderAsync([FromRoute] int id)
        {
            return Response(await _orderPaymentQuery.GetByIdOrderAsync(id));
        }

        /// <summary>
        /// Search OrderPayments
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<OrderPaymentModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] OrderPaymentSearchRequest request)
        {
            return Response(await _orderPaymentQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new OrderPayment
        /// </summary>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderPaymentModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateOrderPaymentCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update OrderPayment
        /// </summary>
        [HttpPut]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderPaymentModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateOrderPaymentCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete OrderPayment
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteOrderPaymentCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
