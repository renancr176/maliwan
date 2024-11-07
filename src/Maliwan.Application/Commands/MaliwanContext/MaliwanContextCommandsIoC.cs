using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
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

        #region Subcategory

        services
            .AddScoped<IRequestHandler<CreateSubcategoryCommand, SubcategoryModel?>, CreateSubcategoryCommandHandler>();
        services
            .AddScoped<IRequestHandler<UpdateSubcategoryCommand, SubcategoryModel?>, UpdateSubcategoryCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteSubcategoryCommand, bool>, DeleteSubcategoryCommandHandler>();

        #endregion
    }
}