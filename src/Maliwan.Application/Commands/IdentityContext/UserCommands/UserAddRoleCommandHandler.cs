using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class UserAddRoleCommandHandler : IRequestHandler<UserAddRoleCommand, bool>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserAddRoleCommandHandler(IMediator mediator, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> Handle(UserAddRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.UserNotFound),
                    _localizer.GetString(nameof(CommonMessages.UserNotFound))));
                return false;
            }

            foreach (var role in request.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _mediator.Publish(new DomainNotification("RoleNotFound", @$"Role ""{role}"" not found."));
                    return false;
                }
            }

            foreach (var role in request.Roles)
            {
                await _userManager.AddToRoleAsync(user, role.ToString());
            }

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