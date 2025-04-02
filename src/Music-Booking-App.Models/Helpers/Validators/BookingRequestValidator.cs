using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class BookingRequestValidator : AbstractValidator<BookingRequestModel>
    {
        public BookingRequestValidator()
        {
            RuleFor(x => x.ArtisteId)
            .NotEmpty().WithMessage("Artist ID is required.")
            .Must(BeAValidGuid).WithMessage("Artist ID must be a valid GUID.");

            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event ID is required.")
                .Must(BeAValidGuid).WithMessage("Event ID must be a valid GUID.");

            RuleFor(x => x.ProposedAmount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }

        private bool BeAValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }
    }
}

