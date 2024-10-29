using AutoMapper;
using FluentValidation.Results;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;

namespace Maliwan.Application.AutoMapper;

public class ConvertObjectsProfile : Profile
{

    public ConvertObjectsProfile()
    {
        CreateMap<ValidationFailure, DomainNotification>()
            .ConstructUsing(src => new DomainNotification(src.ErrorCode, src.ErrorMessage));
    }
}