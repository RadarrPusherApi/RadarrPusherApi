﻿using Newtonsoft.Json;
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
    public class MoviesReceiver : Pusher, IMoviesReceiver
    {
        private readonly ILogger _logger;
        private readonly IRadarrClient _radarrClient;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public MoviesReceiver(ILogger logger, IRadarrClient radarrClient, IInvoker invoker, ICloudinaryClient cloudinaryClient) : base(logger, invoker, cloudinaryClient)
        {
            _logger = logger;
            _radarrClient = radarrClient;

            _channelNameReceive = $"{ CommandType.MoviesCommand }{ PusherChannel.WorkerServiceChannel }";
            _eventNameReceive = $"{ CommandType.MoviesCommand }{ PusherEvent.WorkerServiceEvent }";
            _channelNameSend = $"{ CommandType.MoviesCommand }{ PusherChannel.ApiChannel }";
            _eventNameSend = $"{ CommandType.MoviesCommand }{ PusherEvent.ApiEvent }";
        }

        /// <summary>
        /// Connect the get movies command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task ConnectGetMoviesCommander(string appId, string key, string secret, string cluster)
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

                        if (deserializeObject.Command == CommandType.MoviesCommand)
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

        /// <summary>
        /// Connect the get movie command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task ConnectGetMovieCommander(string appId, string key, string secret, string cluster)
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

                        if (deserializeObject.Command == CommandType.MoviesCommand)
                        {
                            var id = JsonConvert.DeserializeObject<int>(deserializeObject.Values);
                            var command = new GetMovieCommand(_radarrClient, id);
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