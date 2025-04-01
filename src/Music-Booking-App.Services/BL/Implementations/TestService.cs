
using Music_Booking_App.Core.Constants;
using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.BL.Interfaces;
using Music_Booking_App.Services.Helpers;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Music_Booking_App.Services.BL.Implementations
{
    public class TestService : ITestService
    {
        private readonly IDapperCommandRepository<Test> _testCommandRepository;
        private readonly IDapperQueryRepository<Test> _testQueryRepository;
        private readonly ITestValidator _testValidator;
        private readonly IMapper _mapper;
        // private readonly IAuthenticationService _auth;
        //private readonly IDocumentService _doc;
        private readonly IConfiguration _config;
        //private readonly INotificationService _notify;

        public TestService(IDapperCommandRepository<Test> testCommandRepository,
                           IDapperQueryRepository<Test> testQueryRepository,
                           ITestValidator testValidator,
                           IMapper mapper,
                           /*IAuthenticationService auth,*/
                           /*IDocumentService doc,*/
                           IConfiguration config
                           /*INotificationService notify*/)
        {
            _testCommandRepository = testCommandRepository;
            _testQueryRepository = testQueryRepository;
            _testValidator = testValidator;
            _mapper = mapper;
            //_auth = auth;
            //_doc = doc;
            _config = config;
            //_notify = notify;
        }

        public async Task<BaseResponse<TestViewModel>> CreateTestAsync(CreateTestRequestModel requestModel)
        {
            Log.Information(
                 $"Starting test creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _testValidator.ValidateCreateTestRequestAsync(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<TestViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            var criteria = new Dictionary<string, string> { { nameof(Test.Name), requestModel.Name } };
            var queryResult = await _testQueryRepository
                .GetByAsync(criteria, int.MaxValue, 1);

            /* var otpName = new SendOtpRequestModel
             {
                 Email = requestModel.Name += "2@yopmail.com"
             };

             var endpoint = await _auth.SendOtp(otpName);*/


            var existingTests = queryResult.ToList();
            if (existingTests.Count != 0)
                return BaseResponse<TestViewModel>
                    .Failure(ResponseMessages.DuplicateName, StatusCodes.DuplicateRecord);

            try
            {
                Log.Information(
                 $"Sending test creation request: {JsonConvert.SerializeObject(requestModel)}");


                /* var url = _config.GetValue<string>("Base64");
                 var call = _doc.UploadDocumentViaRest("JohnT2", ".jpg", url, "test2");*/

                //var call = _notify.SendVerificationMailAsync("tagtest@yopmail.com", "18ac285b-3164-4f15-b54d-1bc84b11e27a");
                //var call = _notify.SendEmailAsync("tagtest@yopmail.com", EmailTemplates.VerifyShareholder.ToString(), "18ac285b-3164-4f15-b54d-1bc84b11e27a");

                var newTest = _mapper.Map<CreateTestRequestModel, Test>(requestModel);
                await _testCommandRepository.AddAsync(newTest);
                Log.Information($"Successfully saved test.");

                var testView = _mapper.Map<Test, TestViewModel>(newTest);
                var response = BaseResponse<TestViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, testView);
                return response;

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<TestViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<PaginatedResponse<List<TestViewModel>>> GetAllTestAsync(int pageSize, int pageNumber)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            try
            {

                var allTests = await _testQueryRepository.GetAllAsync(pageSize, pageNumber);

                if (!allTests.Any())
                    return PaginatedResponse<List<TestViewModel>>
                                                 .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);

                var totalCount = await _testQueryRepository.GetCountAsync();

                Log.Information(
                 $"Successfully retrieved all tests: {JsonConvert.SerializeObject(allTests)}");

                return PaginatedResponse<List<TestViewModel>>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                    _mapper.Map<List<TestViewModel>>(allTests), totalCount);

            }

            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return PaginatedResponse<List<TestViewModel>>
                                        .Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<TestViewModel>> GetTestByIdAsync(string id)
        {
            var validationResult = _testValidator.ValidateTestId(id);
            if (!validationResult.IsValid)
                return BaseResponse<TestViewModel>
                    .Failure(validationResult.Message, StatusCodes.ModelValidationError);

            try
            {
                Log.Information(
                 $"Sending get test by id request: {JsonConvert.SerializeObject(id)}");

                var test = await _testQueryRepository.FindByIdAsync(Guid.Parse(id));

                if (test == null)
                    return BaseResponse<TestViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);

                Log.Information(
                $"Successfully retrieved test by id: {JsonConvert.SerializeObject(id)}");

                var testView = _mapper.Map<TestViewModel>(test);

                var response = BaseResponse<TestViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, testView);

                return response;

            }

            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<TestViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<TestViewModel>> UpdateTestAsync(UpdateTestRequestModel requestModel)
        {
            Log.Information(
                 $"Starting test update process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _testValidator.ValidateUpdateTestRequestAsync(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<TestViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                Log.Information(
                 $"Sending test update request: {JsonConvert.SerializeObject(requestModel)}");

                var existingTestRecord = await _testQueryRepository.FindByIdAsync(requestModel.Id);
                if (existingTestRecord == null)
                    return BaseResponse<TestViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);

                existingTestRecord.Name = requestModel.Name;
                existingTestRecord.LastUpdateDate = DateTime.UtcNow;

                await _testCommandRepository.UpdateAsync(existingTestRecord);

                Log.Information($"Successfully updated test.");

                var updatedTestView = _mapper.Map<TestViewModel>(existingTestRecord);

                var response = BaseResponse<TestViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, updatedTestView);

                return response;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<TestViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }

        }

        public async Task<BaseResponse<TestViewModel>> DeleteTestAsync(string id)
        {
            Log.Information(
                $"Starting test delete process for test with Id: {JsonConvert.SerializeObject(id)}");

            var validationResult = _testValidator.ValidateTestId(id);
            if (!validationResult.IsValid)
                return BaseResponse<TestViewModel>
                    .Failure(validationResult.Message, StatusCodes.ModelValidationError);

            try
            {
                Log.Information(
                $"Sending test delete request: {JsonConvert.SerializeObject(id)}");

                var test = await _testQueryRepository.FindByIdAsync(Guid.Parse(id));

                if (test == null)
                    return BaseResponse<TestViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);

                Log.Information(
                $"Successfully retrieved test: {JsonConvert.SerializeObject(id)}");

                _testCommandRepository.Delete(Guid.Parse(id));

                return BaseResponse<TestViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }

            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<TestViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }

        }
    }
}
