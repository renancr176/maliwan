using AutoMapper;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.Maliwan.Entities;

namespace Maliwan.Application.AutoMapper;

public class CommandToEntityMappingProfile : Profile
{
    public CommandToEntityMappingProfile()
    {
        #region Identity Context

        CreateMap<SignUpCommand, User>();

        #endregion

        #region Maliwan Context

        #region Brand

        CreateMap<CreateBrandCommand, Brand>();
        CreateMap<UpdateBrandCommand, Brand>();

        #endregion

        #endregion
    }
}