using FluentValidation;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Models.Helpers.Validators
{
    public class ApprovalReviewRequestValidator : AbstractValidator<ApprovalReviewRequestModel>
    {
        public ApprovalReviewRequestValidator()
        {
            RuleFor(x => x.Id)
           .NotEmpty().WithMessage("OrganizationId is required.");

            RuleFor(x => x.ReviewStatus)
                 .IsInEnum().WithMessage("status must be a valid review stage.");
        }
    }
}
