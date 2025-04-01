

using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;
using FluentValidation;
using FluentValidation.Results;

namespace Music_Booking_App.Services.Helpers
{
    public class AuthenticationValidator : IAuthenticationValidator
    {
        private readonly IValidator<SendDefaultPassRequest> _defaultPassRequestValidator;
        private readonly IValidator<SignUpRequestModel> _signUpRequestValidator;
        private readonly IValidator<LoginRequestModel> _loginRequestValidator;
        private readonly IValidator<ResetPasswordRequestModel> _resetPasswordRequestValidator;
        private readonly IValidator<ChangePasswordRequestModel> _changePasswordRequestValidator;
        private readonly IValidator<SendOtpRequestModel> _sendOtpRequestValidator;
        private readonly IValidator<VerifyOtpRequestModel> _verifyOtpRequestValidator;



        public AuthenticationValidator(IValidator<SendDefaultPassRequest> defaultPassRequestValidator,
                                       IValidator<SignUpRequestModel> signUpRequestValidator,
                                       IValidator<LoginRequestModel> loginRequestValidator,
                                       IValidator<ResetPasswordRequestModel> resetPasswordRequestValidator,
                                       IValidator<ChangePasswordRequestModel> changePasswordRequestValidator,
                                       IValidator<SendOtpRequestModel> sendOtpRequestValidator,
                                       IValidator<VerifyOtpRequestModel> verifyOtpRequestValidator)
        {
            _defaultPassRequestValidator = defaultPassRequestValidator;
            _signUpRequestValidator = signUpRequestValidator;
            _loginRequestValidator = loginRequestValidator;
            _resetPasswordRequestValidator = resetPasswordRequestValidator;
            _changePasswordRequestValidator = changePasswordRequestValidator;
            _sendOtpRequestValidator = sendOtpRequestValidator;
            _verifyOtpRequestValidator = verifyOtpRequestValidator;
        }

        public async Task<ValidationResult> ValidateDefaultPassRequestAsync(SendDefaultPassRequest model)
        {
            return await _defaultPassRequestValidator.ValidateAsync(model);
        }
        public async Task<ValidationResult> ValidateSignUpRequestAsync(SignUpRequestModel model)
        {
            return await _signUpRequestValidator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateLoginRequestAsync(LoginRequestModel model)
        {
            return await _loginRequestValidator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateResetPasswordRequestAsync(ResetPasswordRequestModel model)
        {
            return await _resetPasswordRequestValidator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateChangePasswordRequestAsync(ChangePasswordRequestModel model)
        {
            return await _changePasswordRequestValidator.ValidateAsync(model);

        }

        public async Task<ValidationResult> ValidateSendOtpRequestAsync(SendOtpRequestModel model)
        {
            return await _sendOtpRequestValidator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateOtpRequestAsync(VerifyOtpRequestModel model)
        {
            return await _verifyOtpRequestValidator.ValidateAsync(model);
        }

        public ValidationResultDto ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return ValidationResultDto.Failure("Token is required.");
            return ValidationResultDto.Success();
        }

    }
}
