using AutoMapper;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.OrderCommands;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public DeleteOrderCommandHandler(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<bool> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
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

            var entity = await _orderRepository.GetByIdAsync(command.Id);

            if (entity == null)
            {
                return true;
            }

            if (!command.AggregateId.HasValue)
                await _orderRepository.UnitOfWork.BeginTransaction();

            await _orderRepository.DeleteAsync(command.Id);

            foreach (var orderItem in await _orderItemRepository.FindAsync(e => e.IdOrder == command.Id) ?? new List<OrderItem>())
            {
                await _orderItemRepository.DeleteAsync(orderItem.Id);
            }

            if (!command.AggregateId.HasValue)
                await _orderRepository.UnitOfWork.Commit();

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