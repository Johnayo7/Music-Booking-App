using Microsoft.AspNetCore.Mvc;
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/artiste")]
    [ApiController]
    public class ArtisteController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public ArtisteController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("CreateArtisteProfile")]
        public async Task<IActionResult> CreateArtisteProfile(CreateArtisteRequestModel model)
        {
            var response = await _bookingService.CreateArtisteProfileAsync(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPut("UpdateArtisteProfile")]
        public async Task<IActionResult> UpdateArtisteProfile(UpdateArtisteRequestModel requestModel)
        {
            var response = await _bookingService.UpdateArtisteProfileAsync(requestModel, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetAllArtistes")]
        public async Task<IActionResult> GetAllArtistes(AccountStatus status, int pageSize, int pageNumber, string? searchParam)
        {
            var response = await _bookingService.GetAllArtistes(status, pageSize, pageNumber, searchParam);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetArtiste")]
        public async Task<IActionResult> GetArtiste(string id)
        {
            var response = await _bookingService.GetArtiste(id);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("ReviewBookingRequest")]
        public async Task<IActionResult> ReviewBookingRequest(ApprovalReviewRequestModel model)
        {
            var response = await _bookingService.ReviewBookingRequest(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }


    }
}
