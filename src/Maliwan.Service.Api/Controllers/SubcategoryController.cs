using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;
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
    public class SubcategoryController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ISubcategoryQuery _subcategoryQuery;

        public SubcategoryController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, ISubcategoryQuery subcategoryQuery) : base(notifications,
            mediator, httpContextAccessor)
        {
            _mediator = mediator;
            _subcategoryQuery = subcategoryQuery;
        }


        /// <summary>
        /// Get Subcategory by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<SubcategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _subcategoryQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all Subcategorys
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<SubcategoryModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _subcategoryQuery.GetAllAsync());
        }

        /// <summary>
        /// Search Subcategorys
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<SubcategoryModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] SubcategorySearchRequest request)
        {
            return Response(await _subcategoryQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Subcategory
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<SubcategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSubcategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Subcategory
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<SubcategoryModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateSubcategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Subcategory
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteSubcategoryCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
