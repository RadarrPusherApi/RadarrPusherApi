using NzbDrone.Core.Movies;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IGetMoviesService
    {
        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        Task<IList<Movie>> GetMoviesServiceAsync(Setting setting);
    }
}
