using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class TicketPurchaseRequestModelValidator : AbstractValidator<TicketPurchaseRequestModel>
    {
        public TicketPurchaseRequestModelValidator()
        {
            RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.NoOfTickets)
              .GreaterThan(0).WithMessage("Number of tickets must be at least 1.");
        }
    }
}
