﻿using Newtonsoft.Json;
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
        private readonly IWorkerConnector _workerConnector;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public CloudinaryService(ILogger logger, IWorkerConnector workerConnector)
        {
            _logger = logger;
            _workerConnector = workerConnector;

            _channelNameReceive = $"{ServiceType.Cloudinary}{PusherChannel.ApiChannel}";
            _eventNameReceive = $"{ServiceType.Cloudinary}{PusherEvent.ApiEvent}";
            _channelNameSend = $"{ServiceType.Cloudinary}{PusherChannel.WorkerServiceChannel}";
            _eventNameSend = $"{ServiceType.Cloudinary}{PusherEvent.WorkerServiceEvent}";
        }

        /// <summary>
        /// Delete the Cloudinary file by public id.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteCloudinaryRawFile(string publicId)
        {
            try
            {
                await _workerConnector.ConnectWorker(_channelNameReceive, _eventNameReceive);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.DeleteCloudinaryRawFileCommand, Values = JsonConvert.SerializeObject(publicId) };
                await _workerConnector.SendMessage(_channelNameSend, $"{_eventNameSend}_{CommandType.DeleteCloudinaryRawFileCommand}", false, JsonConvert.SerializeObject(pusherSendMessage));
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
        }
    }
}