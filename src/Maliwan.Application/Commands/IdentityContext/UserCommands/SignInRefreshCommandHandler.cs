using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignInRefreshCommandHandler : IRequestHandler<SignInRefreshCommand, SignInResponseModel?>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly IUserService _userService;

    public SignInRefreshCommandHandler(IMediator mediator, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, IUserService userService)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userService = userService;
    }

    public async Task<SignInResponseModel?> Handle(SignInRefreshCommand command, CancellationToken cancellationToken)
    {
        try
        {
            return await _userService.RefreshTokenAsync(command.AccessToken, command.RefreshToken);
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        return default;
    }
}