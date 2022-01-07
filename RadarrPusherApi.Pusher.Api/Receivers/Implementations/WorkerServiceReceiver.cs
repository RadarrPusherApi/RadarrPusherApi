using Newtonsoft.Json;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Receivers.Implementations
{
    public class WorkerServiceReceiver : Pusher, IWorkerServiceReceiver
    {
        private readonly ILogger _logger;
        public TimeSpan TimeLimit { get; set; }
        public string CloudinaryPublicId { get; set; }
        public bool CommandCompleted { get; set; }
        public string ReturnData { get; set; }
        private PusherClient.Pusher _pusherReceive;

        public WorkerServiceReceiver(ILogger logger)
        {
            _logger = logger;
        }

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
        public async Task Connect(string channelNameReceive, string eventNameReceive, string appId, string key, string secret, string cluster)
        {
            try
            {
                CloudinaryPublicId = string.Empty;
                CommandCompleted = false;
                ReturnData = string.Empty;
                _pusherReceive = null;

                if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(cluster))
                {
                    _pusherReceive = new PusherClient.Pusher(key, new PusherClient.PusherOptions { Cluster = cluster });

                    TimeLimit = new TimeSpan(0, 0, 15);
                    var myChannel = await _pusherReceive.SubscribeAsync(channelNameReceive);
                    myChannel.Bind(eventNameReceive, data =>
                    {
                        string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                        var deserializeObject = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);

                        ReturnData = deserializeObject.Message;
                        CloudinaryPublicId = deserializeObject.CloudinaryPublicId;
                        CommandCompleted = true;
                    });

                    await _pusherReceive.ConnectAsync();
                }
                else
                {
                    throw new Exception("No default setting saved.");
                }
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Disconnect the worker service.
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            await _pusherReceive.DisconnectAsync();
            _pusherReceive = null;
            ReturnData = null;
        }
    }
}
