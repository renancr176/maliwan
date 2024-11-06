using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Infra.Data.Contexts.MaliwanDb.Repositories;
using Maliwan.Infra.Data.Contexts.MaliwanDb.Seeders;
using Maliwan.Infra.Data.Contexts.MaliwanDb.Seeders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb;

public static class MaliwanDb
{
    public static void AddMaliwanDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MaliwanDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //services.AddDbContext<IdentityDbContext>(dbContextOptions =>
        //    dbContextOptions.UseMySql(configuration.GetConnectionString("DefaultConnection"),
        //        ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));

        #region Repositories

        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IGenderRepository, GenderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IProductColorRepository, ProductColorRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();

        #endregion

        #region Seeders

        services.AddScoped<IGenderSeeder, GenderSeeder>();

        #endregion
    }

    public static void MaliwanDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<MaliwanDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () => {
            await serviceProvider.GetService<IGenderSeeder>().SeedAsync();
        }).Wait();

        #endregion
    }
}