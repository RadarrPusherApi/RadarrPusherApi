using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class WorkerTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly CloudinaryReceiver _cloudinaryReceiver;
        private readonly WorkerReceiver _workerReceiver;

        public WorkerTests(CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _cloudinaryReceiver = new CloudinaryReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient, _commonHelper.PusherSettings);
            _workerReceiver = new WorkerReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient, _commonHelper.PusherSettings);
        }

        [Fact]
        public async Task GetWorkerServiceVersion()
        {
            // Arrange
            await _cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander();
            await _workerReceiver.ConnectGetWorkerServiceVersionCommander();
            var cloudinaryService = new CloudinaryService(_commonHelper.Logger, _commonHelper.WorkerConnector);
            var workerService = new WorkerService(_commonHelper.Logger, _commonHelper.WorkerConnector, _commonHelper.CloudinaryClient, cloudinaryService);

            // Act
            var version = await workerService.GetWorkerServiceVersionServiceAsync();

            // Assert
            Assert.NotNull(version);
            Assert.Equal(new Version("1.0.0.0"), version);
        }
    }
}