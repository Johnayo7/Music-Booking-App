
using Music_Booking_App.Models.RequestModels;
using FluentValidation;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginRequestModelValidator()
        {
            RuleFor(model => model.Email)
                    .NotNull().WithMessage("Email address cannot be null.")
                    .NotEmpty().WithMessage("Email address is required.")
                    .Length(5, 50).WithMessage("Email address must be within 5 to 50 characters.")
                    .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(model => model.Password)
                    .NotNull()
                    .NotEmpty()
                    .MinimumLength(8)
                    .WithMessage("{PropertyName} must be at least 8 characters long.")
                    .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
                    .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                    .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                    .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one non-alphanumeric character.");
        }
    }
}
