using Maliwan.Domain.Core.Enums;
using Maliwan.Infra.CrossCutting.IoC;
using Maliwan.Infra.Data.Contexts.IdentityDb;
using Maliwan.Service.Api.Middlewares;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using Maliwan.Domain.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Service.Api;

public class StartupTests
{
    public StartupTests()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.Testing.json", true, true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
        services.AddEndpointsApiExplorer();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StartupTests).Assembly));
        services.AddAutoMapper(typeof(StartupTests).GetTypeInfo().Assembly);
        services.AddHttpContextAccessor();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo(LanguageEnum.Portugues.GetAttributeOfType<DescriptionAttribute>().Description),
                new CultureInfo(LanguageEnum.English.GetAttributeOfType<DescriptionAttribute>().Description)
            };
            options.DefaultRequestCulture = new RequestCulture(
                culture: LanguageEnum.English.GetAttributeOfType<DescriptionAttribute>().Description,
                uiCulture: LanguageEnum.English.GetAttributeOfType<DescriptionAttribute>().Description);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Clear();

            options.RequestCultureProviders.Insert(0,
                new AcceptLanguageHeaderRequestCultureProvider()
                {
                    Options = options
                });
        });

        services.RegisterServices(Configuration);

        Init(services.BuildServiceProvider());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

        app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseGlobalExceptionHandler();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void Init(IServiceProvider serviceProvider)
    {
        var scriptClearDataBase = @"USE #DbName;
        DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR

        SET @Cursor = CURSOR FAST_FORWARD FOR
        SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
        FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
        LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME = rc1.CONSTRAINT_NAME

        OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

        WHILE(@@FETCH_STATUS = 0)
        BEGIN
            Exec sp_executesql @Sql
        FETCH NEXT FROM @Cursor INTO @Sql
        END

        CLOSE @Cursor DEALLOCATE @Cursor

        EXEC sp_MSforeachtable 'DROP TABLE ?'";

        #region IdentityDbContext

        var identityDbName = Configuration.GetSection("ConnectionStrings:IdentityConnection").Value
            .Split(";")
            .FirstOrDefault(i => i.ToLower().Contains("database=".ToLower()))
            .Split("=")
            .LastOrDefault();

        var identityDbContext = serviceProvider.GetService<IdentityDbContext>();

        //Delete all tables before run migrations
        identityDbContext.Database.ExecuteSqlRaw(scriptClearDataBase.Replace("#DbName", identityDbName));

        serviceProvider.IdentityDbMigrate();

        #endregion

        #region DigaXDbContext

        //var dbName = Configuration.GetSection("ConnectionStrings:DefaultConnection").Value
        //    .Split(";")
        //    .FirstOrDefault(i => i.ToLower().Contains("database=".ToLower()))
        //    .Split("=")
        //    .LastOrDefault();

        //var defaultDbContext = serviceProvider.GetService<DigaXDbContext>();

        ////Delete all tables before run migrations
        //defaultDbContext.Database.ExecuteSqlRaw(scriptClearDataBase.Replace("#DbName", dbName));

        //serviceProvider.DigaXDbMigrate();

        #endregion
    }
}