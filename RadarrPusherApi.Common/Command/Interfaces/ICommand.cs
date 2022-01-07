using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Interfaces
{
    public interface ICommand
    {
        Task<CommandObject> Execute();
    }
}