using Newtonsoft.Json;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Receivers.Implementations
{
    public class WorkerConnector : Pusher, IWorkerConnector
    {
        private readonly ILogger _logger;
        private readonly IPusherSettings _pusherSettings;

        public TimeSpan TimeLimit { get; set; }
        public string CloudinaryPublicId { get; set; }
        public bool CommandCompleted { get; set; }
        public string ReturnData { get; set; }
        private PusherClient.Pusher _pusherReceive;
        
        public WorkerConnector(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient, IPusherSettings pusherSettings) : base(logger, invoker, cloudinaryClient, pusherSettings)
        {
            _logger = logger;
            _pusherSettings = pusherSettings;
            
            if (string.IsNullOrWhiteSpace(_pusherSettings.PusherAppId) || string.IsNullOrWhiteSpace(_pusherSettings.PusherKey) || string.IsNullOrWhiteSpace(_pusherSettings.PusherSecret) || string.IsNullOrWhiteSpace(_pusherSettings.PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }

        /// <summary>
        /// Connect the worker service receiver to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="channelNameReceive">The channel name to connect to</param>
        /// <param name="eventNameReceive">The event name to connect to</param>
        /// <returns></returns>
        public async Task ConnectWorker(string channelNameReceive, string eventNameReceive)
        {
            try
            {
                CloudinaryPublicId = string.Empty;
                CommandCompleted = false;
                ReturnData = string.Empty;
                _pusherReceive = null;

                _pusherReceive = new PusherClient.Pusher(_pusherSettings.PusherKey, new PusherClient.PusherOptions { Cluster = _pusherSettings.PusherCluster });

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
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Disconnect the worker service.
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectWorker()
        {
            await _pusherReceive.DisconnectAsync();
            _pusherReceive = null;
            ReturnData = null;
        }
    }
}
