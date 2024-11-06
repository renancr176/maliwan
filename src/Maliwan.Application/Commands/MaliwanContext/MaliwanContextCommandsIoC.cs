using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
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
    }
}