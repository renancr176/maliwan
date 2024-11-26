using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.IdentityContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.IdentityContext.UserCommands;

public class DeleteExpiredRefreshTokensCommandHandler : IRequestHandler<DeleteExpiredRefreshTokensCommand, bool>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public DeleteExpiredRefreshTokensCommandHandler(IMediator mediator, IHttpContextAccessor httpContextAccessor, 
        IStringLocalizer<CommonMessages> localizer, IRefreshTokenRepository refreshTokenRepository)
    {
        _mediator = mediator;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<bool> Handle(DeleteExpiredRefreshTokensCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _refreshTokenRepository.DeleteAsync(e => e.ValidUntil < DateTime.UtcNow);
            await _refreshTokenRepository.SaveChangesAsync();
            return true;
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
