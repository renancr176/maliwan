using AutoMapper;
using Azure;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Domain.IdentityContext.Entities;

namespace Maliwan.Application.AutoMapper;

public class EntityToModelMappingProfile : Profile
{
    public EntityToModelMappingProfile()
    {
        #region Identity Context

        CreateMap<User, UserBaseModel>();
        CreateMap<User, UserModel>();

        #endregion
    }
}
