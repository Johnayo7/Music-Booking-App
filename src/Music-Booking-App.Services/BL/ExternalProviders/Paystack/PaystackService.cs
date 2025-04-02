using Microsoft.Extensions.Options;
using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.BL.ExternalProviders.Paystack.DTOs;
using PayStack.Net;
using Serilog;

namespace Music_Booking_App.Services.BL.ExternalProviders.Paystack
{
    public class PaystackService : IPaystackService
    {
        private readonly PaystackSettings _paystackSettings;
        private readonly PayStackApi _paystackApi;

        public PaystackService(IOptions<PaystackSettings> paystackSettings, PayStackApi paystackApi)
        {
            _paystackSettings = paystackSettings.Value;
            _paystackApi = new PayStackApi(_paystackSettings.SecretKey);
        }

        public async Task<PaystackResponseViewModel> PaymentInitialization(PaymentRequestDto request)
        {
            TransactionInitializeRequest payload = new()
            {
                AmountInKobo = (int)((request.Amount * 100)),
                Email = request.Email,
                Reference = Guid.NewGuid().ToString(),
                Currency = "NGN",
                CallbackUrl = $"{_paystackSettings.AppUrl}/paystack/callback"
            };

            var response = _paystackApi.Transactions.Initialize(payload);
            if (!response.Status)
            {
                Log.Error("Paystack Initialization: {@response}", response);
                return new PaystackResponseViewModel
                {
                    Messsage = "Failed to initialize payment"
                };
            }

            return await Task.FromResult(new PaystackResponseViewModel
            {
                Reference = response.Data.Reference,
                AuthorizationUrl = response.Data.AuthorizationUrl,
                Messsage = "Payment Initiated Successfully",
                IsSuccess = true
            });
        }

        public async Task<TransactionVerifyResponse> PaymentVerify(string referenceId)
        {
            return await Task.FromResult(_paystackApi.Transactions.Verify(referenceId));
        }
    }
}
