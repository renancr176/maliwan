﻿using System.Linq.Expressions;
using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Responses;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Queries.MaliwanContext;

public class BrandQuery : IBrandQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IBrandRepository _brandRepository;

    public BrandQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IBrandRepository brandRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _brandRepository = brandRepository;
    }

    private IEnumerable<string> Includes = null;

    public async Task<BrandModel?> GetByIdAsync(int id)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);
            return _mapper.Map<BrandModel>(await _brandRepository.FirstOrDefaultAsync(e => e.Id == id && (isAdmin || e.Active), Includes));
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

    public async Task<IEnumerable<BrandModel>?> GetAllAsync()
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);
            return _mapper.Map<IEnumerable<BrandModel>>(await _brandRepository.FindAsync(e => isAdmin || e.Active, Includes));
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

    public async Task<PagedResponse<BrandModel>?> SearchAsync(BrandSearchRequest request)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            #region Be careful when changing the permissions check

            Expression<Func<Brand, bool>> permitions = e => isAdmin || e.Active;

            #endregion

            Expression<Func<Brand, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((!string.IsNullOrEmpty(request.Name) && e.Name.Contains(request.Name))
                         || (request.Active.HasValue && e.Active == request.Active)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (string.IsNullOrEmpty(request.Name) || e.Name.Contains(request.Name))
                        && (!request.Active.HasValue || e.Active == request.Active);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<BrandModel>>(
                await _brandRepository.GetPagedAsync(
                    request, 
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<Brand, object>>, OrderByEnum>()
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