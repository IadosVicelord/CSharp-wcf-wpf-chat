using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatLib;


namespace ChatServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class MessageContract : IMessageContract
    {
        #region private fields
        private List<ChatStoredUser> _registred = RegisterUsers();
        private Dictionary<ICallbackMessage, ChatUser> _connectedUsers = new Dictionary<ICallbackMessage, ChatUser>();
        private List<Message> _messages = new List<Message>();
        #endregion

        #region properties
        /// <summary>
        /// Registred - Список зарегистрированных пользователей
        /// ConnectedUsers - Список подключенных пользователей
        /// Messages - Список сообщений
        /// </summary>
        public List<ChatStoredUser> Registred
        {
            get { return _registred; }
        }
        public Dictionary<ICallbackMessage, ChatUser> ConnectedUsers
        {
            get { return _connectedUsers; }
            set { _connectedUsers = value; }
        }
        public List<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        #endregion

        //Обработка подключения пользователя
        public void Connect(ChatUser User, string Password)
        {
            //Подключаемый пользователь 
            ChatStoredUser StoredUser = Registred.FirstOrDefault(x => x.Name == User.Name && x.Password == Password);
            //Подключение пользователя
            ICallbackMessage Connection = OperationContext.Current.GetCallbackChannel<ICallbackMessage>();
            //Проверка зарегистрирован ли пользователь
            if (StoredUser != null)
            {
                //Проверка не подключен ли уже пользователь
                if (ConnectedUsers.Values.FirstOrDefault(x => x.Name == StoredUser.Name) != null)
                {
                    Console.WriteLine(User.Name + " already logged!");
                    Connection.GetMessage(new Message ("alreadylogged", -1, StoredUser.ID, DateTime.Now));
                    return;
                }
                else
                {
                    Connection.GetMessage(new Message("connect", -1, StoredUser.ID, DateTime.Now));
                    //Идентификатор подключенного пользователя
                    User.ID = StoredUser.ID;
                    //Добавление подключения в словарь
                    ConnectedUsers[Connection] = User;
                    Console.WriteLine(StoredUser.Name + "Connected!");

                    //Обновление списка пользователей для всех подключенных
                    foreach (var u in ConnectedUsers.Keys)
                    {
                        u.UpdateConnected((ChatUser[])ConnectedUsers.Values.ToArray());
                    }
                    return;
                }
            }
            else
            {
                Console.WriteLine(User.Name + " unregistred!");
                Connection.GetMessage(new Message("unregistred", -1, User.ID, DateTime.Now));
                return;
            }
        }

        //Обработка отправки сообщений
        public void Message(Message Msg)
        {
            //Подключение отправителя
            ICallbackMessage Connection = OperationContext.Current.GetCallbackChannel<ICallbackMessage>();
            //Если отправитель не подключен
            if (!ConnectedUsers.TryGetValue(Connection, out ChatUser Sender))
                return;
            //Установка идентификатора отправителя к сообщению
            Msg.SenderID = Sender.ID;
            //Установка даты отправки
            Msg.SendDate = DateTime.Now;

            //Определение подключения получателя
            ICallbackMessage Receiver = ConnectedUsers.FirstOrDefault(x => x.Value.ID == Msg.ReceiverID).Key;
            //Отправка сообщения получателю если он подключен
            if (Receiver != null)
                Receiver.GetMessage(Msg);

            //Добавление сообщения в общий список
            Messages.Add(Msg);
        }

        //Обработка отключения пользователя
        public void Disconnect(ChatUser User)
        {
            //Определение подключения
            ICallbackMessage Connection = ConnectedUsers.FirstOrDefault(x => x.Value.Name == User.Name).Key;
            Console.WriteLine(User.Name + " trying to disconnect!");
            //Если подкючение найдено
            if (Connection != null)
            {
                //Удаление текущего пользователя из списка подключенных
                ConnectedUsers.Remove(Connection);
                Console.WriteLine(User.Name + " disconnected!");
                //Обновление списка подключенных у всех пользователей
                foreach (ICallbackMessage u in ConnectedUsers.Keys)
                {
                    u.UpdateConnected(ConnectedUsers.Values.ToArray());
                }
                return;
            }
            else
            {
                Console.WriteLine(User.Name + " connection not found!");
                return;
            }
        }

        //Обработка запроса всех подключенных пользователей
        public List<ChatUser> RequestConnected()
        {
            return ConnectedUsers.Values.ToList();
        }

        //Обработка запроса всей переписки между отправителем и получателем
        public Message[] GetMessages(int SenderID, int ReceiverID)
        {
            return Messages.Where(x => (x.ReceiverID == ReceiverID && x.SenderID == SenderID ) || ( x.ReceiverID == SenderID && x.SenderID == ReceiverID)).ToArray();
        }

        //Имитация загрузки списка зарегистрированных из БД
        static List<ChatStoredUser> RegisterUsers()
        {
            return new List<ChatStoredUser>
            {
                new ChatStoredUser(1, "John", "qwert"),
                new ChatStoredUser(2, "Victor", "1234"),
                new ChatStoredUser(3, "Jack", "12"),
                new ChatStoredUser(4, "Sergey", "qwerty")
            };
        }
    }
    class Program
    {
        static void Main()
        {
            //Хост сервиса
            ServiceHost host = new ServiceHost(typeof(MessageContract));
            host.Open();
            Console.WriteLine("Service is started!");
            Console.ReadKey();
            host.Close();
        }
    }
}
