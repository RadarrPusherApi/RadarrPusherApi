using Microsoft.AspNetCore.Mvc;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerServiceController : ControllerBase
    {
        private readonly string _pusherAppId;
        private readonly string _pusherKey;
        private readonly string _pusherSecret;
        private readonly string _pusherCluster;
        private readonly IWorkerService _workerService;

        public WorkerServiceController(IConfiguration configuration, IWorkerService workerService)
        {
            _workerService = workerService;

            _pusherAppId = configuration.GetSection("PusherAppId").Value;
            _pusherKey = configuration.GetSection("PusherKey").Value;
            _pusherSecret = configuration.GetSection("PusherSecret").Value;
            _pusherCluster = configuration.GetSection("PusherCluster").Value;
        }

        [HttpGet(Name = "GetWorkerServiceVersion")]
        public async Task<WorkerServiceVersionModel> Get()
        {
            return await _workerService.GetWorkerServiceVersionServiceAsync(_pusherAppId, _pusherKey, _pusherSecret, _pusherCluster);
        }
    }
}