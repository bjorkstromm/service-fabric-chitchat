using System;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using WebApi.Models;

namespace WebApi.Services
{
    internal class ChatClientService : IChatClientService
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly Guid _guid = Guid.NewGuid();

        public ChatClientService(IServiceLocator serviceLocator)
        {
            if(serviceLocator == null)
            {
                throw new ArgumentNullException(nameof(serviceLocator));
            }

            _serviceLocator = serviceLocator;
        }

        public Task<bool> RegisterAsync(ChatClient client)
        {
            var service = _serviceLocator.GetInstance<ChatService.Interfaces.IChatService>();

            return service.AddClientAsync(new ChatService.Interfaces.ChatClient(_guid, client.Name));
        }
    }
}
