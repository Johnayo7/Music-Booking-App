
using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;
using FluentValidation.Results;

namespace Music_Booking_App.Services.Helpers
{
    public interface IAuthenticationValidator
    {
        Task<ValidationResult> ValidateDefaultPassRequestAsync(SendDefaultPassRequest model);
        Task<ValidationResult> ValidateSignUpRequestAsync(SignUpRequestModel model);
        Task<ValidationResult> ValidateLoginRequestAsync(LoginRequestModel model);
        Task<ValidationResult> ValidateResetPasswordRequestAsync(ResetPasswordRequestModel model);
        Task<ValidationResult> ValidateChangePasswordRequestAsync(ChangePasswordRequestModel model);
        Task<ValidationResult> ValidateSendOtpRequestAsync(SendOtpRequestModel model);
        Task<ValidationResult> ValidateOtpRequestAsync(VerifyOtpRequestModel model);


        ValidationResultDto ValidateToken(string token);
    }
}
