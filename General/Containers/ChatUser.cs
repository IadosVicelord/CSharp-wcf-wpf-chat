using System.Runtime.Serialization;

namespace ChatLib
{
    [DataContract]
    public class ChatUser
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public sealed class ChatStoredUser : ChatUser
    {
        [DataMember]
        public string Password { get; }
        public ChatStoredUser(int id, string name, string pword)
        {
            ID = id;
            Name = name;
            Password = pword;
        }
    }
}
