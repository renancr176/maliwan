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

public class CustomerQuery : ICustomerQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly ICustomerRepository _customerRepository;

    public CustomerQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        ICustomerRepository customerRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _customerRepository = customerRepository;
    }

    private IEnumerable<string> Includes = null;

    public async Task<CustomerModel?> GetByIdAsync(Guid id)
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return default;

            return _mapper.Map<CustomerModel>(await _customerRepository.FirstOrDefaultAsync(e => e.Id == id, Includes));
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

    public async Task<IEnumerable<CustomerModel>?> GetAllAsync()
    {
        try
        {
            if (!await _userService.CurrentUserHasRole(RoleEnum.Admin))
                return default;

            return _mapper.Map<IEnumerable<CustomerModel>>(await _customerRepository.GetAllAsync(Includes));
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

    public async Task<PagedResponse<CustomerModel>?> SearchAsync(CustomerSearchRequest request)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            if (!isAdmin)
                return new PagedResponse<CustomerModel>(request.PageIndex, request.PageSize, 0, 0, new List<CustomerModel>());

            #region Be careful when changing the permissions check

            Expression < Func<Customer, bool> > permitions = e => true;

            #endregion

            Expression<Func<Customer, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((!string.IsNullOrEmpty(request.Name) && e.Name.Contains(request.Name))
                         || (!string.IsNullOrEmpty(request.Document) && e.Name.Contains(request.Document))
                         || (request.Type.HasValue && e.Type == request.Type)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (string.IsNullOrEmpty(request.Name) || e.Name.Contains(request.Name))
                        && (!string.IsNullOrEmpty(request.Document) && e.Name.Contains(request.Document))
                        && (request.Type.HasValue && e.Type == request.Type);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<CustomerModel>>(
                await _customerRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<Customer, object>>, OrderByEnum>()
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