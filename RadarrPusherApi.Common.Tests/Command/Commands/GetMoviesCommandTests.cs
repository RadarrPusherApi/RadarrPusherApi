using Moq;
using Newtonsoft.Json;
using NzbDrone.Core.Movies;
using RadarrApiWrapper;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using RadarrPusherApi.Common.Tests.Factories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Common.Tests.Command.Commands
{
    public class GetMoviesCommandTests
    {
        private readonly Mock<IRadarrClient> _mockIRadarrClient;
        private GetMoviesCommand _getMoviesCommand;

        public GetMoviesCommandTests()
        {
            _mockIRadarrClient = new Mock<IRadarrClient>();
        }

        [Fact]
        public async Task Execute()
        {
            const int noOfMovies = 5;

            // Arrange
            _mockIRadarrClient.Setup(x => x.Movie.GetMovies()).ReturnsAsync(MovieFactory.CreateMovies(noOfMovies));
            _getMoviesCommand = new GetMoviesCommand(_mockIRadarrClient.Object);

            // Act
            var commandData = await _getMoviesCommand.Execute();

            var movies = JsonConvert.DeserializeObject<IList<Movie>>(commandData.Message);

            // Assert
            Assert.NotNull(movies);
            Assert.Equal(noOfMovies, movies.Count);
        }

        [Fact]
        public async Task Execute_ZeroMovies()
        {
            const int noOfMovies = 0;

            // Arrange
            _mockIRadarrClient.Setup(x => x.Movie.GetMovies()).ReturnsAsync(MovieFactory.CreateMovies(noOfMovies));
            _getMoviesCommand = new GetMoviesCommand(_mockIRadarrClient.Object);

            // Act
            var commandData = await _getMoviesCommand.Execute();

            var movies = JsonConvert.DeserializeObject<IList<Movie>>(commandData.Message);

            // Assert
            Assert.NotNull(movies);
            Assert.Equal(noOfMovies, movies.Count);
        }
    }
}