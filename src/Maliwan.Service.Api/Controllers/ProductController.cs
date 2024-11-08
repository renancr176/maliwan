using Maliwan.Application.Commands.MaliwanContext.ProductCommands;
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
    public class ProductController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IProductQuery _productQuery;

        public ProductController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IProductQuery productQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _productQuery = productQuery;
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            return Response(await _productQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Search Products
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<ProductModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] ProductSearchRequest request)
        {
            return Response(await _productQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Product
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Product
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProductCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteProductCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
