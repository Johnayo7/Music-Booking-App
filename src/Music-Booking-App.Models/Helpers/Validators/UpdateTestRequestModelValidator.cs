

using Music_Booking_App.Models.RequestModels;
using FluentValidation;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class UpdateTestRequestModelValidator : AbstractValidator<UpdateTestRequestModel>
    {

        public UpdateTestRequestModelValidator()
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
        }
    }
}
