using Music_Booking_App.Core.Constants;
using Music_Booking_App.Models.ViewModels;
using Newtonsoft.Json;
using Npgsql;
using Serilog;
using System.Net;
using StatusCodes = Music_Booking_App.Core.Constants.StatusCodes;

namespace Music_Booking_App.API.Helpers
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"DB Error: {ex.Message}\n{ex.StackTrace}");
                await HandleExceptionAsync(httpContext, StatusCodes.SqlException);
            }
            catch (Exception ex)
            {
                Log.Error($"General Error: {ex.Message}\n{ex.StackTrace}");
                await HandleExceptionAsync(httpContext, StatusCodes.GeneralError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, string statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = BaseResponse.Failure(ResponseMessages.GeneralError, statusCode);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
