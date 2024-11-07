using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;

public class DeleteProductSizeCommandHandler : IRequestHandler<DeleteProductSizeCommand, bool>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IProductSizeRepository _productSizeRepository;

    public DeleteProductSizeCommandHandler(IMediator mediator, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IProductSizeRepository productSizeRepository)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _productSizeRepository = productSizeRepository;
    }

    public async Task<bool> Handle(DeleteProductSizeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.Unauthorized),
                    _commonMessagesLocalizer.GetString(nameof(CommonMessages.Unauthorized))));
                return default;
            }

            var entity = await _productSizeRepository.GetByIdAsync(command.Id);

            if (entity == null)
            {
                return true;
            }

            if (!command.AggregateId.HasValue)
                await _productSizeRepository.UnitOfWork.BeginTransaction();

            await _productSizeRepository.DeleteAsync(command.Id);

            if (!command.AggregateId.HasValue)
                await _productSizeRepository.UnitOfWork.Commit();

            return true;
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _commonMessagesLocalizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return false;
    }
}