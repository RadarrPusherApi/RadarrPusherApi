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

        public Settings Settings { get; }
        public RestClient RestClient { get; }

        public CommonHelper()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            Settings = new Settings();
            configuration.Bind(Settings);

            RestClient = new RestClient(Settings.RadarrPusherApiWebApi.ApiBaseUrl);
        }

        public async Task<RestResponse> GetAuth0BearerToken()
        {
            var request = new RestRequest("token")
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(new { client_id = Settings.Auth0.ClientId, client_secret = Settings.Auth0.ClientSecret, audience = Settings.Auth0.Audience, grant_type = Settings.Auth0.GrantType });
            
            var client = new RestClient($"https://{Settings.Auth0.Domain}/oauth/");
            var response = await client.PostAsync(request);

            OutputHelper.WriteLine(response.Content);

            return response;
        }

        public async Task<RestResponse> CallEndPoint(string endPoint, Method method, string bearer, string token)
        {
            var request = new RestRequest(endPoint, method);
            request.AddHeader("authorization", $"{bearer} {token}");
            var response = await RestClient.ExecuteAsync(request);

            OutputHelper.WriteLine(response.Content);

            return response;
        }
    }
}
