
using Music_Booking_App.Models.RequestModels;
using FluentValidation;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class SignUpRequestModelValidator : AbstractValidator<SignUpRequestModel>
    {
        public SignUpRequestModelValidator()
        {
            RuleFor(model => model.Name)
                    .NotNull()
                    .NotEmpty()
                    .Length(2, 100)
                    .WithMessage("{PropertyName} must be within 2 to 50 characters")
                    .Matches("^[a-zA-Z0-9 _-]*$") // Allows alphanumeric characters, spaces, hyphens, and underscores
                    .WithMessage("{PropertyName} cannot contain invalid characters.")
                    .Must(name =>
                    {
                        var words = name.Split(' ');
                        return words.Length <= 2 && words.All(word => char.IsUpper(word[0]) && word.Skip(1).All(char.IsLower));
                    })
                    .WithMessage("{PropertyName} must start with an uppercase letter for each word and contain a maximum of three words.");

            RuleFor(model => model.Email)
                    .NotNull().WithMessage("Email address cannot be null.")
                    .NotEmpty().WithMessage("Email address is required.")
                    .Length(5, 50).WithMessage("Email address must be within 5 to 50 characters.")
                    .EmailAddress().WithMessage("Invalid email address format.");
            /* RuleFor(model => model.DefaultPassword)
                     .NotNull()
                     .NotEmpty()
                     .MinimumLength(8)
                     .WithMessage("{PropertyName} must be at least 8 characters long.")
                     .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
                     .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                     .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                     .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one non-alphanumeric character.");
 */
            RuleFor(model => model.Password)
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
                   .Equal(model => model.Password).WithMessage("The password and confirmation password do not match.");
        }
    }
}
