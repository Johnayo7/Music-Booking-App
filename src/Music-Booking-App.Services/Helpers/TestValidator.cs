

using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Models.RequestModels;
using FluentValidation;
using FluentValidation.Results;

namespace Music_Booking_App.Services.Helpers
{
    public class TestValidator : ITestValidator
    {
        private readonly IValidator<CreateTestRequestModel> _createTestRequestValidator;
        private readonly IValidator<UpdateTestRequestModel> _updateTestRequestValidator;

        public TestValidator(IValidator<CreateTestRequestModel> createTestRequestValidator, IValidator<UpdateTestRequestModel> updateTestRequestValidator)
        {
            _createTestRequestValidator = createTestRequestValidator;
            _updateTestRequestValidator = updateTestRequestValidator;
        }

        public async Task<ValidationResult> ValidateCreateTestRequestAsync(CreateTestRequestModel model)
        {
            return await _createTestRequestValidator.ValidateAsync(model);
        }

        public async Task<ValidationResult> ValidateUpdateTestRequestAsync(UpdateTestRequestModel model)
        {
            return await _updateTestRequestValidator.ValidateAsync(model);
        }

        public ValidationResultDto ValidateTestId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ValidationResultDto.Failure("Id is required.");

            if (!Guid.TryParse(id, out _))
                return ValidationResultDto.Failure("Id must be a valid Guid value.");

            return ValidationResultDto.Success();

        }
    }
}
