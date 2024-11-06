using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
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
    public class BrandController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IBrandQuery _brandQuery;

        public BrandController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IBrandQuery brandQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _brandQuery = brandQuery;
        }

        /// <summary>
        /// Get Brand by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<BrandModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _brandQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all Brands
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<BrandModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _brandQuery.GetAllAsync());
        }

        /// <summary>
        /// Search Brands
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<BrandModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] BrandSearchRequest request)
        {
            return Response(await _brandQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Brand
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<BrandModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBrandCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Brand
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<BrandModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateBrandCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Brand
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteBrandCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
