using AutoMapper;
using FluentValidation.Results;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Responses;

namespace Maliwan.Application.AutoMapper;

public class ConvertObjectsProfile : Profile
{

    public ConvertObjectsProfile()
    {
        CreateMap<ValidationFailure, DomainNotification>()
            .ConstructUsing(src => new DomainNotification(src.ErrorCode, src.ErrorMessage));

        CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
    }
}