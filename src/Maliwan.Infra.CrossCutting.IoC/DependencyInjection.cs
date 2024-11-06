using Maliwan.Application.AutoMapper;
using Maliwan.Application.Commands;
using Maliwan.Application.Events;
using Maliwan.Application.Queries;
using Maliwan.Application.Services;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Options;
using Maliwan.Domain.IdentityContext.Validators;
using Maliwan.Domain.Maliwan.Validators;
using Maliwan.Infra.Data.Contexts.IdentityDb;
using Maliwan.Infra.Data.Contexts.MaliwanDb;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Infra.CrossCutting.IoC;

public static class DependencyInjection
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        services.AddAutoMapperProfiles();

        #region Options

        services.Configure<JwtTokenOptions>(configuration.GetSection(JwtTokenOptions.sectionKey));

        #endregion

        #region DbContexts

        services.AddIdentityDb(configuration);
        services.AddMaliwanDb(configuration);

        #endregion

        services.AddIdentityValidators();
        services.AddMaliwanValidators();
        services.AddCommands();
        services.AddEvents();
        services.AddQueries();
        services.AddServices();

        #region External Services
        #endregion
    }
}