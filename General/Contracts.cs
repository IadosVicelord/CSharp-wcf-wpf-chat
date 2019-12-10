using System.Collections.Generic;
using System.ServiceModel;

namespace ChatLib
{
    [ServiceContract(CallbackContract = typeof(ICallbackMessage))]
    public interface IMessageContract
    {
        [OperationContract(IsOneWay = true)]
        void Connect(ChatUser User, string Password);
        [OperationContract(IsOneWay = true)]
        void Disconnect(ChatUser User);
        [OperationContract(IsOneWay = true)]
        void Message(Message Msg);
        [OperationContract]
        Message[] GetMessages(int SenderID, int ReceiverID);
        [OperationContract]
        List<ChatUser> RequestConnected();
    }

    public interface ICallbackMessage
    {
        [OperationContract(IsOneWay = true)]
        void GetMessage(Message Msg);
        [OperationContract(IsOneWay = true)]
        void UpdateConnected(ChatUser[] Users);
    }
}
