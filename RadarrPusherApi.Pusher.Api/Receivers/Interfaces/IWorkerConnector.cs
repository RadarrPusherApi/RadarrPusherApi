namespace RadarrPusherApi.Pusher.Api.Receivers.Interfaces
{
    public interface IWorkerConnector
    {
        TimeSpan TimeLimit { get; set; }
        string CloudinaryPublicId { get; set; }
        bool CommandCompleted { get; set; }
        string ReturnData { get; set; }

        /// <summary>
        /// Connect the worker service receiver to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="channelNameReceive">The channel name to connect to</param>
        /// <param name="eventNameReceive">The event name to connect to</param>
        /// <returns></returns>
        Task ConnectWorker(string channelNameReceive, string eventNameReceive);

        /// <summary>
        /// Disconnect the worker service.
        /// </summary>
        /// <returns></returns>
        Task DisconnectWorker();

        /// <summary>
        /// Send a message to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="channelName">The channel name to connect to</param>
        /// <param name="eventName">The event name to connect to</param>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        Task SendMessage(string channelName, string eventName, bool useCloudinary, string message);
    }
}