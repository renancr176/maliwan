using Maliwan.Application.Commands.MaliwanContext.StockCommands;
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
    public class StockController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IStockQuery _stockQuery;

        public StockController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IStockQuery stockQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _stockQuery = stockQuery;
        }

        /// <summary>
        /// Get Stock by Id
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<StockModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            return Response(await _stockQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Search Stocks
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<StockModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] StockSearchRequest request)
        {
            return Response(await _stockQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Stock
        /// </summary>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<StockModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStockCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Stock
        /// </summary>
        [HttpPut]
        [SwaggerResponse(200, Type = typeof(BaseResponse<StockModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateStockCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Stock
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteStockCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
