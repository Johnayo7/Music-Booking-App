using Microsoft.AspNetCore.Mvc;
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public PaymentController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("BookingPayment")]
        public async Task<IActionResult> BookingPayment(string bookingId)
        {
            var response = await _bookingService.BookingPayment(bookingId, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPost("TicketPurchase")]
        public async Task<IActionResult> TicketPurchase(TicketPurchaseRequestModel model)
        {
            var response = await _bookingService.TicketPurchase(model, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("VerifyPayment")]
        public async Task<IActionResult> VerifyPayment(string referenceId, PaymentType paymentType)
        {
            var response = await _bookingService.VerifyPayment(referenceId, paymentType, User);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetBookingPaymentByRole")]
        public async Task<IActionResult> GetBookingPaymentByRole(PaymentStatus status, int pageSize, int pageNumber, string? searchParam)
        {
            var response = await _bookingService.GetBookingPaymentByRole(User, status, pageSize, pageNumber, searchParam);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetTicketPurchaseByRole")]
        public async Task<IActionResult> GetTicketPurchaseByRole(PaymentStatus status, int pageSize, int pageNumber, string? searchParam)
        {
            var response = await _bookingService.GetTicketPurchaseByRole(User, status, pageSize, pageNumber, searchParam);
            return HttpResponseHelper.GetHttpResponse(response);
        }
    }
}
