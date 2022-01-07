using NzbDrone.Core.Movies;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IGetMoviesPagedService
    {
        /// <summary>
        /// Returns the movies paged from Radarr.
        /// </summary>
        /// <returns>Returns a MoviePagedModel</returns>
        Task<IList<Movie>> GetMoviesPagedServiceAsync(Setting setting);
    }
}
