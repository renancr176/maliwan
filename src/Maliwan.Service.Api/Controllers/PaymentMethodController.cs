using Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;
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
    public class PaymentMethodController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IPaymentMethodQuery _paymentMethodQuery;

        public PaymentMethodController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IPaymentMethodQuery paymentMethodQuery) : base(notifications,
            mediator, httpContextAccessor)
        {
            _mediator = mediator;
            _paymentMethodQuery = paymentMethodQuery;
        }

        /// <summary>
        /// Get PaymentMethod by Id
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PaymentMethodModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return Response(await _paymentMethodQuery.GetByIdAsync(id));
        }

        /// <summary>
        /// Get all PaymentMethods
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<PaymentMethodModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllAsync()
        {
            return Response(await _paymentMethodQuery.GetAllAsync());
        }

        /// <summary>
        /// Search PaymentMethods
        /// </summary>
        [HttpGet("Search")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PagedResponse<PaymentMethodModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> SearchAsync([FromQuery] PaymentMethodSearchRequest request)
        {
            return Response(await _paymentMethodQuery.SearchAsync(request));
        }

        /// <summary>
        /// Create a new PaymentMethod
        /// </summary>
        [HttpPost]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PaymentMethodModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePaymentMethodCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update PaymentMethod
        /// </summary>
        [HttpPut]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<PaymentMethodModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdatePaymentMethodCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete PaymentMethod
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize("Bearer")]
        [SwaggerResponse(200, Type = typeof(BaseResponse))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteAsync([FromRoute] DeletePaymentMethodCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}
