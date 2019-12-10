using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ChatLib
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string MessageContent { get; set; }
        [DataMember]
        public int SenderID { get; set; }
        [DataMember]
        public int ReceiverID { get; set; }
        [DataMember]
        public DateTime SendDate { get; set; }

        public Message(string message, int sender, int receiver, DateTime date)
        {
            MessageContent = message;
            SenderID = sender;
            ReceiverID = receiver;
            SendDate = date;
        }
    }
}
