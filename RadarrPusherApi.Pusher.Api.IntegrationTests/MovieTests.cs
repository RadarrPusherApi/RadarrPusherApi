using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class MovieTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly ITestOutputHelper _outputHelper;
        private readonly CloudinaryReceiver _cloudinaryReceiver;
        private readonly MovieReceiver _movieReceiver;

        public MovieTests(ITestOutputHelper outputHelper, CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _outputHelper = outputHelper;
            _cloudinaryReceiver = new CloudinaryReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
            _movieReceiver = new MovieReceiver(commonHelper.Logger, commonHelper.RadarrClient, commonHelper.Invoker, commonHelper.CloudinaryClient);
        }

        [Fact]
        public async Task GetMovies()
        {
            // Arrange
            await _cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            await _movieReceiver.ConnectGetMoviesCommander(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            var cloudinaryService = new CloudinaryService(_commonHelper.Logger, _commonHelper.WorkerReceiver);
            var movieService = new MovieService(_commonHelper.Logger, _commonHelper.WorkerReceiver, _commonHelper.CloudinaryClient, cloudinaryService);

            // Act
            var movies = await movieService.GetMoviesServiceAsync(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);

            // Assert
            Assert.NotNull(movies);
            Assert.True(movies.Count > 1, "Expected movies count to be greater than 0.");

            foreach (var movie in movies)
            {
                _outputHelper.WriteLine(movie.Title);
            }
        }
    }
}