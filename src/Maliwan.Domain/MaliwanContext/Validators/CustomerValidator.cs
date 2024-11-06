using FluentValidation;
using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Core.Extensions;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Interfaces.Repositories;
using Maliwan.Domain.MaliwanContext.Interfaces.Validators;
using Microsoft.Extensions.Localization;

namespace Maliwan.Domain.MaliwanContext.Validators;

public class CustomerValidator : EntityValidator<Customer>, ICustomerValidator
{
    #region Errors codes with message

    public const string NameIsRequired = "Message from translation file.";
    public const string NameMinLength = "Message from translation file.";
    public const string NameMaxLength = "Message from translation file.";
    public const string NameNotFull = "Message from translation file.";

    public const string DocumentIsRequired = "Message from translation file.";
    public const string DocumentIsInvalid = "Message from translation file.";
    public const string DocumentMaxLength = "Message from translation file.";
    public const string DocumentAlreadyExists = "Message from translation file.";

    public const string TypeIsInvalid = "Message from translation file.";

    #endregion

    private readonly IStringLocalizer<CustomerValidator> _localizer;
    private readonly ICustomerRepository _customerRepository;

    public CustomerValidator(IStringLocalizer<CustomerValidator> localizer, ICustomerRepository customerRepository)
    {
        _localizer = localizer;
        _customerRepository = customerRepository;

        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(NameIsRequired))
            .WithMessage(_localizer.GetString(nameof(NameIsRequired)))
            .MinimumLength(3)
            .WithErrorCode(nameof(NameMinLength))
            .WithMessage(_localizer.GetString(nameof(NameMinLength)).ToString().Replace("#MinLength", "3"))
            .MaximumLength(255)
            .WithErrorCode(nameof(NameMaxLength))
            .WithMessage(_localizer.GetString(nameof(NameMaxLength)).ToString().Replace("#MaxLength", "255"))
            .Must(FullName)
            .WithErrorCode(nameof(NameNotFull))
            .WithMessage(_localizer.GetString(nameof(NameNotFull)));

        RuleFor(e => e.Document)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(nameof(DocumentIsRequired))
            .WithMessage(_localizer.GetString(nameof(DocumentIsRequired)))
            .NotEmpty()
            .WithErrorCode(nameof(DocumentIsRequired))
            .WithMessage(_localizer.GetString(nameof(DocumentIsRequired)))
            .Must(ValidDocument)
            .WithErrorCode(nameof(DocumentIsInvalid))
            .WithMessage(_localizer.GetString(nameof(DocumentIsInvalid)))
            .MaximumLength(50)
            .WithErrorCode(nameof(DocumentMaxLength))
            .WithMessage(_localizer.GetString(nameof(DocumentMaxLength)).ToString().Replace("#MaxLength", "50"))
            .MustAsync(UniqueDocumentAsync)
            .WithErrorCode(nameof(DocumentAlreadyExists))
            .WithMessage(_localizer.GetString(nameof(DocumentAlreadyExists)));

        RuleFor(e => e.Type)
            .IsInEnum()
            .WithErrorCode(nameof(TypeIsInvalid))
            .WithMessage(_localizer.GetString(nameof(TypeIsInvalid)));
    }

    private bool ValidDocument(string document)
    {
        return document.IsCpf() || document.IsCnpj();
    }

    private async Task<bool> UniqueDocumentAsync(Customer entity, string document, CancellationToken arg3)
    {
        return !await _customerRepository.AnyAsync(e => e.Id != entity.Id && e.Document == document);
    }

    private bool FullName(string name)
    {
        return name.Trim().Split(" ").Length > 0;
    }
}