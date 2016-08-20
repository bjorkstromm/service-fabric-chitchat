using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IChatService
    {
        Task<bool> RegisterClientAsync(ChatClient client);
    }
}