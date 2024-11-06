using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Domain.MaliwanContext.Validators;

public static class ValidatorIoC
{
    public static void AddMaliwanValidators(this IServiceCollection services)
    {
        services.AddScoped<IBrandValidator, BrandValidator>();
        services.AddScoped<ICategoryValidator, CategoryValidator>();
        services.AddScoped<ICustomerValidator, CustomerValidator>();
        services.AddScoped<IGenderValidator, GenderValidator>();
        services.AddScoped<IOrderItemValidator, OrderItemValidator>();
        services.AddScoped<IOrderPaymentValidator, OrderPaymentValidator>();
        services.AddScoped<IOrderValidator, OrderValidator>();
        services.AddScoped<IPaymentMethodValidator, PaymentMethodValidator>();
        services.AddScoped<IProductColorValidator, ProductColorValidator>();
        services.AddScoped<IProductSizeValidator, ProductSizeValidator>();
        services.AddScoped<IProductValidator, ProductValidator>();
        services.AddScoped<IStockValidator, StockValidator>();
        services.AddScoped<ISubcategoryValidator, SubcategoryValidator>();
    }
}