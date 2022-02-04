namespace RadarrPusherApi.Pusher.Api.Receivers.Interfaces
{
    public interface ICloudinaryReceiver
    {
        /// <summary>
        /// Connect the delete Cloudinary file command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        Task ConnectDeleteCloudinaryFileCommander();
    }
}