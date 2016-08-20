using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    internal class ChatService : IChatService
    {
        public Task<bool> RegisterClientAsync(ChatClient client)
        {
            return Task.FromResult(true);
        }
    }
}
