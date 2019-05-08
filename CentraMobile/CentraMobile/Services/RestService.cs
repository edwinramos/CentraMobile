using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CentraMobile.DataLayer;
using CentraMobile.Utils;
using Newtonsoft.Json;
//using SQLite;
using Xamarin.Forms;

namespace CentraMobile.Services
{
    public class RestService
    {
        private readonly HttpClient _client;

        public RestService(string serverAddress)
        {
            _client = new HttpClient
            {
                Timeout = new TimeSpan(0,0,15),
                MaxResponseContentBufferSize = 256000,
                BaseAddress = new Uri(serverAddress)
            };
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("'application/x-www-form-urlencoded'"));
            _client.MaxResponseContentBufferSize = 30000000;
        }

        public async Task<bool> PostResponse<T>(string webUrl, string jsonString) where T : class
        {
            const string contentType = "application/json";
            try
            {
                var result = await _client.PostAsync(webUrl, new StringContent(jsonString, Encoding.UTF8, contentType));
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public async Task<T> GetResponse<T>(string webUrl) where T : class
        {
            try
            {
                if (!webUrl.Contains("/test"))
                    webUrl += "&password=" + StaticHelper.ServerPassword;

                var response = await _client.GetAsync(webUrl, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var content = JsonConvert.DeserializeObject<T>(jsonResult);
                        return content;
                    }
                    catch(Exception ex)
                    {
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public async Task<byte[]> GetBytesResponse(string webUrl)
        {
            try
            {
                var response = await _client.GetByteArrayAsync(webUrl);

                if (response != null)
                {
                    return response;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            return null;
        }
    }
    public class QueryResult
    {
        public string Key { get; set; }
        public double Value { get; set; }
    }
    public enum ItemPictureTypes
    {
        GridView = 1,
        DetailView = 2
    }

    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ResponseData { get; set; }
    }
}