using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
using Maliwan.Application.Commands.MaliwanContext.GenderCommands;
using Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;
using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;
using Maliwan.Application.Models.MaliwanContext;
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

        #region Gender

        services.AddScoped<IRequestHandler<CreateGenderCommand, GenderModel?>, CreateGenderCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateGenderCommand, GenderModel?>, UpdateGenderCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteGenderCommand, bool>, DeleteGenderCommandHandler>();

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

        #region Subcategory

        services
            .AddScoped<IRequestHandler<CreateSubcategoryCommand, SubcategoryModel?>, CreateSubcategoryCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdateSubcategoryCommand, SubcategoryModel?>, UpdateSubcategoryCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteSubcategoryCommand, bool>, DeleteSubcategoryCommandHandler>();

        #endregion
    }
}