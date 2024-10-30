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

        //services.AddScoped<IObjectRepository, ObjectRepository>();

        #endregion

        #region Seeders

        //services.AddScoped<IObjectTypeSeed, ObjectTypeSeed>();

        #endregion
    }

    public static void MaliwanDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<MaliwanDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () => {
            //await serviceProvider.GetService<IObjectTypeSeed>().SeedAsync();
        }).Wait();

        #endregion
    }
}