using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ChatActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IChatActor : IActor
    {
        Task<bool> MessageAvailable();

        Task<ChatMessage> GetMessage();

        Task AddMessage(ChatMessage message);
    }
}
