

namespace Music_Booking_App.Core.Helpers
{
    public interface ICommon : IAutoDependencyCore
    {
        Task<HttpResponseMessage> MakeHttpRequest(object request, string baseAddress, string requestUri, HttpMethod method,
            Dictionary<string, string> headers = null);

        string GetJsonString(object data);

        Task<ApiResponse> CallApi(string baseUrl, string uri, string data, string mediaType, string method = "POST",
            Dictionary<string, string> headers = null);

        string GetLocalIpAddress();
    }
}
