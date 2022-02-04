using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IWorkerService
    {
        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns the WorkerService Version</returns>
        Task<Version> GetWorkerServiceVersionServiceAsync();
    }
}
