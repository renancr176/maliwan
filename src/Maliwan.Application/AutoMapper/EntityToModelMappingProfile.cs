using AutoMapper;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.MaliwanContext.Entities;

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
        CreateMap<Category, CategoryModel>();
        CreateMap<Customer, CustomerModel>();
        CreateMap<Gender, GenderModel>();
        CreateMap<Order, OrderModel>();
        CreateMap<OrderItem, OrderItemModel>();
        CreateMap<OrderPayment, OrderPaymentModel>();
        CreateMap<PaymentMethod, PaymentMethodModel>();
        CreateMap<Product, ProductModel>();
        CreateMap<ProductColor, ProductColorModel>();
        CreateMap<ProductSize, ProductSizeModel>();
        CreateMap<Stock, StockModel>();
        CreateMap<Subcategory, SubcategoryModel>();

        #endregion
    }
}
