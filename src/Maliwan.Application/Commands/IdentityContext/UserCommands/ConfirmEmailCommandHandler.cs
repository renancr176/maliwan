using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;

    public ConfirmEmailCommandHandler(IMediator mediator, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, UserManager<User> userManager, IUserService userService)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _userService = userService;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.UserNotFound),
                    _localizer.GetString(nameof(CommonMessages.UserNotFound))));
                return false;
            }

            if (user.EmailConfirmationToken != request.EmailConfirmationKey)
            {
                await _userService.SendEmailConfirmationAsync(user.Id);
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.InvalidConfirmationKey),
                    _localizer.GetString(nameof(CommonMessages.InvalidConfirmationKey))));
                return false;
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            await _userManager.UpdateAsync(user);

            return true;
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return false;
    }
}