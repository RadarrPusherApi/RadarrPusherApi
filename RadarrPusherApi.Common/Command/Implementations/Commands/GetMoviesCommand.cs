using Newtonsoft.Json;
using RadarrApiWrapper;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Implementations.Commands
{
    public class GetMoviesCommand : ICommand
    {
        private readonly IRadarrClient _radarrClient;

        public GetMoviesCommand(IRadarrClient radarrClient)
        {
            _radarrClient = radarrClient;
        }

        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        public async Task<CommandObject> Execute()
        {
            var movies = await _radarrClient.Movie.GetMovies();

            var commandObject = new CommandObject
            {
                SendMessage = true,
                Message = JsonConvert.SerializeObject(movies)
            };

            return await Task.FromResult(commandObject);
        }
    }
}