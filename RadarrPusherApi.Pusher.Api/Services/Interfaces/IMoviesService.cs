using NzbDrone.Core.Movies;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IMoviesService
    {
        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        Task<IList<Movie>> GetMoviesServiceAsync(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster);
    }
}
