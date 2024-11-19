using AutoMapper;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;

public class DeleteOrderPaymentCommandHandler : IRequestHandler<DeleteOrderPaymentCommand, bool>
{
    private readonly IMediator _mediator;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IOrderPaymentRepository _orderPaymentRepository;

    public DeleteOrderPaymentCommandHandler(IMediator mediator, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IOrderPaymentRepository orderPaymentRepository)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _orderPaymentRepository = orderPaymentRepository;
    }

    public async Task<bool> Handle(DeleteOrderPaymentCommand command, CancellationToken cancellationToken)
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

            var entity = await _orderPaymentRepository.GetByIdAsync(command.Id);

            if (entity == null)
            {
                return true;
            }

            if (!command.AggregateId.HasValue)
                await _orderPaymentRepository.UnitOfWork.BeginTransaction();

            await _orderPaymentRepository.DeleteAsync(command.Id);

            if (!command.AggregateId.HasValue)
                await _orderPaymentRepository.UnitOfWork.Commit();

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