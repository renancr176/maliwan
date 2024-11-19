using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Responses;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using Maliwan.Domain.Core.Extensions;

namespace Maliwan.Application.Queries.MaliwanContext;

public class OrderPaymentQuery : IOrderPaymentQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IOrderPaymentRepository _orderPaymentRepository;

    public OrderPaymentQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IOrderPaymentRepository orderPaymentRepository, IEnumerable<string> includes)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _orderPaymentRepository = orderPaymentRepository;
    }

    private IEnumerable<string> Includes = null;

    public async Task<OrderPaymentModel?> GetByIdAsync(Guid id)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return default;

            return _mapper.Map<OrderPaymentModel>(await _orderPaymentRepository.FirstOrDefaultAsync(e => e.Id == id, Includes));
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _commonMessagesLocalizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        return default;
    }

    public async Task<IEnumerable<OrderPaymentModel>?> GetByIdOrderAsync(int idOrder)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return default;

            return _mapper.Map<IEnumerable<OrderPaymentModel>>(await _orderPaymentRepository.FindAsync(e => e.IdOrder == idOrder, Includes));
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _commonMessagesLocalizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        return default;
    }

    public async Task<PagedResponse<OrderPaymentModel>?> SearchAsync(OrderPaymentSearchRequest request)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return new PagedResponse<OrderPaymentModel>(request.PageIndex, request.PageSize, 0, 0, new List<OrderPaymentModel>());

            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            #region Be careful when changing the permissions check

            Expression<Func<OrderPayment, bool>> permitions = e => isAdmin;

            #endregion

            Expression<Func<OrderPayment, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((request.IdOrder.HasValue && e.IdOrder == request.IdOrder)
                        || (request.IdPaymentMethod.HasValue && e.IdPaymentMethod == request.IdPaymentMethod)
                        || (request.AmountPaidMin.HasValue && e.AmountPaid >= request.AmountPaidMin)
                        || (request.AmountPaidMax.HasValue && e.AmountPaid <= request.AmountPaidMax)
                        || (request.PaymentDate.HasValue && e.PaymentDate.Date <= request.PaymentDate.Value.Date)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (!request.IdOrder.HasValue || e.IdOrder == request.IdOrder)
                        && (!request.IdPaymentMethod.HasValue || e.IdPaymentMethod == request.IdPaymentMethod)
                        && (!request.AmountPaidMin.HasValue || e.AmountPaid >= request.AmountPaidMin)
                        && (!request.AmountPaidMax.HasValue || e.AmountPaid <= request.AmountPaidMax)
                        && (!request.PaymentDate.HasValue || e.PaymentDate.Date == request.PaymentDate.Value.Date);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<OrderPaymentModel>>(
                await _orderPaymentRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<OrderPayment, object>>, OrderByEnum>()
                    {
                        {e => e.PaymentDate, OrderByEnum.Ascending}
                    }));
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _commonMessagesLocalizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        return default;
    }
}