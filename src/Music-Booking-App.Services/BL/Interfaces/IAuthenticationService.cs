

using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;
using System.Security.Claims;

namespace Music_Booking_App.Services.BL.Interfaces
{
    public interface IAuthenticationService
    {
        Task<BaseResponse<OtpViewModel>> SendOtp(SendOtpRequestModel request);
        Task<BaseResponse<OtpViewModel>> VerifyOtp(VerifyOtpRequestModel request);
        Task<BaseResponse<SignUpViewModel>> SignUp(SignUpRequestModel request);
        Task<BaseResponse<LoginViewModel>> Login(LoginRequestModel request);
        Task<BaseResponse<PasswordViewModel>> ResetPassword(ResetPasswordRequestModel request);
        Task<BaseResponse<PasswordViewModel>> ChangePassword(ChangePasswordRequestModel request, ClaimsPrincipal userClaims);
        Task<BaseResponse<LogoutViewModel>> Logout();

        //Task<BaseResponse<DefaultPassViewModel>> SendDefaultPassword(SendDefaultPassRequest request);

    }
}
