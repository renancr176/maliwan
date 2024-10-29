using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Service.Api.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Maliwan.Service.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IMediator _mediator;

    public UserController(INotificationHandler<DomainNotification> notifications, IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
        : base(notifications, mediator,
        httpContextAccessor)
    {
        _mediator = mediator;
    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<SignInResponseModel?>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignInAsync([FromBody] SignInCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("SignIn/Refresh")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<SignInResponseModel?>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignInRefreshAsync([FromBody] SignInRefreshCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("SignUp")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<UserModel>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("ConfirmEmail")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("PasswordReset")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<string>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> PasswordResetAsync([FromBody] PasswordResetCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<string>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }

    [HttpPost("IncludeRole")]
    [SwaggerResponse(200, Type = typeof(BaseResponse<UserModel>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
#if !DEBUG
    [Authorize("Bearer", Roles = $"{nameof(RoleEnum.Admin)}")]
    [ApiExplorerSettings(IgnoreApi = true)]
#else
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = false)]
#endif
    public async Task<IActionResult> IncludeRoleAsync([FromBody] UserAddRoleCommand request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        return Response(await _mediator.Send(request));
    }
}
