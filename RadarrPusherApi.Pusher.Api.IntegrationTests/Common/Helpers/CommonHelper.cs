using Microsoft.Extensions.Configuration;
using RadarrApiWrapper;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.AppSettings;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RestSharp;
using System;
using System.IO;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers
{
    public class CommonHelper
    {
        public Settings Settings { get; }
        public IRadarrClient RadarrClient { get; }
        public IInvoker Invoker { get; }
        public ILogger Logger { get; }
        public ICloudinaryClient CloudinaryClient { get; }
        public IWorkerConnector WorkerConnector { get; }
        public IPusherSettings PusherSettings { get; }

        public CommonHelper()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            Settings = new Settings();
            configuration.Bind(Settings);

            var restClient = new RestClient(Settings.Radarr.ApiBaseUrl);
            restClient.AddDefaultHeader("X-Api-Key", Settings.Radarr.ApiKey);

            RadarrClient = new RadarrClient(restClient);

            Invoker = new Invoker();
            Logger = new Logger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RadarrPusherApi.Pusher.Api.IntegrationTests.SQLite.db3"));
            CloudinaryClient = new CloudinaryClient(Settings.Cloudinary.CloudName, Settings.Cloudinary.ApiKey, Settings.Cloudinary.ApiSecret);
            PusherSettings = new PusherSettings(Settings.Pusher.AppId, Settings.Pusher.Key, Settings.Pusher.Secret, Settings.Pusher.Cluster);
            WorkerConnector = new WorkerConnector(Logger, Invoker, CloudinaryClient, PusherSettings);
        }
    }
}
