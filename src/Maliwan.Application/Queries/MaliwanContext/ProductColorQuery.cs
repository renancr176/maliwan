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

public class ProductColorQuery : IProductColorQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IProductColorRepository _productColorRepository;

    public ProductColorQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IProductColorRepository productColorRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _productColorRepository = productColorRepository;
    }

    private IEnumerable<string> Includes = null;

    public async Task<ProductColorModel?> GetByIdAsync(int id)
    {
        try
        {
            return _mapper.Map<ProductColorModel>(await _productColorRepository.FirstOrDefaultAsync(e => e.Id == id, Includes));
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

    public async Task<IEnumerable<ProductColorModel>?> GetAllAsync()
    {
        try
        {
            return _mapper.Map<IEnumerable<ProductColorModel>>(await _productColorRepository.GetAllAsync(Includes));
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

    public async Task<PagedResponse<ProductColorModel>?> SearchAsync(ProductColorSearchRequest request)
    {
        try
        {
            #region Be careful when changing the permissions check

            Expression<Func<ProductColor, bool>> permitions = e => true;

            #endregion

            Expression<Func<ProductColor, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((!string.IsNullOrEmpty(request.Name) && e.Name.Contains(request.Name)));
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (string.IsNullOrEmpty(request.Name) || e.Name.Contains(request.Name));
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<ProductColorModel>>(
                await _productColorRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<ProductColor, object>>, OrderByEnum>()
                    {
                        {e => e.Name, OrderByEnum.Ascending}
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