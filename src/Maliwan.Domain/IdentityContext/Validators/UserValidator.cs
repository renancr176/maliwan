using FluentValidation;
using FluentValidation.Results;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.IdentityContext.Interfaces.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.IdentityContext.Validators;

public class UserValidator : AbstractValidator<User>, IUserValidator
{
    #region Consts

    public const string UserNameIsRequired = "The user name is required.";
    public const string UserNameMinLength = "The user name require at least #Length characters.";
    public const string UserNameMaxLength = "The user name exceeded #Length characters.";
    public const string UserNameAlreadyExists = "The user name is already in use.";

    public const string NameIsRequired = "The name is required.";
    public const string NameMinLength = "The name require at least #Length characters.";
    public const string NameMaxLength = "The name exceeded #Length characters.";

    public const string EmailIsRequired = "The email is required.";
    public const string EmailIsInvalid = "The email is invalid.";
    public const string EmailAlreadyExists = "The email is already in use.";

    #endregion

    public ValidationResult ValidationResult { get; private set; }

    private readonly IStringLocalizer<UserValidator> _localizer;
    private readonly UserManager<User> _userManager;

    public UserValidator(IStringLocalizer<UserValidator> localizer, UserManager<User> userManager)
    {
        _localizer = localizer;
        _userManager = userManager;

        RuleFor(e => e.UserName)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotNull()
            .WithErrorCode(nameof(UserNameIsRequired))
            .WithMessage(_localizer.GetString(nameof(UserNameIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(UserNameIsRequired))
            .WithMessage(_localizer.GetString(nameof(UserNameIsRequired)))
            .MinimumLength(1)
            .WithErrorCode(nameof(UserNameMinLength))
            .WithMessage(_localizer.GetString(nameof(UserNameMinLength))?.ToString()?.Replace("#Length", "1"))
            .MaximumLength(255)
            .WithErrorCode(nameof(UserNameMaxLength))
            .WithMessage(_localizer.GetString(nameof(UserNameMaxLength))?.ToString()?.Replace("#Length", "255"))
            .MustAsync(UserNameIsUniqueAsync)
            .WithErrorCode(nameof(UserNameAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(UserNameAlreadyExists)));

        RuleFor(e => e.Name)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotNull()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .MinimumLength(1)
            .WithErrorCode(nameof(NameMinLength))
            .WithMessage(_localizer.GetString(nameof(NameMinLength))?.ToString()?.Replace("#Length", "1"))
            .MaximumLength(255)
            .WithErrorCode(nameof(NameMaxLength))
            .WithMessage(_localizer.GetString(nameof(NameMaxLength))?.ToString()?.Replace("#Length", "255"));

        RuleFor(e => e.Email)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotNull()
            .WithErrorCode(nameof(EmailIsRequired))
            .WithMessage(_localizer.GetString(nameof(EmailIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(EmailIsRequired))
            .WithMessage(_localizer.GetString(nameof(EmailIsRequired)))
            .EmailAddress()
            .WithErrorCode(nameof(EmailIsInvalid))
            .WithMessage(_localizer.GetString(nameof(EmailIsInvalid)))
            .MustAsync(EmailIsUniqueAsync)
            .WithErrorCode(nameof(EmailAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(EmailIsInvalid)));

        RuleFor(e => e.RememberPhrase)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(255);
    }

    private async Task<bool> UserNameIsUniqueAsync(User entity, string userName, CancellationToken arg2)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user == null || user.Id == entity.Id;
    }

    private async Task<bool> EmailIsUniqueAsync(User entity, string email, CancellationToken arg2)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null || user.Id == entity.Id;
    }

    public async Task<bool> IsValidAsync(User user)
    {
        ValidationResult = await ValidateAsync(user);
        return ValidationResult.IsValid;
    }
}