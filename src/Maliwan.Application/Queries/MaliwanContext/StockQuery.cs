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
using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Queries.MaliwanContext;

public class StockQuery : IStockQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IStockRepository _stockRepository;

    public StockQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IStockRepository stockRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _stockRepository = stockRepository;
    }

    private IEnumerable<string> Includes = new []
    {
        $"{nameof(Stock.Product)}.{nameof(Product.Brand)}",
        $"{nameof(Stock.Product)}.{nameof(Product.Subcategory)}.{nameof(Subcategory.Category)}",
        $"{nameof(Stock.Product)}.{nameof(Product.Gender)}",
        nameof(Stock.Size),
        nameof(Stock.Color),
        nameof(Stock.OrderItems),
    };

    public async Task<StockModel?> GetByIdAsync(Guid id)
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

            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);
            return _mapper.Map<StockModel>(await _stockRepository.FirstOrDefaultAsync(e => e.Id == id && (isAdmin || e.Active), Includes));
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

    public async Task<PagedResponse<StockModel>?> SearchAsync(StockSearchRequest request)
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

            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            #region Be careful when changing the permissions check

            Expression<Func<Stock, bool>> permitions = e => (isAdmin || e.Active);

            #endregion

            Expression<Func<Stock, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((request.IdProduct.HasValue && e.IdProduct == request.IdProduct)
                         || (request.IdSize.HasValue && e.IdSize == request.IdSize)
                         || (request.IdColor.HasValue && e.IdColor == request.IdColor)
                         || (request.InputDate.HasValue && e.InputDate == request.InputDate)
                         || (request.CurrentQuantityMin.HasValue &&
                             (e.InputQuantity - e.OrderItems
                                 .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                 .Sum(x => x.Quantity)) >= request.CurrentQuantityMin)
                         || (request.CurrentQuantityMax.HasValue &&
                             (e.InputQuantity - e.OrderItems
                                 .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                 .Sum(x => x.Quantity)) <= request.CurrentQuantityMax)
                         || (request.StockLevel.HasValue
                            && (request.StockLevel != StockLevelEnum.High ||
                                ((e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) >= 3 &&
                                 (e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) >= (2 * (e.InputQuantity / 3))))
                            && (request.StockLevel != StockLevelEnum.Medium ||
                                ((e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) >= 3 &&
                                 (e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) > (e.InputQuantity / 3) &&
                                 (e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) < (2 * (e.InputQuantity / 3))))
                            && (request.StockLevel != StockLevelEnum.Low ||
                                (((e.InputQuantity - e.OrderItems
                                      .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                      .Sum(x => x.Quantity)) < 3 ||
                                  (e.InputQuantity - e.OrderItems
                                      .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                      .Sum(x => x.Quantity)) <= (e.InputQuantity / 3))))
                            )
                         || (request.Active.HasValue && e.Active == request.Active)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (!request.IdProduct.HasValue || e.IdProduct == request.IdProduct)
                        && (!request.IdSize.HasValue || e.IdSize == request.IdSize)
                        && (!request.IdColor.HasValue || e.IdColor == request.IdColor)
                        && (!request.InputDate.HasValue || e.InputDate == request.InputDate)
                        && (!request.CurrentQuantityMin.HasValue ||
                            (e.InputQuantity - e.OrderItems
                                .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                .Sum(x => x.Quantity)) >= request.CurrentQuantityMin)
                        && (!request.CurrentQuantityMax.HasValue ||
                            (e.InputQuantity - e.OrderItems
                                .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                .Sum(x => x.Quantity)) <= request.CurrentQuantityMax)
                        && (!request.StockLevel.HasValue
                            || (request.StockLevel == StockLevelEnum.High &&
                                (e.InputQuantity - e.OrderItems
                                    .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                    .Sum(x => x.Quantity)) >= 3 &&
                                (e.InputQuantity - e.OrderItems
                                    .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                    .Sum(x => x.Quantity)) >= (2 * (e.InputQuantity / 3)))
                            || (request.StockLevel == StockLevelEnum.Medium &&
                                (e.InputQuantity - e.OrderItems
                                    .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                    .Sum(x => x.Quantity)) >= 3 &&
                                (e.InputQuantity - e.OrderItems
                                    .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                    .Sum(x => x.Quantity)) > (e.InputQuantity / 3) &&
                                (e.InputQuantity - e.OrderItems
                                    .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                    .Sum(x => x.Quantity)) < (2 * (e.InputQuantity / 3)))
                            || (request.StockLevel == StockLevelEnum.Low &&
                                ((e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) < 3 ||
                                 (e.InputQuantity - e.OrderItems
                                     .Where(x => !x.DeletedAt.HasValue && !x.Order.DeletedAt.HasValue)
                                     .Sum(x => x.Quantity)) <= (e.InputQuantity / 3)))
                            )
                        && (!request.Active.HasValue || e.Active == request.Active);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<StockModel>>(
                await _stockRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<Stock, object>>, OrderByEnum>()
                    {
                        {e => e.InputDate, OrderByEnum.Descending}
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