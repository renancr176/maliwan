using AutoMapper;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
using Maliwan.Application.Commands.MaliwanContext.CustomerCommands;
using Maliwan.Application.Commands.MaliwanContext.GenderCommands;
using Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;
using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.MaliwanContext.Entities;

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

        #region Customer

        CreateMap<CreateCustomerCommand, Customer>();
        CreateMap<UpdateBrandCommand, Customer>();

        #endregion

        #region Gender

        CreateMap<CreateGenderCommand, Gender>();
        CreateMap<UpdateGenderCommand, Gender>();

        #endregion

        #region PaymentMethod

        CreateMap<CreatePaymentMethodCommand, PaymentMethod>();
        CreateMap<UpdatePaymentMethodCommand, PaymentMethod>();

        #endregion

        #region Product

        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();

        #endregion

        #region ProductColor

        CreateMap<CreateProductColorCommand, ProductColor>();
        CreateMap<UpdateProductColorCommand, ProductColor>();

        #endregion

        #region ProductSize

        CreateMap<CreateProductSizeCommand, ProductSize>();
        CreateMap<UpdateProductSizeCommand, ProductSize>();

        #endregion

        #region Subcategory

        CreateMap<CreateSubcategoryCommand, Subcategory>();

        #endregion

        #endregion
    }
}