using NzbDrone.Core.Movies;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IMoviesService
    {
        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        Task<IList<Movie>> GetMoviesServiceAsync();

        /// <summary>
        /// Returns a movie from Radarr.
        /// </summary>
        /// <returns>Returns a Movie</returns>
        Task<Movie> GetMovieServiceAsync(int id);
    }
}
