using Microsoft.Extensions.Configuration;
using RadarrPusherApi.WebApi.IntegrationTests.Common.AppSettings;
using RestSharp;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace RadarrPusherApi.WebApi.IntegrationTests.Common.Helpers
{
    public class CommonHelper
    {
        public ITestOutputHelper OutputHelper { get; set; }

        private readonly Settings _settings;
        private readonly RestClient _restClient;

        public CommonHelper()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            _settings = new Settings();
            configuration.Bind(_settings);

            _restClient = new RestClient(_settings.RadarrPusherApiWebApi.ApiBaseUrl);
        }

        public async Task<RestResponse> GetAuth0BearerToken()
        {
            var request = new RestRequest("token")
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(new { client_id = _settings.Auth0.ClientId, client_secret = _settings.Auth0.ClientSecret, audience = _settings.Auth0.Audience, grant_type = _settings.Auth0.GrantType });
            
            var client = new RestClient($"https://{_settings.Auth0.Domain}/oauth/");
            var response = await client.PostAsync(request);

            OutputHelper.WriteLine(response.Content);

            return response;
        }

        public async Task<RestResponse> CallEndPoint(string endPoint, Method method, string bearer, string token)
        {
            var request = new RestRequest(endPoint, method);
            request.AddHeader("authorization", $"{bearer} {token}");
            var response = await _restClient.ExecuteAsync(request);

            OutputHelper.WriteLine(response.Content);

            return response;
        }
    }
}
