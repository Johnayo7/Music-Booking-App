using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.BL.ExternalProviders.Paystack.DTOs;
using PayStack.Net;

namespace Music_Booking_App.Services.BL.ExternalProviders.Paystack
{
    public interface IPaystackService
    {
        Task<PaystackResponseViewModel> PaymentInitialization(PaymentRequestDto request);
        Task<TransactionVerifyResponse> PaymentVerify(string referenceId);
    }
}
