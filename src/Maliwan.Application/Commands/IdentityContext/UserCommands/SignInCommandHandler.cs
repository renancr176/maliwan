using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponseModel?>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserService _userService;

    public SignInCommandHandler(IMediator mediator, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, UserManager<User> userManager, SignInManager<User> signInManager,
        IUserService userService)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    public async Task<SignInResponseModel?> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    await _mediator.Publish(new DomainNotification(
                        nameof(CommonMessages.LoginBlocked),
                        _localizer.GetString(nameof(CommonMessages.LoginBlocked))));
                }
                else
                {
                    await _mediator.Publish(new DomainNotification(
                        nameof(CommonMessages.InvalidUseramePassword),
                        _localizer.GetString(nameof(CommonMessages.InvalidUseramePassword))));
                }

                return default;
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (!user.EmailConfirmed)
            {
                await _userService.SendEmailConfirmationAsync(user.Id);
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.InvalidUseramePassword),
                    _localizer.GetString(nameof(CommonMessages.InvalidUseramePassword))));
                return default!;
            }

            return await _userService.GetJwtAsync(user);
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return default;
    }
}