namespace RadarrPusherApi.Pusher.Api.Receivers.Interfaces
{
    public interface IWorkerServiceReceiver
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
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        Task Connect(string channelNameReceive, string eventNameReceive, string appId, string key, string secret, string cluster);

        /// <summary>
        /// Disconnect the worker service.
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// Send a message to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="channelName">The channel name to connect to</param>
        /// <param name="eventName">The event name to connect to</param>
        /// <param name="message">The message to send</param>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        Task SendMessage(string channelName, string eventName, string message, string appId, string key, string secret, string cluster);
    }
}