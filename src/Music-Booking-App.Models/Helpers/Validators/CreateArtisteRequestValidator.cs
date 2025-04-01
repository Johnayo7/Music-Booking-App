using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class CreateArtisteRequestValidator : AbstractValidator<CreateArtisteRequestModel>
    {
        public CreateArtisteRequestValidator()
        {
            RuleFor(model => model.Name)
              .NotNull()
              .NotEmpty()
              .Length(2, 20)
              .WithMessage("{PropertyName} must be within 5 to 20 characters")
              .Matches("^[a-zA-Z0-9_-]*$") // Allows alphanumeric characters, hyphens, and underscores
              .WithMessage("{PropertyName} cannot contain invalid characters.")
              .Must(name =>
              {
                  var words = name.Split(' ');
                  return words.Length <= 3 && words.All(word => char.IsUpper(word[0]) && word.Skip(1).All(char.IsLower));
              })
              .WithMessage("{PropertyName} must start with an uppercase letter for each word and contain a maximum of three words.");

            RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .Length(2, 50).WithMessage("Genre must be between 2 and 50 characters.");

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required.")
                .Length(10, 500).WithMessage("Bio must be between 10 and 500 characters.");

            RuleFor(x => x.BookingRate)
                .GreaterThan(0).WithMessage("Booking rate must be greater than zero.");

        }
    }
}
