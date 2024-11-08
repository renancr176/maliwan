using AutoMapper;
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
using Maliwan.Application.Models.MaliwanContext;

namespace Maliwan.Application.Queries.MaliwanContext;

public class ProductQuery : IProductQuery
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IUserService _userService;
    private readonly IProductRepository _productRepository;

    public ProductQuery(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IUserService userService,
        IProductRepository productRepository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _userService = userService;
        _productRepository = productRepository;
    }

    private IEnumerable<string> Includes = new[]
    {
        nameof(Product.Brand),
        nameof(Product.Gender),
        $"{nameof(Product.Subcategory)}.{nameof(Subcategory.Category)}",
        $"{nameof(Product.Stocks)}.{nameof(Stock.Size)}",
        $"{nameof(Product.Stocks)}.{nameof(Stock.Color)}",
        $"{nameof(Product.Stocks)}.{nameof(Stock.OrderItems)}",
    };

    public async Task<ProductModel?> GetByIdAsync(Guid id)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);
            return _mapper.Map<ProductModel>(await _productRepository.FirstOrDefaultAsync(e => e.Id == id && (isAdmin || e.Active), Includes));
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

    public async Task<PagedResponse<ProductModel>?> SearchAsync(ProductSearchRequest request)
    {
        try
        {
            var isAdmin = await _userService.CurrentUserHasRole(RoleEnum.Admin);

            #region Be careful when changing the permissions check

            Expression<Func<Product, bool>> permitions = e => isAdmin || e.Active;

            #endregion

            Expression<Func<Product, bool>> filters = null;

            switch (request.ConditionType)
            {
                case ConditionTypeEnum.Or:
                    filters = e =>
                        ((request.IdBrand.HasValue && e.IdBrand == request.IdBrand)
                         || (request.IdCategory.HasValue && e.Subcategory.IdCategory == request.IdCategory)
                         || (request.IdSubcategory.HasValue && e.IdSubcategory == request.IdSubcategory)
                         || (request.IdGender.HasValue && e.IdGender == request.IdGender)
                         || (!string.IsNullOrEmpty(request.Name) && e.Name.Contains(request.Name))
                         || (request.MinPrice.HasValue && e.UnitPrice >= request.MinPrice)
                         || (request.MaxPrice.HasValue && e.UnitPrice <= request.MaxPrice)
                         || (!string.IsNullOrEmpty(request.Sku) && e.Sku.Trim().ToUpper() == request.Sku.Trim().ToUpper())
                         || (request.Active.HasValue && e.Active == request.Active)
                        );
                    break;
                default:
                case ConditionTypeEnum.And:
                    filters = e =>
                        (!request.IdBrand.HasValue || e.IdBrand == request.IdBrand)
                        && (!request.IdCategory.HasValue || e.Subcategory.IdCategory == request.IdCategory)
                        && (!request.IdSubcategory.HasValue || e.IdSubcategory == request.IdSubcategory)
                        && (!request.IdGender.HasValue || e.IdGender == request.IdGender)
                        && (string.IsNullOrEmpty(request.Name) || e.Name.Contains(request.Name))
                        && (!request.MinPrice.HasValue || e.UnitPrice >= request.MinPrice)
                        && (!request.MaxPrice.HasValue || e.UnitPrice <= request.MaxPrice)
                        && (string.IsNullOrEmpty(request.Sku) || e.Sku.Trim().ToUpper() == request.Sku.Trim().ToUpper())
                        && (!request.Active.HasValue || e.Active == request.Active);
                    break;
            }

            var combined = permitions.AndAlso(filters);

            return _mapper.Map<PagedResponse<ProductModel>>(
                await _productRepository.GetPagedAsync(
                    request,
                    Includes,
                    combined,
                    new Dictionary<Expression<Func<Product, object>>, OrderByEnum>()
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