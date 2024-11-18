using Maliwan.Application.Commands.MaliwanContext.OrderCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Responses;
using Maliwan.Service.Api.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Maliwan.Service.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class OrderController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IOrderQuery _stockQuery;

        public OrderController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IOrderQuery stockQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _stockQuery = stockQuery;
        }

        /// <summary>
        /// Get Order by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _stockQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Search Orders
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<OrderModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] OrderSearchRequest request)
        {
            return Response(await _stockQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Order
        /// </summary>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateOrderCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Order
        /// </summary>
        [HttpDelete("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteOrderCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
