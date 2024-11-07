using Maliwan.Application.Commands.MaliwanContext.GenderCommands;
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
    public class GenderController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IGenderQuery _genderQuery;

        public GenderController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IGenderQuery genderQuery) : base(notifications, mediator,
            httpContextAccessor)
        {
            _mediator = mediator;
            _genderQuery = genderQuery;
        }

        /// <summary>
        /// Get Gender by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<GenderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _genderQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all Genders
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<GenderModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _genderQuery.GetAllAsync());
        }

        /// <summary>
        /// Search Genders
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<GenderModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] GenderSearchRequest request)
        {
            return Response(await _genderQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new Gender
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<GenderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGenderCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update Gender
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<GenderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateGenderCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Gender
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeleteGenderCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
