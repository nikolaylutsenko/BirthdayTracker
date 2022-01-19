using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BirthdayTracker.Client.Services
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string uri);
        Task PostAsync(string uri, object value);
        Task<T> PostAsync<T>(string uri, object value);
        Task PutAsync(string uri, object value);
        Task<T> PutAsync<T>(string uri, object value);
        Task DeleteAsync(string uri);
        Task<T> DeleteAsync<T>(string uri);
    }

    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        //private IConfiguration _configuration;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            //_configuration = configuration;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task PostAsync(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            await SendRequest(request);
        }

        public async Task<T> PostAsync<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task PutAsync(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            await SendRequest(request);
        }

        public async Task<T> PutAsync<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task DeleteAsync(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            await SendRequest(request);
        }

        public async Task<T> DeleteAsync<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return await SendRequest<T>(request);
        }

        // helper methods

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task SendRequest(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return;
            }

            await HandleErrors(response);
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return default;
            }

            await HandleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            //options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        private async Task AddJwtHeader(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            //var user = await _localStorageService.GetItemAsync<UserModel>("user");

            //if (user != null)
            //   request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        private async Task HandleErrors(HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
        }
    }
}