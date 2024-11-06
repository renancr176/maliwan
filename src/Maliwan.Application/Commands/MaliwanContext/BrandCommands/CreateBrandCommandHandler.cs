using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;

namespace Maliwan.Application.Commands.MaliwanContext.BrandCommands;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly IBrandRepository _brandRepository;
    private readonly IBrandValidator _brandValidator;

    public CreateBrandCommandHandler(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        IBrandRepository brandRepository, IBrandValidator brandValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _brandRepository = brandRepository;
        _brandValidator = brandValidator;
    }

    public async Task<BrandModel?> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
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

            var entity = _mapper.Map<Brand>(command);

            if (!command.AggregateId.HasValue)
                await _brandRepository.UnitOfWork.BeginTransaction();

            if (!await _brandValidator.IsValidAsync(entity))
            {
                foreach (var error in _brandValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _brandRepository.InsertAsync(entity);

            if (!command.AggregateId.HasValue)
                await _brandRepository.UnitOfWork.Commit();

            return _mapper.Map<BrandModel>(entity);
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