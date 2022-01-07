using Newtonsoft.Json;
using RadarrApiWrapper;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Receivers.Implementations
{
    public class GetMoviesCommandReceiver : Pusher, IGetMoviesCommandReceiver
    {
        private readonly ILogger _logger;
        private readonly IRadarrClient _radarrClient;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public GetMoviesCommandReceiver(ILogger logger, IRadarrClient radarrClient, IInvoker invoker, ICloudinaryClient cloudinaryClient) : base(logger, invoker, cloudinaryClient)
        {
            _logger = logger;
            _radarrClient = radarrClient;

            _channelNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherChannel.WorkerServiceChannel }";
            _eventNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherEvent.WorkerServiceEvent }";
            _channelNameSend = $"{ CommandType.GetMoviesCommand }{ PusherChannel.ApiChannel }";
            _eventNameSend = $"{ CommandType.GetMoviesCommand }{ PusherEvent.ApiEvent }";
        }

        /// <summary>
        /// Connect the get movies command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task Connect(string appId, string key, string secret, string cluster)
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
                        var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherData);

                        if (deserializeObject.Command == CommandType.GetMoviesCommand)
                        {
                            var command = new GetMoviesCommand(_radarrClient);
                            await ExecuteCommand(command, $"{_channelNameSend}_{deserializeObject.SendMessageChanelGuid}", _eventNameSend, appId, key, secret, cluster);
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
