using AutoMapper;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.IdentityContext.Interfaces.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly IDistributedCache _memoryCache;
    private readonly IUserValidator _userValidator;

    public ResetPasswordCommandHandler(IMediator mediator, IMapper mapper, /*ILog log,*/
        IHttpContextAccessor httpContextAccessor, IStringLocalizer<CommonMessages> localizer,
        UserManager<User> userManager, IDistributedCache memoryCache, IUserValidator userValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _memoryCache = memoryCache;
        _userValidator = userValidator;
    }

    #region Consts



    #endregion

    public async Task<string?> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_memoryCache.TryGetValue(request.Token,
                    out User user)
                || !await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider,
                    "ResetPassword", user.ResetPasswordToken)
                || user.UserName != request.UserName)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.InvalidToken),
                    _localizer.GetString(nameof(CommonMessages.InvalidToken))));
                return default;
            }

            user = await _userManager.FindByIdAsync(user.Id.ToString());

            user.RememberPhrase = request.RememberPhrase;

            if (!await _userValidator.IsValidAsync(user))
            {
                foreach (var error in _userValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
            }

            var result = await _userManager.ResetPasswordAsync(user, user.ResetPasswordToken, request.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    await _mediator.Publish(new DomainNotification(identityError.Code, identityError.Description));
                }
                return default;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
            }

            user.EmailConfirmationToken = null;
            user.ResetPasswordToken = null;
            await _userManager.UpdateAsync(user);

            _memoryCache.Remove(request.Token);

            return "Password reseted successfuly.";
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