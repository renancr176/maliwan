using FluentValidation;
using FluentValidation.Results;
using Maliwan.Domain.Core.Options;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.IdentityContext.Interfaces.Repositories;
using Maliwan.Domain.IdentityContext.Interfaces.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Maliwan.Domain.IdentityContext.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshToken>, IRefreshTokenValidator
{
    #region Errors codes with message

    public const string UserNotFound = "Message from translation file.";

    public const string TokenIsRequired = "Message from translation file.";
    public const string TokenMinLength = "Message from translation file.";
    public const string TokenAlreadyExists = "Message from translation file.";

    public const string ValidUntilMinVal = "Message from translation file.";

    #endregion

    public ValidationResult ValidationResult { get; private set; }

    private readonly IStringLocalizer<RefreshTokenValidator> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOptions<JwtTokenOptions> _jwtTokenOptions;

    public RefreshTokenValidator(IStringLocalizer<RefreshTokenValidator> localizer, UserManager<User> userManager,
        IRefreshTokenRepository refreshTokenRepository, IOptions<JwtTokenOptions> jwtTokenOptions)
    {
        _localizer = localizer;
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenOptions = jwtTokenOptions;

        RuleFor(e => e.UserId)
            .MustAsync(UserExistsAsync)
            .WithErrorCode(nameof(UserNotFound))
            .WithMessage(_localizer.GetString(nameof(UserNotFound)));

        RuleFor(e => e.Token)
            .NotNull()
            .WithErrorCode(nameof(TokenIsRequired))
            .WithMessage(_localizer.GetString(nameof(TokenIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(TokenIsRequired))
            .WithMessage(localizer.GetString(nameof(TokenIsRequired)))
            .MinimumLength(32)
            .WithErrorCode(nameof(TokenMinLength))
            .WithMessage(localizer.GetString(nameof(TokenMinLength)).ToString().Replace("#Length", "32"))
            .MustAsync(TokenIsUniqueAsync)
            .WithErrorCode(nameof(TokenAlreadyExists))
            .WithMessage(localizer.GetString(nameof(TokenAlreadyExists)));

        RuleFor(e => e.ValidUntil)
            .GreaterThan(_jwtTokenOptions.Value.Expiration)
            .WithErrorCode(nameof(ValidUntilMinVal))
            .WithMessage(localizer.GetString(nameof(ValidUntilMinVal))
                .ToString()
                .Replace(
                    "#Val",
                    _jwtTokenOptions.Value.Expiration.ToString("g",
                        CultureInfo.CurrentCulture)));
    }

    private async Task<bool> UserExistsAsync(Guid userId, CancellationToken arg2)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null;
    }

    private async Task<bool> TokenIsUniqueAsync(RefreshToken entity, string token, CancellationToken arg3)
    {
        return !await _refreshTokenRepository.AnyAsync(e =>
            e.Id != entity.Id
            && e.Token == token);
    }

    public async Task<bool> IsValidAsync(RefreshToken entity)
    {
        ValidationResult = await ValidateAsync(entity);
        return ValidationResult.IsValid;
    }
}