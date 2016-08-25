using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Owin.WebSocket;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class ChatController : WebSocketConnection
    {
        private readonly IChatClientService _chatService;
        private ChatClient _chatClient = null;

        public ChatController(IChatClientService chatService)
        {
            if(chatService == null)
            {
                throw new ArgumentNullException(nameof(chatService));
            }

            _chatService = chatService;
        }

        public override Task OnOpenAsync()
        {
            return SendWelcomeMessage();
        }

        public async override Task OnMessageReceived(ArraySegment<byte> messageBuffer, WebSocketMessageType type)
        {
            // Close if not Text
            if(type != WebSocketMessageType.Text)
            {
                var status = type == WebSocketMessageType.Binary 
                    ? WebSocketCloseStatus.InvalidMessageType
                    : WebSocketCloseStatus.NormalClosure;

                await Close(status, string.Empty);
                return;
            }
            
            // Check if Registered
            if(_chatClient == null)
            {
                var name = Encoding.UTF8.GetString(messageBuffer.Array, messageBuffer.Offset, messageBuffer.Count);

                var chatClient = new ChatClient(name, async (msg) =>
                {
                    await SendMessage($"{{ \"sender\"=\"{msg.Sender}\", \"message\"=\"{msg.Message}\" }}");
                });

                if (await _chatService.RegisterAsync(chatClient))
                {
                    _chatClient = chatClient;
                    await SendMessage($"{{ \"message\"=\"Welcome {name}!\" }}");
                }
                else
                {
                    await SendMessage($"{{ \"message\"=\"The name, {name} is already taken.\" }}");
                    await SendWelcomeMessage();
                }

                return;
            }


            var message = Encoding.UTF8.GetString(messageBuffer.Array, messageBuffer.Offset, messageBuffer.Count).TrimStart();

            if(message.StartsWith(@"@server", StringComparison.InvariantCultureIgnoreCase))
            {
                var tokens = message.Split(' ');
                if(tokens.Length > 1 && tokens[1].Equals("clients", StringComparison.InvariantCultureIgnoreCase))
                {
                    var clients = await _chatService.GetClientsAsync();
                    
                    await SendMessage($"{{ \"message\"=\"{string.Join(",",clients)}\" }}");
                }
            }
        }

        private Task SendWelcomeMessage()
        {
            return SendMessage("{ \"message\"=\"Welcome to ChitChat. Please tell me your name.\" }");
        }

        private Task SendMessage(string message)
        {
            return SendText(Encoding.UTF8.GetBytes(message), true);
        }
    }
}
