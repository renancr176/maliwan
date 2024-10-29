using Maliwan.Domain.IdentityContext.Interfaces.Repositories;
using Maliwan.Infra.Data.Contexts.IdentityDb.Repositories;
using Maliwan.Infra.Data.Contexts.IdentityDb.Seeders;
using Maliwan.Infra.Data.Contexts.IdentityDb.Seeders.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Infra.Data.Contexts.IdentityDb;

public static class IdentityDb
{
    public static void AddIdentityDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                options => options.EnableRetryOnFailure()));

        #region Repositories

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        #endregion

        #region Seeders

        services.AddScoped<IRoleSeed, RoleSeed>();

        #endregion
    }

    public static void IdentityDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<IdentityDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () =>
        {
            await serviceProvider.GetService<IRoleSeed>().SeedAsync();
        }).Wait();

        #endregion
    }
}