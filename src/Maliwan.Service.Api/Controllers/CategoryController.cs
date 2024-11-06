using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
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
    public class CategoryController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ICategoryQuery _brandQuery;

        public CategoryController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, ICategoryQuery brandQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _brandQuery = brandQuery;
        }

        /// <summary>
        /// Get Category by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _brandQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all Categories
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<CategoryModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _brandQuery.GetAllAsync());
        }

        /// <summary>
        /// Search Categories
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<CategoryModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] CategorySearchRequest request)
        {
            return Response(await _brandQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Category
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Category
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateCategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteCategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
