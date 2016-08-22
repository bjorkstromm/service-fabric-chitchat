using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace ChatService.Interfaces
{
    public interface IChatService : IService
    {
        Task<bool> AddClientAsync(ChatClient client);
        Task<bool> RemoveClientAsync(ChatClient client);

        Task<IEnumerable<ChatClient>> GetClientsAsync();
    }
}
