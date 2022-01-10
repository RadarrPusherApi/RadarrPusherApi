using Newtonsoft.Json;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Models;
using System.Reflection;

namespace RadarrPusherApi.Common.Command.Implementations.Commands
{
    public class GetWorkerServiceVersionCommand : ICommand
    {
        private const string AppName = "RadarrPusherApi.Common";
        
        /// <summary>
        /// Returns the Worker Service version.
        /// </summary>
        /// <returns>Returns a WorkerServiceVersionModel</returns>
        public async Task<CommandObject> Execute()
        {
            var commandObject = new CommandObject();
            
            var currentAppPath = AppDomain.CurrentDomain.BaseDirectory;
            var currentApp = Path.Combine(currentAppPath, $"{AppName}.dll");
            var assemblyName = AssemblyName.GetAssemblyName(currentApp);

            var commandData = new WorkerServiceVersionModel { Version = assemblyName.Version };


            commandObject.SendMessage = true;
            commandObject.Message = JsonConvert.SerializeObject(commandData);

            return await Task.FromResult(commandObject);
        }
    }
}