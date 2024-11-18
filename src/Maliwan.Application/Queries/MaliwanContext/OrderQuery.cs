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

public class OrderQuery : IOrderQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IOrderRepository _orderRepository;

    public OrderQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IOrderRepository orderRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _orderRepository = orderRepository;
    }

    private IEnumerable<string> Includes = new []
    {
        nameof(Order.Customer),
        nameof(Order.OrderPayments),
        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Stock)}.{nameof(Stock.Product)}.{nameof(Product.Brand)}",
        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Stock)}.{nameof(Stock.Product)}.{nameof(Product.Gender)}",
        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Stock)}.{nameof(Stock.Product)}.{nameof(Product.Subcategory)}.{nameof(Subcategory.Category)}",
        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Stock)}.{nameof(Stock.Size)}",
        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Stock)}.{nameof(Stock.Color)}",
    };

    public async Task<OrderModel?> GetByIdAsync(int id)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return default;
            return _mapper.Map<OrderModel>(await _orderRepository.FirstOrDefaultAsync(e => e.Id == id , Includes));
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

    public async Task<PagedResponse<OrderModel>?> SearchAsync(OrderSearchRequest request)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            if (!isAdmin)
                return new PagedResponse<OrderModel>(request.PageIndex, request.PageSize, 0, 0, new List<OrderModel>());

            #region Be careful when changing the permissions check

            Expression<Func<Order, bool>> permitions = e => isAdmin;

            #endregion

            Expression<Func<Order, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((request.IdCustomer.HasValue && e.IdCustomer == request.IdCustomer)
                        || (request.SellDate.HasValue && e.SellDate.Date == request.SellDate.Value.Date)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (!request.IdCustomer.HasValue || e.IdCustomer == request.IdCustomer)
                        && (!request.SellDate.HasValue || e.SellDate.Date == request.SellDate.Value.Date);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<OrderModel>>(
                await _orderRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<Order, object>>, OrderByEnum>()
                    {
                        {e => e.SellDate, OrderByEnum.Descending}
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