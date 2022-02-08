using Newtonsoft.Json;
using RadarrPusherApi.WebApi.IntegrationTests.Common.Helpers;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RadarrPusherApi.WebApi.IntegrationTests.Controllers
{
    [Collection(nameof(CommonHelper))]
    public class WorkerServiceController
    {
        private readonly CommonHelper _commonHelper;

        public WorkerServiceController(ITestOutputHelper outputHelper, CommonHelper commonHelper)
        {
            commonHelper.OutputHelper = outputHelper;
            _commonHelper = commonHelper;
        }

        [Fact(Skip = "This test needs to be disabled since the WebApi is not yet deployed for testing and if run it will fail.")]
        public async Task GetWorkerServiceVersion()
        {
            // Arrange
            var response = await _commonHelper.GetAuth0BearerToken();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic tokenResponse = JsonConvert.DeserializeObject(response.Content);
            Assert.NotNull(tokenResponse);

            var bearer = tokenResponse.token_type;
            var token = tokenResponse.access_token;

            Assert.NotNull(bearer);
            Assert.NotNull(token);

            // Act
            response = await _commonHelper.CallEndPoint("workerservice", Method.Get, bearer.ToString(), token.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic versionResponse = JsonConvert.DeserializeObject(response.Content);

            // Assert
            Assert.NotNull(versionResponse);
        }
    }
}