using Moq;
using Newtonsoft.Json;
using NzbDrone.Core.Movies;
using RadarrApiWrapper;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using RadarrPusherApi.Common.Tests.Factories;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Common.Tests.Command.Commands
{
    public class GetMovieCommandTests
    {
        private readonly Mock<IRadarrClient> _mockIRadarrClient;
        private GetMovieCommand _getMovieCommand;

        public GetMovieCommandTests()
        {
            _mockIRadarrClient = new Mock<IRadarrClient>();
        }

        [Fact]
        public async Task Execute()
        {
            const int id = 1;
            const int noOfMovies = 5;

            // Arrange
            _mockIRadarrClient.Setup(x => x.Movie.GetMovie(id)).ReturnsAsync(MovieFactory.CreateMovies(noOfMovies).FirstOrDefault());
            _getMovieCommand = new GetMovieCommand(_mockIRadarrClient.Object, id);

            // Act
            var commandData = await _getMovieCommand.Execute();

            var movie = JsonConvert.DeserializeObject<Movie>(commandData.Message);

            // Assert
            Assert.NotNull(movie);
            Assert.True(!string.IsNullOrWhiteSpace(movie?.Title));
        }

        [Fact]
        public async Task Execute_ZeroMovies()
        {
            const int id = 1;
            const int noOfMovies = 0;

            // Arrange
            _mockIRadarrClient.Setup(x => x.Movie.GetMovie(id)).ReturnsAsync(MovieFactory.CreateMovies(noOfMovies).FirstOrDefault());
            _getMovieCommand = new GetMovieCommand(_mockIRadarrClient.Object, id);

            // Act
            var commandData = await _getMovieCommand.Execute();

            var movies = JsonConvert.DeserializeObject<Movie>(commandData.Message);

            // Assert
            Assert.Null(movies);
        }
    }
}