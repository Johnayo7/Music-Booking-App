using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequestModel>
    {
        public CreateEventRequestValidator()
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

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .Length(2, 200).WithMessage("Location must be between 2 and 200 characters.");

            RuleFor(x => x.Date)
                .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.");

            RuleFor(x => x.TicketPrice)
                .GreaterThan(0).WithMessage("Ticket price must be greater than zero.");

        }
    }
}
