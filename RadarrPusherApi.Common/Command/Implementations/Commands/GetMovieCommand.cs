using Newtonsoft.Json;
using RadarrApiWrapper;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Implementations.Commands
{
    public class GetMovieCommand : ICommand
    {
        private readonly IRadarrClient _radarrClient;
        private readonly int _id;

        public GetMovieCommand(IRadarrClient radarrClient, int id)
        {
            _radarrClient = radarrClient;
            _id = id;
        }

        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a Movie</returns>
        public async Task<CommandObject> Execute()
        {
            var movies = await _radarrClient.Movie.GetMovie(_id);

            var commandObject = new CommandObject
            {
                SendMessage = true,
                Message = JsonConvert.SerializeObject(movies)
            };

            return await Task.FromResult(commandObject);
        }
    }
}