using Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;
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
    public class ProductSizeController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IProductSizeQuery _productSizeQuery;

        public ProductSizeController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IProductSizeQuery productSizeQuery) : base(notifications,
            mediator, httpContextAccessor)
        {
            _mediator = mediator;
            _productSizeQuery = productSizeQuery;
        }

        /// <summary>
        /// Get ProductSize by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductSizeModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _productSizeQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all ProductSizes
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<ProductSizeModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _productSizeQuery.GetAllAsync());
        }

        /// <summary>
        /// Search ProductSizes
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<ProductSizeModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] ProductSizeSearchRequest request)
        {
            return Response(await _productSizeQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new ProductSize
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductSizeModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductSizeCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update ProductSize
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductSizeModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProductSizeCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete ProductSize
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteProductSizeCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
