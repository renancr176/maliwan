using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.StockCommands;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, StockModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IStockRepository _stockRepository;
    private readonly IStockValidator _stockValidator;

    public UpdateStockCommandHandler(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IStockRepository stockRepository, IStockValidator stockValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _stockRepository = stockRepository;
        _stockValidator = stockValidator;
    }

    public async Task<StockModel?> Handle(UpdateStockCommand command, CancellationToken cancellationToken)
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

            var entity = await _stockRepository.GetByIdAsync(command.Id);

            if (entity == null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.BrandNotFound),
                    _commonMessagesLocalizer.GetString(nameof(CommonMessages.BrandNotFound))));
            }

            entity = _mapper.Map(command, entity);

            if (!command.AggregateId.HasValue)
                await _stockRepository.UnitOfWork.BeginTransaction();

            if (!await _stockValidator.IsValidAsync(entity))
            {
                foreach (var error in _stockValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _stockRepository.UpdateAsync(entity);

            if (!command.AggregateId.HasValue)
                await _stockRepository.UnitOfWork.Commit();

            return _mapper.Map<StockModel>(entity);
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