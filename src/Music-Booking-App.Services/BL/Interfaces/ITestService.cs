
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;

namespace Music_Booking_App.Services.BL.Interfaces
{
    public interface ITestService
    {
        Task<BaseResponse<TestViewModel>> CreateTestAsync(CreateTestRequestModel requestModel);
        Task<PaginatedResponse<List<TestViewModel>>> GetAllTestAsync(int pageSize, int pageNumber);
        Task<BaseResponse<TestViewModel>> GetTestByIdAsync(string id);
        Task<BaseResponse<TestViewModel>> UpdateTestAsync(UpdateTestRequestModel requestModel);
        Task<BaseResponse<TestViewModel>> DeleteTestAsync(string id);
    }
}
