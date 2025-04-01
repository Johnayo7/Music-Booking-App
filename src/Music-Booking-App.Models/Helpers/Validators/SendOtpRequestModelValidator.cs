using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class SendOtpRequestModelValidator : AbstractValidator<SendOtpRequestModel>
    {
        public SendOtpRequestModelValidator()
        {
            RuleFor(model => model.Email)
           .NotNull().WithMessage("Email address cannot be null.")
           .NotEmpty().WithMessage("Email address is required.")
           .Length(5, 50).WithMessage("Email address must be within 5 to 50 characters.")
           .EmailAddress().WithMessage("Invalid email address format.");
        }
    }
}
