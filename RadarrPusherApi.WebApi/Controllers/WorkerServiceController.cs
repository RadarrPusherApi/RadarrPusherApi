using Microsoft.AspNetCore.Mvc;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerServiceController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkerServiceController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [HttpGet(Name = "GetWorkerServiceVersion")]
        public async Task<Version> Get()
        {
            return await _workerService.GetWorkerServiceVersionServiceAsync();
        }
    }
}