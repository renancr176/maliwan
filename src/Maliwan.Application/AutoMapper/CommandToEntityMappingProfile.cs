using AutoMapper;
using Azure;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Domain.IdentityContext.Entities;

namespace Maliwan.Application.AutoMapper;

public class CommandToEntityMappingProfile : Profile
{
    public CommandToEntityMappingProfile()
    {
        #region Identity Context

        CreateMap<SignUpCommand, User>();

        #endregion
    }
}