namespace Maliwan.Application;

public class CommonMessages
{
    public const string InternalServerError = "An internal server error occurred, please try again later.";
    public const string Unauthorized = "Unauthorized.";
    public const string UserNotFound = "The user was not found.";
    public const string UserNotFoundByEmail = "There is no user with email #Email.";
    public const string InvalidConfirmationKey = "Invalid confirmation key.";
    public const string UnableSendMail = "Unable to send email.";
    public const string EmailPasswordResetSubject = "Change password.";
    public const string EmailPasswordResetBody =
        @"<p>Hello #Name</p><br/><p>Your password change code is:</p><br/><h3>#Token</h3>";
    public const string PasswordResetEmailSent = "Check your email for further instructions.";
    public const string InvalidToken = "Invalid reset password token.";
    public const string LoginBlocked = "Login blocked.";
    public const string InvalidUseramePassword = "Invalid username or password.";
    public const string EmailNotConfirmed = "Unverified email, check your email and follow the instructions for email confirmation.";
    public const string InvalidPassword = "Password pattern not met.";
    public const string UnableToUpload = "Got a problem when trying to upload the file.";
    public const string ObjectUploadAlreadyFinished = "Got a problem when trying to upload the file.";
    public const string ObjectUploadInvalidPart = "Got a problem when trying to upload the file.";
    public const string MetadataIsRequired = "The file metadata is required.";
    public const string InvalidJwtToken = "The token is invalid.";
    public const string InvalidJwtRefreshToken = "The refresh token is invalid.";
    public const string JwtTokenIsStillValid = "The JWT token is still valid, it is not possible to generate a new token until the previous token has expired.";
    public const string RelationshipNotFound = "Relationship not found.";

    #region Entity messages

    public const string BrandNotFound = "Brand not found.";
    public const string CategoryNotFound = "Category not found.";
    public const string GenderNotFound = "Gender not found.";
    public const string PaymentMethodNotFound = "Payment method not found.";
    public const string ProductColorNotFound = "Product color not found.";
    public const string ProductSizeNotFound = "Product size not found.";
    public const string SubcategoryNotFound = "Subcategory not found.";

    #endregion
}