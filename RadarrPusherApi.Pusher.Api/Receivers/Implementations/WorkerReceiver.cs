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
    public class WorkerReceiver : Pusher, IWorkerReceiver
    {
        private readonly ILogger _logger;
        private readonly IPusherSettings _pusherSettings;
        
        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public WorkerReceiver(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient, IPusherSettings pusherSettings) : base(logger, invoker, cloudinaryClient, pusherSettings)
        {
            _logger = logger;
            _pusherSettings = pusherSettings;

            _channelNameReceive = PusherChannel.WorkerServiceChannel.ToString();
            _eventNameReceive = PusherEvent.WorkerServiceEvent.ToString();
            _channelNameSend = PusherChannel.ApiChannel.ToString();
            _eventNameSend = PusherEvent.ApiEvent.ToString();

            if (string.IsNullOrWhiteSpace(_pusherSettings.PusherAppId) || string.IsNullOrWhiteSpace(_pusherSettings.PusherKey) || string.IsNullOrWhiteSpace(_pusherSettings.PusherSecret) || string.IsNullOrWhiteSpace(_pusherSettings.PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }

        /// <summary>
        /// Connect the get worker service version receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectGetWorkerServiceVersionCommander()
        {
            try
            {
                var pusherReceive = new PusherClient.Pusher(_pusherSettings.PusherKey, new PusherClient.PusherOptions { Cluster = _pusherSettings.PusherCluster });

                var myChannel = await pusherReceive.SubscribeAsync(_channelNameReceive);
                myChannel.Bind($"{_eventNameReceive}_{CommandType.GetWorkerServiceVersionCommand}", async data =>
                {
                    string pusherData = data.GetType().GetProperty("data").GetValue(data, null);
                    var pusherReceiveMessageModel = JsonConvert.DeserializeObject<PusherReceiveMessageModel>(pusherData);
                    var deserializeObject = JsonConvert.DeserializeObject<PusherSendMessageModel>(pusherReceiveMessageModel.Message);

                    if (deserializeObject.Command == CommandType.GetWorkerServiceVersionCommand)
                    {
                        var command = new GetWorkerServiceVersionCommand();
                        await ExecuteCommand(command, _channelNameSend, $"{_eventNameSend}_{deserializeObject.SendMessageChanelGuid}");
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
