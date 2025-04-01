

namespace Music_Booking_App.Models.ViewModels
{
    public class BaseResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

        public static BaseResponse Success(string message, string statusCode)
        {
            return new BaseResponse
            {
                Status = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static BaseResponse Failure(string message, string statusCode)
        {
            return new BaseResponse
            {
                Status = false,
                Message = message,
                StatusCode = statusCode
            };
        }

    }

    public class BaseResponse<T> where T : class
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public T Data { get; set; }

        public static BaseResponse<T> Success(string message, string statusCode, T data)
        {
            return new BaseResponse<T>
            {
                Status = true,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }

        public static BaseResponse<T> Failure(string message, string statusCode)
        {
            return new BaseResponse<T>
            {
                Status = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
