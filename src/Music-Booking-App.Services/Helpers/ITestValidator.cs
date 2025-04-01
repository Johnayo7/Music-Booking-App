
using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;
using FluentValidation.Results;

namespace Music_Booking_App.Services.Helpers
{
    public interface ITestValidator
    {
        Task<ValidationResult> ValidateCreateTestRequestAsync(CreateTestRequestModel model);
        Task<ValidationResult> ValidateUpdateTestRequestAsync(UpdateTestRequestModel model);
        ValidationResultDto ValidateTestId(string id);
    }
}
