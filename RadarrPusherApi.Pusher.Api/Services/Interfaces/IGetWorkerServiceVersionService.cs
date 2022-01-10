using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IGetWorkerServiceVersionService
    {
        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns a WorkerServiceVersionModel</returns>
        Task<WorkerServiceVersionModel> GetWorkerServiceVersionServiceAsync(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster);
    }
}
