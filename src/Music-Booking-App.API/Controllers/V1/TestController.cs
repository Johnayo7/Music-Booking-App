using Music_Booking_App.API.Helpers;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpPost("CreateTest")]
        public async Task<IActionResult> CreateTest(CreateTestRequestModel model)
        {
            var response = await _testService.CreateTestAsync(model);
            if (response.StatusCode == Core.Constants.StatusCodes.Successful)
                return new ObjectResult(response) { StatusCode = 201 };
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("GetAllTests")]
        public async Task<IActionResult> GetAllTests(int pageSize, int pageNumber)
        {
            var response = await _testService.GetAllTestAsync(pageSize, pageNumber);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(string id)
        {
            var response = await _testService.GetTestByIdAsync(id);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(Guid id, UpdateTestRequestModel model)
        {
            model.Id = id;
            var response = await _testService.UpdateTestAsync(model);
            return HttpResponseHelper.GetHttpResponse(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(string id)
        {
            var response = await _testService.DeleteTestAsync(id);
            return HttpResponseHelper.GetHttpResponse(response);
        }
    }
}
