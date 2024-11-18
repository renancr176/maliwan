using AutoMapper;
using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
using Maliwan.Application.Commands.MaliwanContext.CustomerCommands;
using Maliwan.Application.Commands.MaliwanContext.GenderCommands;
using Maliwan.Application.Commands.MaliwanContext.OrderCommands;
using Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductCommands;
using Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;
using Maliwan.Application.Commands.MaliwanContext.StockCommands;
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

        CreateMap<CreateBrandCommand, Brand>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateBrandCommand, Brand>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

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

        CreateMap<CreateGenderCommand, Gender>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateGenderCommand, Gender>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

        #endregion

        #region Order

        CreateMap<CreateOrderCommand, Order>();

        #endregion

        #region OrderPayment



        #endregion

        #region PaymentMethod

        CreateMap<CreatePaymentMethodCommand, PaymentMethod>();
        CreateMap<UpdatePaymentMethodCommand, PaymentMethod>();

        #endregion

        #region Product

        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

        #endregion

        #region ProductColor

        CreateMap<CreateProductColorCommand, ProductColor>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateProductColorCommand, ProductColor>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

        #endregion

        #region ProductSize

        CreateMap<CreateProductSizeCommand, ProductSize>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateProductSizeCommand, ProductSize>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

        #endregion

        #region Stock

        CreateMap<CreateStockCommand, Stock>();
        CreateMap<UpdateStockCommand, Stock>();

        #endregion

        #region Subcategory

        CreateMap<CreateSubcategoryCommand, Subcategory>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));
        CreateMap<UpdateSubcategoryCommand, Subcategory>()
            .ForMember(dest => dest.Sku, act => act.MapFrom(src => src.Sku.Trim().ToUpper()));

        #endregion

        #endregion
    }
}