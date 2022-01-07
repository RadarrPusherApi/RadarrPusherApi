using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Interfaces
{
    public interface IInvoker
    {
        Task<CommandObject> Invoke(ICommand command);
    }
}