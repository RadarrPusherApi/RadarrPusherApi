using RadarrApiWrapper;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.WorkerService.Windows;
using RestSharp;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        var cloudinaryCloudName = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Cloudinary:CloudName");
        var cloudinaryApiKey = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Cloudinary:ApiKey");
        var cloudinaryApiSecret = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Cloudinary:ApiSecret");
        var pusherAppId = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Pusher:AppId");
        var pusherKey = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Pusher:Key");
        var pusherSecret = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Pusher:Secret");
        var pusherCluster = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Pusher:Cluster");
        var radarrApiBaseUrl = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Radarr:ApiBaseUrl");
        var radarrApiKey = services.BuildServiceProvider().GetService<IConfiguration>()?.GetValue<string>("Radarr:ApiKey");

        services.AddSingleton<ICloudinaryClient, CloudinaryClient>(serviceProvider => new CloudinaryClient(cloudinaryCloudName, cloudinaryApiKey, cloudinaryApiSecret));
        services.AddSingleton<IPusherSettings, PusherSettings>(serviceProvider => new PusherSettings(pusherAppId, pusherKey, pusherSecret, pusherCluster));
        var restClient = new RestClient(radarrApiBaseUrl);
        restClient.AddDefaultHeader("X-Api-Key", radarrApiKey);

        services.AddSingleton<RadarrPusherApi.Common.Logger.Interfaces.ILogger, Logger>(serviceProvider => new Logger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RadarrApiWrapper.WorkerService.Windows.SQLite.db3")));

        services.AddHostedService<Worker>();

        services.AddSingleton<IInvoker, Invoker>();
        services.AddSingleton<IRadarrClient, RadarrClient>(serviceProvider => new RadarrClient(restClient));

        services.AddSingleton<ICloudinaryReceiver, CloudinaryReceiver>();
        services.AddSingleton<IWorkerReceiver, WorkerReceiver>();
        services.AddSingleton<IMoviesReceiver, MoviesReceiver>();
    })
    .Build();

await host.RunAsync();
