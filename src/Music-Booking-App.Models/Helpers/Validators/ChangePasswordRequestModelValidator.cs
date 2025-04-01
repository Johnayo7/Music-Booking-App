

using Music_Booking_App.Models.RequestModels;
using FluentValidation;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class ChangePasswordRequestModelValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordRequestModelValidator()
        {
            RuleFor(model => model.CurrentPassword)
                   .NotNull()
                   .NotEmpty()
                   .MinimumLength(8)
                   .WithMessage("{PropertyName} must be at least 8 characters long.")
                   .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
                   .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                   .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                   .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one non-alphanumeric character.");

            RuleFor(model => model.NewPassword)
                    .NotNull()
                    .NotEmpty()
                    .MinimumLength(8)
                    .WithMessage("{PropertyName} must be at least 8 characters long.")
                    .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
                    .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                    .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                    .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one non-alphanumeric character.");

            RuleFor(model => model.ConfirmPassword)
                   .NotNull()
                   .NotEmpty()
                   .MinimumLength(8)
                   .WithMessage("{PropertyName} must be at least 8 characters long.")
                   .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
                   .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                   .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                   .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one non-alphanumeric character.")
                   .Equal(model => model.NewPassword).WithMessage("The password and confirmation password do not match.");
        }
    }
}
