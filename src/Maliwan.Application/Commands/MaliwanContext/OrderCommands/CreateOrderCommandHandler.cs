using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Maliwan.Domain.MaliwanContext.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.OrderCommands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IOrderRepository _orderRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderValidator _orderValidator;

    public CreateOrderCommandHandler(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IOrderRepository orderRepository, IStockRepository stockRepository, IProductRepository productRepository,
        IOrderValidator orderValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _orderRepository = orderRepository;
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _orderValidator = orderValidator;
    }

    public async Task<OrderModel?> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
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

            if (command.Items == null || !command.Items.Any())
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.OrderItemsMinVal),
                    _commonMessagesLocalizer.GetString(nameof(CommonMessages.OrderItemsMinVal))));
                return default;
            }

            var entity = _mapper.Map<Order>(command);
            var validationError = false;

            foreach (var item in command.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.IdProduct);
                if (product == null)
                {
                    validationError = true;
                    await _mediator.Publish(new DomainNotification(
                        nameof(CommonMessages.ProductNotFound),
                        _commonMessagesLocalizer.GetString(nameof(CommonMessages.ProductNotFound))));
                    continue;
                } 
                else if (item.Quantity <= 0)
                {
                    validationError = true;
                    await _mediator.Publish(new DomainNotification(nameof(CommonMessages.OrderItemQuantityMinVal),
                        _commonMessagesLocalizer.GetString(nameof(CommonMessages.OrderItemQuantityMinVal))
                            .ToString()
                            .Replace("#ProductName",
                                product.Name)
                            .Replace("#QuantityMinVal",
                                "0")));
                    continue;
                }

                var stocks = (await _stockRepository.FindAsync(e => 
                    e.Active 
                    && e.IdProduct == item.IdProduct
                    && (e.InputQuantity - e.OrderItems.Sum(oi => oi.Quantity)) > 0,
                    new[]
                    {
                        nameof(Stock.Product),
                        nameof(Stock.OrderItems)
                    })).ToList();
                stocks = stocks.OrderBy(e => e.InputDate).ToList();

                var currentQuantity = stocks?.Sum(e => e.CurrentQuantity) ?? 0;
                if (stocks == null || !stocks.Any() || currentQuantity < item.Quantity)
                {
                    validationError = true;
                    
                    if (stocks == null || !stocks.Any())
                    {
                        await _mediator.Publish(new DomainNotification(nameof(CommonMessages.ProductOutOfStock),
                            _commonMessagesLocalizer.GetString(nameof(CommonMessages.ProductOutOfStock))
                                .ToString()
                                .Replace("#ProductName",
                                    product.Name)));
                    }
                    else if (currentQuantity < item.Quantity)
                    {
                        await _mediator.Publish(new DomainNotification(nameof(CommonMessages.StockQuantityExceded),
                            _commonMessagesLocalizer.GetString(nameof(CommonMessages.StockQuantityExceded))
                                .ToString()
                                .Replace("#ProductName",
                                    product.Name)
                                .Replace("#CurrentQuantity",
                                    currentQuantity.ToString())));
                    }
                }
                else
                {
                    var needQuantity = int.Parse($"{item.Quantity}");

                    do
                    {
                        var stock = stocks.FirstOrDefault();
                        stocks.Remove(stock);
                        var quantity = needQuantity <= stock.CurrentQuantity ? needQuantity : stock.CurrentQuantity;
                        needQuantity -= quantity;
                        entity.OrderItems.Add(new OrderItem(stock.Id, quantity, stock.Product.UnitPrice, 0M));
                    } while (needQuantity > 0);
                }
            }

            if (validationError)
            {
                return default;
            }

            if (!command.AggregateId.HasValue)
                await _orderRepository.UnitOfWork.BeginTransaction();

            if (!await _orderValidator.IsValidAsync(entity))
            {
                foreach (var error in _orderValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _orderRepository.InsertAsync(entity);

            if (!command.AggregateId.HasValue)
                await _orderRepository.UnitOfWork.Commit();

            return _mapper.Map<OrderModel>(entity);
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor?.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _commonMessagesLocalizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return default; ;
    }
}