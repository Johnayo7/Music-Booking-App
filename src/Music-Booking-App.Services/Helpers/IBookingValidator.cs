using FluentValidation.Results;
using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Services.Helpers
{
    public interface IBookingValidator
    {
        Task<ValidationResult> ValidateCreateArtisteProfileRequest(CreateArtisteRequestModel model);
        Task<ValidationResult> ValidateCreateEventRequest(CreateEventRequestModel model);
        Task<ValidationResult> ValidateBookingRequest(BookingRequestModel model);
        Task<ValidationResult> ValidateApprovalRequest(ApprovalReviewRequestModel model);


        public ValidationResultDto ValidateGuid(string id);
    }
}
