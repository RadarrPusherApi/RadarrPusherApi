using Newtonsoft.Json;
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

        public GetWorkerServiceVersionService(ILogger logger, IWorkerServiceReceiver workerServiceReceiver)
        {
            _logger = logger;
            _workerServiceReceiver = workerServiceReceiver;
        }

        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns WorkerServiceVersionModel</returns>
        public async Task<WorkerServiceVersionModel> GetWorkerServiceVersionServiceAsync(Setting setting)
        {
            WorkerServiceVersionModel workerServiceVersion = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.GetWorkerServiceVersionCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerServiceReceiver.Connect(channelNameReceive, eventNameReceive, setting.PusherAppId, setting.PusherKey, setting.PusherSecret, setting.PusherCluster);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetWorkerServiceVersionCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerServiceReceiver.SendMessage(channelNameSend, eventNameSend, JsonConvert.SerializeObject(pusherSendMessage), setting.PusherAppId, setting.PusherKey, setting.PusherSecret, setting.PusherCluster);

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
                        throw new Exception("Get Worker Service version has no return data!");
                    }

                    workerServiceVersion = JsonConvert.DeserializeObject<WorkerServiceVersionModel>(_workerServiceReceiver.ReturnData);
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