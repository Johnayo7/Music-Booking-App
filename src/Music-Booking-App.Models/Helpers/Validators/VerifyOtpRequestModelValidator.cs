using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class VerifyOtpRequestModelValidator : AbstractValidator<VerifyOtpRequestModel>
    {
        public VerifyOtpRequestModelValidator()
        {
            RuleFor(model => model.Otp)
                .NotNull()
                .NotEmpty()
                .Length(4);

            RuleFor(model => model.Email)
                .NotNull()
                .NotEmpty()
                .Length(5, 50)
                .WithMessage("{PropertyName} must be within 5 to 50 characters")
                .EmailAddress()
                .WithMessage("{PropertyName} is not a valid email address.")
                .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$") // Regex pattern for valid email
                .WithMessage("{PropertyName} cannot contain invalid characters.");
        }
    }
}
