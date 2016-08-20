using System;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatClient
    {
        private Func<ChatMessage, Task> _onMessageReceived;

        public string Name { get; }

        public ChatClient(string name, Func<ChatMessage, Task> onMessageReceived)
        {
            Name = name;
            _onMessageReceived = onMessageReceived;
        }

        public async Task ReceiveMessage(ChatMessage message)
        {
            await _onMessageReceived(message);
        }
    }
}
