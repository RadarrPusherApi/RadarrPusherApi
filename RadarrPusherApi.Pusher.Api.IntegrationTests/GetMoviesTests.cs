using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class GetMoviesTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly ITestOutputHelper _outputHelper;
        private readonly DeleteCloudinaryRawFileCommandReceiver _deleteCloudinaryRawFileCommandReceiver;
        private readonly GetMoviesCommandReceiver _getMoviesCommandReceiver;

        public GetMoviesTests(ITestOutputHelper outputHelper, CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _outputHelper = outputHelper;
            _deleteCloudinaryRawFileCommandReceiver = new DeleteCloudinaryRawFileCommandReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
            _getMoviesCommandReceiver = new GetMoviesCommandReceiver(commonHelper.Logger, commonHelper.RadarrClient, commonHelper.Invoker, commonHelper.CloudinaryClient);
        }

        [Fact]
        public async Task GetMovies()
        {
            // Arrange
            await _deleteCloudinaryRawFileCommandReceiver.Connect(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            await _getMoviesCommandReceiver.Connect(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            var deleteCloudinaryRawFileService = new DeleteCloudinaryRawFileService(_commonHelper.Logger, _commonHelper.WorkerServiceReceiver);
            var getMoviesService = new GetMoviesService(_commonHelper.Logger, _commonHelper.WorkerServiceReceiver, _commonHelper.CloudinaryClient, deleteCloudinaryRawFileService);

            // Act
            var movies = await getMoviesService.GetMoviesServiceAsync(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);

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