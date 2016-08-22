using System;
using System.Runtime.Serialization;

namespace ChatService.Interfaces
{
    [DataContract]
    public class ChatClient
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        public ChatClient(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
