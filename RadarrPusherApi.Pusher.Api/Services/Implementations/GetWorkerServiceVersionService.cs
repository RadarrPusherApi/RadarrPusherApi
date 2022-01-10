using Newtonsoft.Json;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;
using System.Diagnostics;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class GetWorkerServiceVersionService : IGetWorkerServiceVersionService
    {
        private readonly ILogger _logger;
        private readonly IWorkerServiceReceiver _workerServiceReceiver;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly IDeleteCloudinaryRawFileService _deleteCloudinaryRawFileService;

        public GetWorkerServiceVersionService(ILogger logger, IWorkerServiceReceiver workerServiceReceiver, ICloudinaryClient cloudinaryClient, IDeleteCloudinaryRawFileService deleteCloudinaryRawFileService)
        {
            _logger = logger;
            _workerServiceReceiver = workerServiceReceiver;
            _cloudinaryClient = cloudinaryClient;
            _deleteCloudinaryRawFileService = deleteCloudinaryRawFileService;
        }

        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns WorkerServiceVersionModel</returns>
        public async Task<WorkerServiceVersionModel> GetWorkerServiceVersionServiceAsync(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster)
        {
            WorkerServiceVersionModel workerServiceVersion = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerServiceReceiver.Connect(channelNameReceive, eventNameReceive, pusherAppId, pusherKey, pusherSecret, pusherCluster);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetWorkerServiceVersionCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerServiceReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage), pusherAppId, pusherKey, pusherSecret, pusherCluster);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerServiceReceiver.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerServiceReceiver.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerServiceReceiver.TimeLimit.TotalMilliseconds)
                    {
                        throw new Exception("Get Worker Service version took too long!");
                    }
                }
                
                if (_workerServiceReceiver.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerServiceReceiver.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerServiceReceiver.ReturnData);

                    await _deleteCloudinaryRawFileService.DeleteCloudinaryRawFile(pusherAppId, pusherKey, pusherSecret, pusherCluster, _workerServiceReceiver.CloudinaryPublicId);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("Get movies cloudinary response has no return data!");
                    }

                    workerServiceVersion = JsonConvert.DeserializeObject<WorkerServiceVersionModel>(responseContent);
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
            finally
            {
                await _workerServiceReceiver.Disconnect();
            }

            return workerServiceVersion;
        }
    }
}