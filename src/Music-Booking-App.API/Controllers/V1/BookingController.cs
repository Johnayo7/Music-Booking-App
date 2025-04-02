using Microsoft.AspNetCore.Mvc;
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("IniatiateBooking")]
        public async Task<IActionResult> IniatiateBooking(BookingRequestModel model)
        {
            var response = await _bookingService.IniatiateBookingAsync(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPut("UpdateBooking")]
        public async Task<IActionResult> UpdateBooking(UpdateBookingRequestModel requestModel)
        {
            var response = await _bookingService.UpdateBookingAsync(requestModel, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetBookingsByRole")]
        public async Task<IActionResult> GetBookingsByRole(AccountStatus status, int pageSize, int pageNumber, string? searchParam)
        {
            var response = await _bookingService.GetBookingsByRole(User, status, pageSize, pageNumber, searchParam);
            return HttpResponseHelper.GetHttpResponse(response);
        }
    }
}
