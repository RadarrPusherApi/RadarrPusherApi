using Newtonsoft.Json;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Receivers.Implementations
{
    public class CloudinaryReceiver : Pusher, ICloudinaryReceiver
    {
        private readonly ILogger _logger;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly IPusherSettings _pusherSettings;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;

        public CloudinaryReceiver(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient, IPusherSettings pusherSettings) : base(logger, invoker, cloudinaryClient, pusherSettings)
        {
            _logger = logger;
            _cloudinaryClient = cloudinaryClient;
            _pusherSettings = pusherSettings;

            _channelNameReceive = $"{ CommandType.CloudinaryCommand }{ PusherChannel.WorkerServiceChannel }";
            _eventNameReceive = $"{ CommandType.CloudinaryCommand }{ PusherEvent.WorkerServiceEvent }";

            if (string.IsNullOrWhiteSpace(_pusherSettings.PusherAppId) || string.IsNullOrWhiteSpace(_pusherSettings.PusherKey) || string.IsNullOrWhiteSpace(_pusherSettings.PusherSecret) || string.IsNullOrWhiteSpace(_pusherSettings.PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }

        /// <summary>
        /// Connect the delete Cloudinary file command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectDeleteCloudinaryFileCommander()
        {
            try
            {
                var pusherReceive = new PusherClient.Pusher(_pusherSettings.PusherKey, new PusherClient.PusherOptions { Cluster = _pusherSettings.PusherCluster });

                var myChannel = await pusherReceive.SubscribeAsync(_channelNameReceive);
                myChannel.Bind(_eventNameReceive, async data =>
                {
                    string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                    var pusherReceiveMessageModel = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);
                    var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherReceiveMessageModel.Message);

                    if (deserializeObject.Command == CommandType.CloudinaryCommand)
                    {
                        var command = new DeleteCloudinaryRawFileCommand(_cloudinaryClient, JsonConvert.DeserializeObject<string>(deserializeObject.Values));
                        await ExecuteCommand(command, null, null);
                    }
                });

                await pusherReceive.ConnectAsync();
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }
    }
}
