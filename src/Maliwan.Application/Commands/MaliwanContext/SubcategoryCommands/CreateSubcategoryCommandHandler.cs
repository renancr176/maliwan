using AutoMapper;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

public class CreateSubcategoryCommandHandler : IRequestHandler<CreateSubcategoryCommand, SubcategoryModel?>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CommonMessages> _commonMessagesLocalizer;
    private readonly ISubcategoryRepository _subcategoryRepository;
    private readonly ISubcategoryValidator _subcategoryValidator;

    public CreateSubcategoryCommandHandler(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IUserService userService, IStringLocalizer<CommonMessages> commonMessagesLocalizer,
        ISubcategoryRepository subcategoryRepository, ISubcategoryValidator subcategoryValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _commonMessagesLocalizer = commonMessagesLocalizer;
        _subcategoryRepository = subcategoryRepository;
        _subcategoryValidator = subcategoryValidator;
    }

    public async Task<SubcategoryModel?> Handle(CreateSubcategoryCommand command, CancellationToken cancellationToken)
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
            var entity = _mapper.Map<Subcategory>(command);

            if (!command.AggregateId.HasValue)
                await _subcategoryRepository.UnitOfWork.BeginTransaction();

            if (!await _subcategoryValidator.IsValidAsync(entity))
            {
                foreach (var error in _subcategoryValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _subcategoryRepository.InsertAsync(entity);

            if (!command.AggregateId.HasValue)
                await _subcategoryRepository.UnitOfWork.Commit();

            return _mapper.Map<SubcategoryModel>(entity);
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