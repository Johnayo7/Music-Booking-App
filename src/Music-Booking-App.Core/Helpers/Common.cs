
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Music_Booking_App.Core.Helpers
{
    public class Common : ICommon
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Common(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///     Asynchronously make a http call to an endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> MakeHttpRequest(object request, string baseAddress, string requestUri,
            HttpMethod method, Dictionary<string, string> headers = null)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(15);

            if (headers != null)
                foreach (var header in headers)
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

            if (method != HttpMethod.Post) return await httpClient.GetAsync(requestUri);
            var data = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(requestUri, content);

        }
        public string GetJsonString(object data)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            return json;
        }

        public async Task<ApiResponse> CallApi(string baseUrl, string uri, string data, string mediaType,
            string method = "POST", Dictionary<string, string> headers = null)
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            if (headers is not null)
                foreach (var header in headers)
                    http.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);


            var apiResponse = new ApiResponse();
            HttpResponseMessage httpResponseMessage = null;
            if (method == "POST")
                httpResponseMessage = await http.PostAsync(uri, new StringContent(data, Encoding.UTF8, mediaType));
            if (method == "GET") httpResponseMessage = await http.GetAsync(uri);

            if (httpResponseMessage == null) return apiResponse;
            apiResponse.StatusCode = httpResponseMessage.StatusCode.ToString();
            apiResponse.Result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            apiResponse.IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            apiResponse.ReasonPhrase = httpResponseMessage.ReasonPhrase;

            return apiResponse;
        }

        public string GetLocalIpAddress()
        {
            var hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);
            var ipv4Address = Array.Find(ipAddresses,
                address => address.AddressFamily == AddressFamily.InterNetwork);
            return ipv4Address.ToString();
        }
    }
}
