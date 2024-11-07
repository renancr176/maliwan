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

namespace Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;

public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IPaymentMethodValidator _paymentMethodValidator;

    public UpdatePaymentMethodCommandHandler(IMediator mediator, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IUserService userService,
        IStringLocalizer<CommonMessages> commonMessagesLocalizer, IPaymentMethodRepository paymentMethodRepository,
        IPaymentMethodValidator paymentMethodValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _paymentMethodRepository = paymentMethodRepository;
        _paymentMethodValidator = paymentMethodValidator;
    }

    public async Task<PaymentMethodModel?> Handle(UpdatePaymentMethodCommand command, CancellationToken cancellationToken)
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

            var entity = await _paymentMethodRepository.GetByIdAsync(command.Id);

            if (entity == null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.BrandNotFound),
                    _commonMessagesLocalizer.GetString(nameof(CommonMessages.BrandNotFound))));
            }

            entity = _mapper.Map(command, entity);

            if (!command.AggregateId.HasValue)
                await _paymentMethodRepository.UnitOfWork.BeginTransaction();

            if (!await _paymentMethodValidator.IsValidAsync(entity))
            {
                foreach (var error in _paymentMethodValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _paymentMethodRepository.UpdateAsync(entity);

            if (!command.AggregateId.HasValue)
                await _paymentMethodRepository.UnitOfWork.Commit();

            return _mapper.Map<PaymentMethodModel>(entity);
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