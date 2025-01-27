﻿using Maliwan.Application.Queries.MaliwanContext.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Queries.MaliwanContext;

public static class MaliwanContextQueriesIoC
{
    public static void AddMaliwanContextQueries(this IServiceCollection services)
    {
        services.AddScoped<IBrandQuery, BrandQuery>();
        services.AddScoped<ICategoryQuery, CategoryQuery>();
        services.AddScoped<ICustomerQuery, CustomerQuery>();
        services.AddScoped<IGenderQuery, GenderQuery>();
        services.AddScoped<IOrderQuery, OrderQuery>();
        services.AddScoped<IOrderPaymentQuery, OrderPaymentQuery>();
        services.AddScoped<IPaymentMethodQuery, PaymentMethodQuery>();
        services.AddScoped<IProductQuery, ProductQuery>();
        services.AddScoped<IProductColorQuery, ProductColorQuery>();
        services.AddScoped<IProductSizeQuery, ProductSizeQuery>();
        services.AddScoped<IStockQuery, StockQuery>();
        services.AddScoped<ISubcategoryQuery, SubcategoryQuery>();
    }
}