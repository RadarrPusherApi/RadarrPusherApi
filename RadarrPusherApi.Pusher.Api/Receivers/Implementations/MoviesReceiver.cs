using Newtonsoft.Json;
using RadarrApiWrapper;
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
    public class MoviesReceiver : Pusher, IMoviesReceiver
    {
        private readonly ILogger _logger;
        private readonly IRadarrClient _radarrClient;
        private readonly IPusherSettings _pusherSettings;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public MoviesReceiver(ILogger logger, IRadarrClient radarrClient, IInvoker invoker, ICloudinaryClient cloudinaryClient, IPusherSettings pusherSettings) : base(logger, invoker, cloudinaryClient, pusherSettings)
        {
            _logger = logger;
            _radarrClient = radarrClient;
            _pusherSettings = pusherSettings;

            _channelNameReceive = $"{ ServiceType.Movies }{ PusherChannel.WorkerServiceChannel }";
            _eventNameReceive = $"{ ServiceType.Movies }{ PusherEvent.WorkerServiceEvent }";
            _channelNameSend = $"{ ServiceType.Movies }{ PusherChannel.ApiChannel }";
            _eventNameSend = $"{ ServiceType.Movies }{ PusherEvent.ApiEvent }";

            if (string.IsNullOrWhiteSpace(_pusherSettings.PusherAppId) || string.IsNullOrWhiteSpace(_pusherSettings.PusherKey) || string.IsNullOrWhiteSpace(_pusherSettings.PusherSecret) || string.IsNullOrWhiteSpace(_pusherSettings.PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }

        /// <summary>
        /// Connect the get movies command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectGetMoviesCommander()
        {
            try
            {
                var pusherReceive = new PusherClient.Pusher(_pusherSettings.PusherKey, new PusherClient.PusherOptions { Cluster = _pusherSettings.PusherCluster });

                var myChannel = await pusherReceive.SubscribeAsync(_channelNameReceive);
                myChannel.Bind($"{_eventNameReceive}_{CommandType.GetMoviesCommand}", async data =>
                {
                    string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                    var pusherReceiveMessageModel = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);
                    var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherReceiveMessageModel.Message);

                    if (deserializeObject.Command == CommandType.GetMoviesCommand)
                    {
                        var command = new GetMoviesCommand(_radarrClient);
                        await ExecuteCommand(command, $"{_channelNameSend}_{deserializeObject.SendMessageChanelGuid}", _eventNameSend);
                    }
                });

                await pusherReceive.ConnectAsync();
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Connect the get movie command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectGetMovieCommander()
        {
            try
            {
                var pusherReceive = new PusherClient.Pusher(_pusherSettings.PusherKey, new PusherClient.PusherOptions { Cluster = _pusherSettings.PusherCluster });

                var myChannel = await pusherReceive.SubscribeAsync(_channelNameReceive);
                myChannel.Bind($"{_eventNameReceive}_{CommandType.GetMovieCommand}", async data =>
                {
                    string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                    var pusherReceiveMessageModel = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);
                    var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherReceiveMessageModel.Message);

                    if (deserializeObject.Command == CommandType.GetMovieCommand)
                    {
                        var id = JsonConvert.DeserializeObject<int>(deserializeObject.Values);
                        var command = new GetMovieCommand(_radarrClient, id);
                        await ExecuteCommand(command, $"{_channelNameSend}_{deserializeObject.SendMessageChanelGuid}", _eventNameSend);
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
