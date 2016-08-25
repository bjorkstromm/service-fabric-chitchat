using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatActor.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using WebApi.Models;

namespace WebApi.Services
{
    internal class ChatClientService : IChatClientService
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly Guid _guid = Guid.NewGuid();
        private ChatClient _chatClient;
        private IChatActor _chatActor;
        private volatile bool _stopMessageCheck = false;
        private Task _checkMessageTask;

        public ChatClientService(IServiceLocator serviceLocator)
        {
            if(serviceLocator == null)
            {
                throw new ArgumentNullException(nameof(serviceLocator));
            }

            _serviceLocator = serviceLocator;
        }

        public async Task<bool> RegisterAsync(ChatClient client)
        {
            var service = _serviceLocator.GetInstance<ChatService.Interfaces.IChatService>();

            if(!await service.AddClientAsync(new ChatService.Interfaces.ChatClient(_guid, client.Name)))
            {
                return false;
            }

            _chatActor = ActorProxy.Create<IChatActor>(new ActorId(_guid), new Uri("fabric:/Chitchat/ChatActorService"));
            _chatClient = client;

            _checkMessageTask = CheckMessagesAsync();

            return true;
        }

        private async Task CheckMessagesAsync()
        {
            while(!_stopMessageCheck)
            {
                if(await _chatActor.MessageAvailable())
                {
                    var message = await _chatActor.GetMessage();

                    await _chatClient.ReceiveMessage(
                        new Models.ChatMessage(
                            message.Sender,
                            message.Message));
                }

                Thread.Sleep(150);
            }
        }

        public async Task DeRegisterAsync()
        {
            _stopMessageCheck = true;

            await _checkMessageTask;

            var actorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/Chitchat/ChatActorService"), _chatActor.GetActorId());

            await actorServiceProxy.DeleteActorAsync(_chatActor.GetActorId(), CancellationToken.None);

            _chatActor = null;
        }

        public async Task<IEnumerable<string>> GetClientsAsync()
        {
            var service = _serviceLocator.GetInstance<ChatService.Interfaces.IChatService>();

            var clients = await service.GetClientsAsync();

            return clients.Select(x => x.Name);
        }
    }
}
