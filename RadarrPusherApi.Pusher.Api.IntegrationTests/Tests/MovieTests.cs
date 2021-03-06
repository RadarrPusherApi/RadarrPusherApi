using System.Threading.Tasks;
using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using Xunit;
using Xunit.Abstractions;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests.Tests
{
    [Collection(nameof(CommonHelper))]
    public class MovieTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly CommonHelper _commonHelper;
        private readonly CloudinaryReceiver _cloudinaryReceiver;
        private readonly MoviesReceiver _moviesReceiver;

        public MovieTests(ITestOutputHelper outputHelper, CommonHelper commonHelper)
        {
            _outputHelper = outputHelper;
            _commonHelper = commonHelper;
            _cloudinaryReceiver = new CloudinaryReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient, _commonHelper.PusherSettings);
            _moviesReceiver = new MoviesReceiver(commonHelper.Logger, commonHelper.RadarrClient, commonHelper.Invoker, commonHelper.CloudinaryClient, _commonHelper.PusherSettings);
        }

        [Fact]
        public async Task GetMovies()
        {
            // Arrange
            await _cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander();
            await _moviesReceiver.ConnectGetMoviesCommander();
            var cloudinaryService = new CloudinaryService(_commonHelper.Logger, _commonHelper.WorkerConnector);
            var moviesService = new MoviesService(_commonHelper.Logger, _commonHelper.WorkerConnector, _commonHelper.CloudinaryClient, cloudinaryService);

            // Act
            var movies = await moviesService.GetMoviesServiceAsync();

            // Assert
            Assert.NotNull(movies);
            Assert.True(movies.Count > 1, "Expected movies count to be greater than 0.");

            foreach (var movie in movies)
            {
                _outputHelper.WriteLine(movie.Title);
            }
        }

        [Fact]
        public async Task GetMovie()
        {
            // Arrange
            const int id = 1;

            await _cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander();
            await _moviesReceiver.ConnectGetMovieCommander();
            var cloudinaryService = new CloudinaryService(_commonHelper.Logger, _commonHelper.WorkerConnector);
            var moviesService = new MoviesService(_commonHelper.Logger, _commonHelper.WorkerConnector, _commonHelper.CloudinaryClient, cloudinaryService);

            // Act
            var movie = await moviesService.GetMovieServiceAsync(id);

            // Assert
            Assert.NotNull(movie);
            Assert.Equal(id, movie.Id);

            _outputHelper.WriteLine(movie.Title);
        }
    }
}