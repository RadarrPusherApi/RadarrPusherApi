using Newtonsoft.Json;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly ILogger _logger;
        private readonly IWorkerReceiver _workerReceiver;

        public CloudinaryService(ILogger logger, IWorkerReceiver workerReceiver)
        {
            _logger = logger;
            _workerReceiver = workerReceiver;
        }

        /// <summary>
        /// Delete the Cloudinary file by public id.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteCloudinaryRawFile(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster, string publicId)
        {
            var channelNameReceive = $"{ CommandType.CloudinaryCommand }{ PusherChannel.ApiChannel}";
            var eventNameReceive = $"{ CommandType.CloudinaryCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.CloudinaryCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.CloudinaryCommand }{ PusherEvent.WorkerServiceEvent}";
            
            try
            {
                await _workerReceiver.ConnectWorker(channelNameReceive, eventNameReceive, pusherAppId, pusherKey, pusherSecret, pusherCluster);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.CloudinaryCommand, Values = JsonConvert.SerializeObject(publicId) };
                await _workerReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage), pusherAppId, pusherKey, pusherSecret, pusherCluster);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
        }
    }
}