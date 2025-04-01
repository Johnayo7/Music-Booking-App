using Music_Booking_App.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using StatusCodes = Music_Booking_App.Core.Constants.StatusCodes;

namespace Music_Booking_App.API.Helpers
{
    public class HttpResponseHelper
    {
        public static IActionResult GetHttpResponse<T>(BaseResponse<T> internalResponse) where T : class
        {
            var internalServerError = new ObjectResult(internalResponse)
            {
                StatusCode = 500
            };

            return internalResponse.StatusCode switch
            {
                StatusCodes.Successful => new OkObjectResult(internalResponse),
                StatusCodes.ModelValidationError => new BadRequestObjectResult(internalResponse),
                StatusCodes.NoRecordFound => new NotFoundObjectResult(internalResponse),
                StatusCodes.BadRequest => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.BadRequest },
                StatusCodes.DuplicateRecord => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Conflict },
                StatusCodes.UnAuthorized => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Unauthorized },
                StatusCodes.Forbidden => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Forbidden },
                StatusCodes.TooManyRequests => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.TooManyRequests },
                StatusCodes.Locked => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Locked },
                StatusCodes.TimeoutOrExpired => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.RequestTimeout },

                _ => internalServerError
            };
        }

        public static IActionResult GetHttpResponse(BaseResponse internalResponse)
        {
            var internalServerError = new ObjectResult(internalResponse)
            {
                StatusCode = 500
            };

            return internalResponse.StatusCode switch
            {
                StatusCodes.Successful => new OkObjectResult(internalResponse),
                StatusCodes.ModelValidationError => new BadRequestObjectResult(internalResponse),
                StatusCodes.NoRecordFound => new NotFoundObjectResult(internalResponse),
                StatusCodes.BadRequest => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.BadRequest },
                StatusCodes.DuplicateRecord => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Conflict },
                StatusCodes.UnAuthorized => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Unauthorized },
                StatusCodes.Forbidden => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Forbidden },
                StatusCodes.TooManyRequests => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.TooManyRequests },
                StatusCodes.Locked => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.Locked },
                StatusCodes.TimeoutOrExpired => new ObjectResult(internalResponse) { StatusCode = (int)HttpStatusCode.RequestTimeout },
                _ => internalServerError
            };
        }
    }
}
