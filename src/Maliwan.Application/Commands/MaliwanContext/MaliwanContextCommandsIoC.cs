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
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.MaliwanContext.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Commands.MaliwanContext;

public static class MaliwanContextCommandsIoC
{
    public static void AddMaliwanContextCommands(this IServiceCollection services)
    {
        #region Brands

        services.AddScoped<IRequestHandler<CreateBrandCommand, BrandModel?>, CreateBrandCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateBrandCommand, BrandModel?>, UpdateBrandCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteBrandCommand, bool>, DeleteBrandCommandHandler>();

        #endregion

        #region Category

        services.AddScoped<IRequestHandler<CreateCategoryCommand, CategoryModel?>, CreateCategoryCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateCategoryCommand, CategoryModel?>, UpdateCategoryCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteCategoryCommand, bool>, DeleteCategoryCommandHandler>();

        #endregion

        #region Customer

        services.AddScoped<IRequestHandler<CreateCustomerCommand, CustomerModel?>, CreateCustomerCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateCustomerCommand, CustomerModel?>, UpdateCustomerCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteCustomerCommand, bool>, DeleteCustomerCommandHandler>();

        #endregion

        #region Gender

        services.AddScoped<IRequestHandler<CreateGenderCommand, GenderModel?>, CreateGenderCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateGenderCommand, GenderModel?>, UpdateGenderCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteGenderCommand, bool>, DeleteGenderCommandHandler>();

        #endregion

        #region Order

        services.AddScoped<IRequestHandler<CreateOrderCommand, OrderModel?>, CreateOrderCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteOrderCommand, bool>, DeleteOrderCommandHandler>();

        #endregion

        #region OrderPayment



        #endregion

        #region PaymentMethod

        services
            .AddScoped<IRequestHandler<CreatePaymentMethodCommand, PaymentMethodModel?>,
                CreatePaymentMethodCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodModel?>,
                UpdatePaymentMethodCommandHandler>();
        services.AddScoped<IRequestHandler<DeletePaymentMethodCommand, bool>, DeletePaymentMethodCommandHandler>();

        #endregion

        #region Product

        services.AddScoped<IRequestHandler<CreateProductCommand, ProductModel?>, CreateProductCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateProductCommand, ProductModel?>, UpdateProductCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteProductCommand, bool>, DeleteProductCommandHandler>();

        #endregion

        #region ProductColor

        services
            .AddScoped<IRequestHandler<CreateProductColorCommand, ProductColorModel?>,
                CreateProductColorCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdateProductColorCommand, ProductColorModel?>,
                UpdateProductColorCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteProductColorCommand, bool>, DeleteProductColorCommandHandler>();

        #endregion

        #region ProductSize

        services
            .AddScoped<IRequestHandler<CreateProductSizeCommand, ProductSizeModel?>, CreateProductSizeCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdateProductSizeCommand, ProductSizeModel?>, UpdateProductSizeCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteProductSizeCommand, bool>, DeleteProductSizeCommandHandler>();

        #endregion

        #region Stock

        services.AddScoped<IRequestHandler<CreateStockCommand, StockModel?>, CreateStockCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateStockCommand, StockModel?>, UpdateStockCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteStockCommand, bool>, DeleteStockCommandHandler>();

        #endregion

        #region Subcategory

        services
            .AddScoped<IRequestHandler<CreateSubcategoryCommand, SubcategoryModel?>, CreateSubcategoryCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdateSubcategoryCommand, SubcategoryModel?>, UpdateSubcategoryCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteSubcategoryCommand, bool>, DeleteSubcategoryCommandHandler>();

        #endregion
    }
}