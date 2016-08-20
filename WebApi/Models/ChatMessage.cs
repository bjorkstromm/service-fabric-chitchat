namespace WebApi.Models
{
    public class ChatMessage
    {
        public string Sender { get; }
        public string Message { get; }

        public ChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
