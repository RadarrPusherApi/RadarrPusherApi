using Autofac;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.WebApi
{
    public class AutofacConfig
    {
        public static void Configure(IConfiguration configuration, ContainerBuilder builder)
        {
            var cloudinaryCloudName = configuration.GetSection("CloudinaryCloudName").Value;
            var cloudinaryApiKey = configuration.GetSection("CloudinaryApiKey").Value;
            var cloudinaryApiSecret = configuration.GetSection("CloudinaryApiSecret").Value;
            var pusherAppId = configuration.GetSection("PusherAppId").Value;
            var pusherKey = configuration.GetSection("PusherKey").Value;
            var pusherSecret = configuration.GetSection("PusherSecret").Value;
            var pusherCluster = configuration.GetSection("PusherCluster").Value;

            builder.Register(c => new Logger(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RadarrPusherApi.WebApi.SQLite.db3"))).As<Common.Logger.Interfaces.ILogger>().SingleInstance();
            builder.RegisterType<Invoker>().As<IInvoker>().SingleInstance();
            builder.Register(c => new CloudinaryClient(cloudinaryCloudName, cloudinaryApiKey, cloudinaryApiSecret)).As<ICloudinaryClient>().SingleInstance();
            builder.Register(c => new PusherSettings(pusherAppId, pusherKey, pusherSecret, pusherCluster)).As<IPusherSettings>().SingleInstance();
            builder.RegisterType<CloudinaryService>().As<ICloudinaryService>().SingleInstance();
            builder.RegisterType<CloudinaryService>().As<CloudinaryService>().SingleInstance();
            builder.RegisterType<WorkerReceiver>().As<IWorkerReceiver>().SingleInstance();
            builder.RegisterType<WorkerService>().As<IWorkerService>().SingleInstance();
            builder.RegisterType<MoviesService>().As<IMoviesService>().SingleInstance();

        }
    }
}
