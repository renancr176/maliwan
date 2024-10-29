using AutoMapper;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using Maliwan.Domain.IdentityContext.Interfaces.Validators;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, UserModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;
    private readonly IUserValidator _userValidator;
    private readonly IStringLocalizer<SignUpCommandHandler> _localizer;

    public SignUpCommandHandler(IMediator mediator, IMapper mapper, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager, IUserService userService, IUserValidator userValidator,
        IStringLocalizer<SignUpCommandHandler> localizer)
    {
        _mediator = mediator;
        _mapper = mapper;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _userService = userService;
        _userValidator = userValidator;
        _localizer = localizer;
    }

    public const string PasswordRole = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%¨&*_+-=^~?<>]).{8,50}$";

    public async Task<UserModel?> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _mapper.Map<User>(request);

            if (!await _userValidator.IsValidAsync(user)
                || string.IsNullOrEmpty(request.Password)
                || !Regex.IsMatch(request.Password, PasswordRole))
            {
                foreach (var error in _userValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }

                if (string.IsNullOrEmpty(request.Password)
                || !Regex.IsMatch(request.Password, PasswordRole))
                {
                    await _mediator.Publish(new DomainNotification(
                        nameof(CommonMessages.InvalidPassword),
                        _localizer.GetString(nameof(CommonMessages.InvalidPassword))));
                }

                return default;
            }

            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    await _mediator.Publish(new DomainNotification(identityError.Code, identityError.Description));
                }
                return default;
            }

            await _userService.SendEmailConfirmationAsync(user.Id);

            var userModel = _mapper.Map<UserModel>(user);

            return userModel;
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