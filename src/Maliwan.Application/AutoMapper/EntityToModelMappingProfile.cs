using AutoMapper;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.Maliwan.Entities;

namespace Maliwan.Application.AutoMapper;

public class EntityToModelMappingProfile : Profile
{
    public EntityToModelMappingProfile()
    {
        #region Identity Context

        CreateMap<User, UserBaseModel>();
        CreateMap<User, UserModel>();

        #endregion

        #region Maliwan Context

        CreateMap<Brand, BrandModel>();
        CreateMap<PagedResponse<Brand>, PagedResponse<BrandModel>>();

        #endregion
    }
}
