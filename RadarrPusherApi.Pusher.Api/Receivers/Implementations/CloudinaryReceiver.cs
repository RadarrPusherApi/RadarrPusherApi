using Newtonsoft.Json;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Receivers.Implementations
{
    public class CloudinaryReceiver : Pusher, ICloudinaryReceiver
    {
        private readonly ILogger _logger;
        private readonly ICloudinaryClient _cloudinaryClient;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;

        public CloudinaryReceiver(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient) : base(logger, invoker, cloudinaryClient)
        {
            _logger = logger;
            _cloudinaryClient = cloudinaryClient;

            _channelNameReceive = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherChannel.WorkerServiceChannel }";
            _eventNameReceive = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherEvent.WorkerServiceEvent }";
        }

        /// <summary>
        /// Connect the delete Cloudinary file command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task ConnectDeleteCloudinaryFileCommander(string appId, string key, string secret, string cluster)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(cluster))
                {
                    var pusherReceive = new PusherClient.Pusher(key, new PusherClient.PusherOptions { Cluster = cluster });

                    var myChannel = await pusherReceive.SubscribeAsync(_channelNameReceive);
                    myChannel.Bind(_eventNameReceive, async data =>
                    {
                        string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                        var pusherReceiveMessageModel = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);
                        var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherReceiveMessageModel.Message);

                        if (deserializeObject.Command == CommandType.DeleteCloudinaryRawFileCommand)
                        {
                            var command = new DeleteCloudinaryRawFileCommand(_cloudinaryClient, JsonConvert.DeserializeObject<string>(deserializeObject.Values));
                            await ExecuteCommand(command, null, null, appId, key, secret, cluster);
                        }
                    });

                    await pusherReceive.ConnectAsync();
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
    }
}
