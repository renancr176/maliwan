using AutoMapper;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;
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

        #region Category

        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, Category>();

        #endregion

        #region Subcategory

        CreateMap<CreateSubcategoryCommand, Subcategory>();

        #endregion

        #endregion
    }
}