using FluentValidation;
using FluentValidation.Validators;

namespace Sashimi.GoogleCloud.Accounts
{
    class GoogleCloudAccountValidator : AbstractValidator<GoogleCloudAccountDetails>
    {
        public GoogleCloudAccountValidator()
        {
            RuleFor(p => p.AccountEmail).EmailAddress(EmailValidationMode.AspNetCoreCompatible).WithMessage("A valid account email is required.");
            RuleFor(p => p.JsonKey).NotEmpty().WithMessage("Json key is required.");
        }
    }
}