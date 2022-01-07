using RadarrApiWrapper;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.WorkerService.Windows;
using RestSharp;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        var cloudinaryCloudName = services.BuildServiceProvider().GetService<IConfiguration>()?.GetSection("CloudinaryCloudName").Value;
        var cloudinaryApiKey = services.BuildServiceProvider().GetService<IConfiguration>()?.GetSection("CloudinaryApiKey").Value;
        var cloudinaryApiSecret = services.BuildServiceProvider().GetService<IConfiguration>()?.GetSection("CloudinaryApiSecret").Value;
        var radarrUrl = services.BuildServiceProvider().GetService<IConfiguration>()?.GetSection("RadarrUrl").Value;
        var radarrApiKey = services.BuildServiceProvider().GetService<IConfiguration>()?.GetSection("RadarrApiKey").Value;

        services.AddSingleton<ICloudinaryClient, CloudinaryClient>(serviceProvider => new CloudinaryClient(cloudinaryCloudName, cloudinaryApiKey, cloudinaryApiSecret));
        var restClient = new RestClient(radarrUrl);
        restClient.AddDefaultHeader("X-Api-Key", radarrApiKey);

        services.AddSingleton<RadarrPusherApi.Common.Logger.Interfaces.ILogger, Logger>(serviceProvider => new Logger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RadarrApiWrapper.WorkerService.Windows.SQLite.db3")));

        services.AddHostedService<Worker>();

        services.AddSingleton<IInvoker, Invoker>();
        services.AddSingleton<IRadarrClient, RadarrClient>(serviceProvider => new RadarrClient(restClient));

        services.AddSingleton<IDeleteCloudinaryRawFileCommandReceiver, DeleteCloudinaryRawFileCommandReceiver>();
        services.AddSingleton<IGetWorkerServiceVersionCommandReceiver, GetWorkerServiceVersionCommandReceiver>();
        services.AddSingleton<IGetMoviesCommandReceiver, GetMoviesCommandReceiver>();
    })
    .Build();

await host.RunAsync();
