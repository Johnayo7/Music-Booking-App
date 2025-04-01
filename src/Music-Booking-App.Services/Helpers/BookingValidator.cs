using FluentValidation;
using FluentValidation.Results;
using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;

namespace Music_Booking_App.Services.Helpers
{
    public class BookingValidator : IBookingValidator
    {
        private readonly IValidator<CreateArtisteRequestModel> _createArtisteRequestValidator;
        private readonly IValidator<CreateEventRequestModel> _createEventRequestValidator;

        public BookingValidator(IValidator<CreateArtisteRequestModel> createartisteRequestValidator, IValidator<CreateEventRequestModel> createEventRequestValidator)
        {
            _createArtisteRequestValidator = createartisteRequestValidator;
            _createEventRequestValidator = createEventRequestValidator;
        }
        public async Task<ValidationResult> ValidateCreateArtisteProfileRequest(CreateArtisteRequestModel model)
           => await _createArtisteRequestValidator.ValidateAsync(model);

        public async Task<ValidationResult> ValidateCreateEventRequest(CreateEventRequestModel model)
           => await _createEventRequestValidator.ValidateAsync(model);

        public ValidationResultDto ValidateGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ValidationResultDto.Failure("Id is required.");

            if (!Guid.TryParse(id, out _))
                return ValidationResultDto.Failure("Id must be a valid Guid value.");

            return ValidationResultDto.Success();

        }
    }
}
