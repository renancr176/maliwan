using Maliwan.Application.Commands.IdentityContext.UserCommands;
using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Application.Models.IdentityContext;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Maliwan.Application.Commands.IdentityContext;

public static class IdentityContextCommandsIoC
{
    public static void AddIdentityContextCommands(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<SignInCommand, SignInResponseModel>, SignInCommandHandler>();
        services.AddScoped<IRequestHandler<SignInRefreshCommand, SignInResponseModel?>, SignInRefreshCommandHandler>();
        services.AddScoped<IRequestHandler<SignUpCommand, UserModel>, SignUpCommandHandler>();
        services.AddScoped<IRequestHandler<PasswordResetCommand, string>, PasswordResetCommandHandler>();
        services.AddScoped<IRequestHandler<ResetPasswordCommand, string>, ResetPasswordCommandHandler>();
        services.AddScoped<IRequestHandler<ConfirmEmailCommand, bool>, ConfirmEmailCommandHandler>();
        services.AddScoped<IRequestHandler<UserAddRoleCommand, bool>, UserAddRoleCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteExpiredRefreshTokensCommand, bool>, DeleteExpiredRefreshTokensCommandHandler>();
    }
}