using Newtonsoft.Json;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class DeleteCloudinaryRawFileService : IDeleteCloudinaryRawFileService
    {
        private readonly ILogger _logger;
        private readonly IWorkerServiceReceiver _workerServiceReceiver;

        public DeleteCloudinaryRawFileService(ILogger logger, IWorkerServiceReceiver workerServiceReceiver)
        {
            _logger = logger;
            _workerServiceReceiver = workerServiceReceiver;
        }

        /// <summary>
        /// Delete the cloudinary raw file by public id.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteCloudinaryRawFile(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster, string publicId)
        {
            var channelNameReceive = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherChannel.ApiChannel}";
            var eventNameReceive = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.DeleteCloudinaryRawFileCommand }{ PusherEvent.WorkerServiceEvent}";
            
            try
            {
                await _workerServiceReceiver.Connect(channelNameReceive, eventNameReceive, pusherAppId, pusherKey, pusherSecret, pusherCluster);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.DeleteCloudinaryRawFileCommand, Values = JsonConvert.SerializeObject(publicId) };
                await _workerServiceReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage), pusherAppId, pusherKey, pusherSecret, pusherCluster);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
        }
    }
}