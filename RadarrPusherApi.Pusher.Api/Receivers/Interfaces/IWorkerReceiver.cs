namespace RadarrPusherApi.Pusher.Api.Receivers.Interfaces
{
    public interface IWorkerReceiver
    {
        /// <summary>
        /// Connect the get worker service version receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        Task ConnectGetWorkerServiceVersionCommander();
    }
}