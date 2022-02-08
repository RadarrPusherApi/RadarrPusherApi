using Newtonsoft.Json;
using RadarrPusherApi.Common.Command.Implementations.Commands;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Common.Tests.Command.Commands
{
    public class GetWorkerServiceVersionCommandTests
    {
        [Fact]
        public async Task Execute()
        {
            // Arrange
            var getWorkerServiceVersionCommand = new GetWorkerServiceVersionCommand();

            // Act
            var commandData = await getWorkerServiceVersionCommand.Execute();

            var version = JsonConvert.DeserializeObject<Version>(commandData.Message);

            // Assert
            Assert.NotNull(version);
            Assert.Equal(new Version("1.0.0.0"), version);
        }
    }
}