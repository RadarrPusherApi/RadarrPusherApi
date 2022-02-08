using Newtonsoft.Json;
using RadarrPusherApi.WebApi.IntegrationTests.Common.Helpers;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RadarrPusherApi.WebApi.IntegrationTests.Controllers
{
    [Collection(nameof(CommonHelper))]
    public class MoviesController
    {
        private readonly CommonHelper _commonHelper;

        public MoviesController(ITestOutputHelper outputHelper, CommonHelper commonHelper)
        {
            commonHelper.OutputHelper = outputHelper;
            _commonHelper = commonHelper;
        }

        [Fact(Skip = "This test needs to be disabled since the WebApi is not yet deployed for testing and if run it will fail.")]
        public async Task GetMovies()
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
            response = await _commonHelper.CallEndPoint("movies", Method.Get, bearer.ToString(), token.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic moviesResponse = JsonConvert.DeserializeObject(response.Content);

            // Assert
            Assert.NotNull(moviesResponse);

            var movies = moviesResponse.ToObject<dynamic[]>();
            Assert.True(movies.Length > 0);
        }

        [Fact(Skip = "This test needs to be disabled since the WebApi is not yet deployed for testing and if run it will fail.")]
        public async Task GetMovie()
        {
            // Arrange
            const int id = 1;

            var response = await _commonHelper.GetAuth0BearerToken();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic tokenResponse = JsonConvert.DeserializeObject(response.Content);
            Assert.NotNull(tokenResponse);

            var bearer = tokenResponse.token_type;
            var token = tokenResponse.access_token;

            Assert.NotNull(bearer);
            Assert.NotNull(token);

            // Act
            response = await _commonHelper.CallEndPoint($"movies/{id}", Method.Get, bearer.ToString(), token.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic movieResponse = JsonConvert.DeserializeObject(response.Content);

            // Assert
            Assert.NotNull(movieResponse);
            Assert.Equal(id, Convert.ToInt32(movieResponse.id.Value));
        }
    }
}