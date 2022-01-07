using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Implementations
{
    public class Invoker : IInvoker
    {
        public Invoker() { }
        public async Task<CommandObject> Invoke(ICommand command)
        {
            return await command.Execute();
        }
    }
}
