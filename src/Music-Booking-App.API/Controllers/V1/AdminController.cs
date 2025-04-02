using Microsoft.AspNetCore.Mvc;
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public AdminController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("ReviewCreatedArtisteProfile")]
        public async Task<IActionResult> ReviewCreatedArtisteProfile(ApprovalReviewRequestModel model)
        {
            var response = await _bookingService.ReviewCreatedArtisteProfileRequest(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("ReviewCreatedEvent")]
        public async Task<IActionResult> ReviewCreatedEvent(ApprovalReviewRequestModel model)
        {
            var response = await _bookingService.ReviewCreatedEventRequest(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

    }
}
