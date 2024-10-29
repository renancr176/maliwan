using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Core.Messages;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, string>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly IDistributedCache _memoryCache;
    //private readonly IMailService _mailService;

    public PasswordResetCommandHandler(IMediator mediator, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, UserManager<User> userManager, IDistributedCache memoryCache)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _memoryCache = memoryCache;
    }

    public async Task<string> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.UserNotFound),
                    _localizer.GetString(nameof(CommonMessages.UserNotFound))));
                return default!;
            }

            user.ResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.SetEmailConfirmationToken();

            await _userManager.UpdateAsync(user);

            await _memoryCache.SetObjectAsync(user.EmailConfirmationToken,
                user,
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                },
                cancellationToken);

            var body = _localizer.GetString(nameof(CommonMessages.EmailPasswordResetBody))
                .ToString()
                .Replace("#Name", user.Name)
                .Replace("#Token", user.EmailConfirmationToken);

            //if (!await _mailService.SendAsync(new SendMailResquest(
            //user.Email,
            //_localizer.GetString(nameof(CommonMessages.EmailPasswordResetSubject)),
            //body)))
            //{
            //    await _mediator.Publish(new DomainNotification(
            //        nameof(CommonMessages.UnableSendMail),
            //        _localizer.GetString(nameof(CommonMessages.UnableSendMail))));
            //    return default!;
            //}

            return _localizer.GetString(nameof(CommonMessages.PasswordResetEmailSent));
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return default!;
    }
}