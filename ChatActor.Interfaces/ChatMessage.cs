using System.Runtime.Serialization;

namespace ChatActor.Interfaces
{
    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string Message { get; set; }

        public ChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}