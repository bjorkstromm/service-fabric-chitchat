using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IChatClientService
    {
        Task<bool> RegisterAsync(ChatClient client);

        Task DeRegisterAsync();

        Task<IEnumerable<string>> GetClientsAsync();
    }
}