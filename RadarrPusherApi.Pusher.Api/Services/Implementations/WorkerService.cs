using Newtonsoft.Json;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;
using System.Diagnostics;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class WorkerService : IWorkerService
    {
        private readonly ILogger _logger;
        private readonly IWorkerReceiver _workerReceiver;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public WorkerService(ILogger logger, IWorkerReceiver workerReceiver, ICloudinaryClient cloudinaryClient, ICloudinaryService cloudinaryService)
        {
            _logger = logger;
            _workerReceiver = workerReceiver;
            _cloudinaryClient = cloudinaryClient;
            _cloudinaryService = cloudinaryService;

            _channelNameReceive = $"{ ServiceType.WorkerService }{ PusherChannel.ApiChannel}";
            _eventNameReceive = $"{ ServiceType.WorkerService }{ PusherEvent.ApiEvent}";
            _channelNameSend = $"{ ServiceType.WorkerService }{ PusherChannel.WorkerServiceChannel}";
            _eventNameSend = $"{ ServiceType.WorkerService }{ PusherEvent.WorkerServiceEvent}";
        }

        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns the WorkerService Version</returns>
        public async Task<Version> GetWorkerServiceVersionServiceAsync()
        {
            Version version = null;

            var chanelGuid = Guid.NewGuid();

            try
            {
                await _workerReceiver.ConnectWorker($"{_channelNameReceive}_{chanelGuid}", _eventNameReceive);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetWorkerServiceVersionCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerReceiver.SendMessage(_channelNameSend, $"{_eventNameSend}_{CommandType.GetWorkerServiceVersionCommand}", false, JsonConvert.SerializeObject(pusherSendMessage));

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerReceiver.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerReceiver.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerReceiver.TimeLimit.TotalMilliseconds)
                    {
                        throw new Exception("Get Worker Service version took too long!");
                    }
                }
                
                if (_workerReceiver.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerReceiver.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerReceiver.ReturnData);

                    await _cloudinaryService.DeleteCloudinaryRawFile(_workerReceiver.CloudinaryPublicId);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("Get movies cloudinary response has no return data!");
                    }

                    version = JsonConvert.DeserializeObject<Version>(responseContent);
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
            finally
            {
                await _workerReceiver.DisconnectWorker();
            }

            return version;
        }
    }
}