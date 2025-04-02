using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;
using System.Security.Claims;

namespace Music_Booking_App.Services.BL.Interfaces
{
    public interface IBookingService
    {
        Task<BaseResponse<CreationViewModel>> CreateArtisteProfileAsync(CreateArtisteRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<BaseResponse<CreationViewModel>> CreateEventAsync(CreateEventRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<PaginatedResponse<List<ArtistesViewModel>>> GetAllArtistes(AccountStatus status, int pageSize, int pageNumber, string searchParam = null);
        Task<PaginatedResponse<List<EventsViewModel>>> GetAllEvents(AccountStatus status, int pageSize, int pageNumber, string searchParam = null);
        Task<BaseResponse<ArtisteDetailViewModel>> GetArtiste(string id);
        Task<BaseResponse<EventDetailsViewModel>> GetEvent(string id);
        Task<BaseResponse<ArtisteDetailViewModel>> UpdateArtisteProfileAsync(UpdateArtisteRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<BaseResponse<EventDetailsViewModel>> UpdateEventAsync(UpdateEventRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<BaseResponse<CreationViewModel>> IniatiateBookingAsync(BookingRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<BaseResponse<BookingsViewModel>> UpdateBookingAsync(UpdateBookingRequestModel requestModel, ClaimsPrincipal userClaims);
        Task<PaginatedResponse<List<BookingsViewModel>>> GetBookingsByRole(ClaimsPrincipal userClaims, AccountStatus status, int pageSize, int pageNumber, string searchParam = null);
        Task<BaseResponse<ApprovalReviewViewModel>> ReviewBookingRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims);
        Task<BaseResponse<ApprovalReviewViewModel>> ReviewCreatedArtisteProfileRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims);
        Task<BaseResponse<ApprovalReviewViewModel>> ReviewCreatedEventRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims);

        Task<BaseResponse<CreationViewModel>> BookingPayment(string bookingId, ClaimsPrincipal userClaims);
        Task<BaseResponse<CreationViewModel>> TicketPurchase(TicketPurchaseRequestModel request, ClaimsPrincipal userClaims);
        Task<BaseResponse<CreationViewModel>> VerifyPayment(string referenceId, PaymentType paymentType, ClaimsPrincipal userClaims);
        Task<PaginatedResponse<List<BookingPaymentViewModel>>> GetBookingPaymentByRole(ClaimsPrincipal userClaims, PaymentStatus status, int pageSize, int pageNumber, string searchParam = null);
        Task<PaginatedResponse<List<TicketPurchaseViewModel>>> GetTicketPurchaseByRole(ClaimsPrincipal userClaims, PaymentStatus status, int pageSize, int pageNumber, string searchParam = null);
    }
}
