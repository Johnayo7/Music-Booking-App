
namespace Music_Booking_App.Models.ViewModels
{
    public class PaginatedResponse<T> : BaseResponse<T> where T : class
    {
        public long TotalCount { get; set; }

        public new static PaginatedResponse<T> Success(string message, string statusCode, T data, long totalCount)
        {
            return new PaginatedResponse<T>
            {
                Status = true,
                Message = message,
                StatusCode = statusCode,
                Data = data,
                TotalCount = totalCount
            };
        }

        public new static PaginatedResponse<T> Failure(string message, string statusCode)
        {
            return new PaginatedResponse<T>
            {
                Status = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
