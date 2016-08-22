using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using ChatService.Interfaces;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ChatService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ChatService : StatefulService, IChatService
    {
        private const string ChatClients = "ChatClients";

        public ChatService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<IEnumerable<ChatClient>> GetClientsAsync()
        {
            var chatClients = await StateManager.GetOrAddAsync<IReliableDictionary<string, Guid>>(ChatClients);
            var clientList = new List<ChatClient>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerable = await chatClients.CreateEnumerableAsync(transaction);

                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    clientList.Add(new ChatClient(enumerator.Current.Value, enumerator.Current.Key));
                }
            }

            return clientList;
        }

        public async Task<bool> AddClientAsync(ChatClient client)
        {
            var chatClients = await StateManager.GetOrAddAsync<IReliableDictionary<string, Guid>>(ChatClients);

            using (var transaction = StateManager.CreateTransaction())
            {
                if (await chatClients.TryAddAsync(transaction, client.Name, client.Id))
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    transaction.Abort();
                    return false;
                }
            }
        }

        public async Task<bool> RemoveClientAsync(ChatClient client)
        {
            var chatClients = await StateManager.GetOrAddAsync<IReliableDictionary<string, Guid>>(ChatClients);

            using (var transaction = StateManager.CreateTransaction())
            {
                var value = await chatClients.TryRemoveAsync(transaction, client.Name);
                if (value.HasValue)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    transaction.Abort();
                    return false;
                }
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var chatClients = await StateManager.GetOrAddAsync<IReliableDictionary<string, Guid>>(ChatClients);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this, "Running {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
