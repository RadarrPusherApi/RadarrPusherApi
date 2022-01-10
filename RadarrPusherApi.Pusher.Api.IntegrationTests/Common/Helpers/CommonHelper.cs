using Microsoft.Extensions.Configuration;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Common.Logger.Interfaces;
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
        public IRestClient RestClient { get; }
        public IInvoker Invoker { get; }
        public ILogger Logger { get; }
        public ICloudinaryClient CloudinaryClient { get; }
        public IWorkerServiceReceiver WorkerServiceReceiver { get; }

        public CommonHelper()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            Settings = new Settings();
            configuration.Bind(Settings);

            RestClient = new RestClient(Settings.RadarrUrl);
            RestClient.AddDefaultHeader("X-Api-Key", Settings.RadarrApiKey);

            Invoker = new Invoker();
            Logger = new Logger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RadarrPusherApi.Pusher.Api.IntegrationTests.SQLite.db3"));
            CloudinaryClient = new CloudinaryClient(Settings.CloudinaryCloudName, Settings.CloudinaryApiKey, Settings.CloudinaryApiSecret);
            WorkerServiceReceiver = new WorkerServiceReceiver(Logger, Invoker, CloudinaryClient);
        }
    }
}
