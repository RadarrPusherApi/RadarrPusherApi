using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Movies;

namespace RadarrPusherApi.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly string _pusherAppId;
        private readonly string _pusherKey;
        private readonly string _pusherSecret;
        private readonly string _pusherCluster;
        private readonly Pusher.Api.Services.Interfaces.IMoviesService _moviesService;

        public MoviesController(IConfiguration configuration, Pusher.Api.Services.Interfaces.IMoviesService moviesService)
        {
            _moviesService = moviesService;

            _pusherAppId = configuration.GetSection("PusherAppId").Value;
            _pusherKey = configuration.GetSection("PusherKey").Value;
            _pusherSecret = configuration.GetSection("PusherSecret").Value;
            _pusherCluster = configuration.GetSection("PusherCluster").Value;
        }

        [HttpGet(Name = "GetMovies")]
        public async Task<IList<Movie>> Get()
        {
            return await _moviesService.GetMoviesServiceAsync(_pusherAppId, _pusherKey, _pusherSecret, _pusherCluster);
        }
    }
}