using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp(SendOtpRequestModel model)
        {
            var response = await _authService.SendOtp(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestModel model)
        {
            var response = await _authService.VerifyOtp(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequestModel model)
        {
            var response = await _authService.SignUp(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var response = await _authService.Login(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestModel model)
        {
            var response = await _authService.ResetPassword(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel model)
        {
            var response = await _authService.ChangePassword(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _authService.Logout();
            return HttpResponseHelper.GetHttpResponse(response);
        }


        /*[HttpPost("SendDefaultPassword")]
        public async Task<IActionResult> SendDefaultPassword(SendDefaultPassRequest model)
        {
            var response = await _authService.SendDefaultPassword(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }*/
    }
}
