using Microsoft.AspNetCore.Mvc;
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public EventController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent(CreateEventRequestModel model)
        {
            var response = await _bookingService.CreateEventAsync(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPut("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent(UpdateEventRequestModel requestModel)
        {
            var response = await _bookingService.UpdateEventAsync(requestModel, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents(AccountStatus status, int pageSize, int pageNumber, string? searchParam)
        {
            var response = await _bookingService.GetAllEvents(status, pageSize, pageNumber, searchParam);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetEvent")]
        public async Task<IActionResult> GetEvent(string id)
        {
            var response = await _bookingService.GetEvent(id);
            return HttpResponseHelper.GetHttpResponse(response);
        }







    }
}
