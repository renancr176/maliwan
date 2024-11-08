using Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;
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
    public class ProductColorController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IProductColorQuery _productColorQuery;

        public ProductColorController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IProductColorQuery productColorQuery) : base(notifications,
            mediator, httpContextAccessor)
        {
            _mediator = mediator;
            _productColorQuery = productColorQuery;
        }

        /// <summary>
        /// Get ProductColor by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductColorModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _productColorQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all ProductColors
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<ProductColorModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _productColorQuery.GetAllAsync());
        }

        /// <summary>
        /// Search ProductColors
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<ProductColorModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] ProductColorSearchRequest request)
        {
            return Response(await _productColorQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new ProductColor
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductColorModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductColorCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update ProductColor
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductColorModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProductColorCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete ProductColor
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteProductColorCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
