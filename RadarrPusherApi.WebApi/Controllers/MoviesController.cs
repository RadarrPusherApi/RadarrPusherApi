using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Movies;

namespace RadarrPusherApi.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly Pusher.Api.Services.Interfaces.IMoviesService _moviesService;

        public MoviesController(Pusher.Api.Services.Interfaces.IMoviesService moviesService)
        {
            _moviesService = moviesService;
        }

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<IList<Movie>> GetMovies()
        {
            return await _moviesService.GetMoviesServiceAsync();
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<Movie> GetMovie(int id)
        {
            return await _moviesService.GetMovieServiceAsync(id);
        }
    }
}