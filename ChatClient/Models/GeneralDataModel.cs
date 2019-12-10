using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Server;
using System.ServiceModel;
using Client.ViewModels;

namespace Client.Models
{
    class GeneralDataModel
    {
        #region fields
        InstanceContext Instance;
        MessageContractClient Server;
        ShellViewModel Shell;
        ChatUser CurrentUser;
        ChatUser SelectedUser;
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Shell">Основной экран-контейнер</param>
        public GeneralDataModel(ShellViewModel Shell)
        {
            //Поля для работы с сервисом
            Instance = new InstanceContext(new CallbackMessage(this));
            Server = new MessageContractClient(Instance);

            //Основная форма-контейнер
            this.Shell = Shell;
        }

        /// <summary>
        /// Подключение пользователя по логину и паролю
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        public void Connect(string username, string password)
        {
            CurrentUser = new ChatUser { ID = 0, Name = username };
            Server.Connect(CurrentUser, password);
        }

        /// <summary>
        /// Отключение пользователя
        /// </summary>
        public void Disconnect()
        {
            if (CurrentUser != null)
            {
                Server.Disconnect(CurrentUser);
                Shell.IsUserLogged = false;
                CurrentUser = null;
            }
        }

        /// <summary>
        /// Обновление ссылки на текущего пользователя при успешной авторизации
        /// </summary>
        public void UpdateCurrentUser()
        {
            Shell.CurrentUser = CurrentUser;
            Shell.IsUserLogged = true;
        }

        /// <summary>
        /// Обновление списка подключенных пользователей
        /// </summary>
        /// <param name="users">Массив подключенных пользователей</param>
        public void UpdateConnectedUsers(ChatUser[] users)
        {
            //Определить ID текущего пользователя
            CurrentUser.ID = users.FirstOrDefault(x => x.Name == CurrentUser.Name).ID;
            //Обновить информацию о текущем пользователе
            UpdateCurrentUser();
            //Удалить текущего пользователя из полученного списка
            users = users.Where(x => x.Name != CurrentUser.Name).ToArray();

            //Обновить список подключенных пользователей
            Shell.ConnectedUsers = new Caliburn.Micro.BindableCollection<ChatUser>(users);

            //Сортировка пользователей по дате последнего сообщения в переписке
            Shell.ConnectedUsers.Sort((a, b) => a.LastMessageDate.CompareTo(b.LastMessageDate));
        }

        /// <summary>
        /// Обновить дату последнего сообщения в списке пользователей
        /// </summary>
        /// <param name="updatedID">Идентификатор обновляемого пользователя</param>
        /// <param name="updatedTime">Новая дата последнего сообщения</param>
        public void UpdateConnectedUsers(int updatedID, DateTime updatedTime)
        {
            //Обнаружение пользователя для которого предполагаются изменения
            ChatUser u = Shell.ConnectedUsers.FirstOrDefault(x => x.ID == updatedID);
            //Обновление информации о последнем сообщении пользователя
            u.LastMessageDate = updatedTime;
            //Обновить информацию о наличии переписки с пользователем
            if (!u.IsMessageExist)
                u.IsMessageExist = true;

            Shell.KeepSelectedStore();
            //Сортировка пользователей по дате последнего сообщения переписки
            Shell.ConnectedUsers.Sort((a, b) => a.LastMessageDate.CompareTo(b.LastMessageDate));
            Shell.KeepSelectedSet();
            Shell.UpdateConnectedList();
        }
        
        /// <summary>
        /// Обработка обновления выбранного пользователя
        /// </summary>
        /// <param name="User">Выбранный пользователь</param>
        public void SetSelectedUser(ChatUser User)
        {
            if(User != null)
                this.SelectedUser = User;
        }

        /// <summary>
        /// Получение идентификатора выбранного пользователя
        /// </summary>
        /// <returns>Идентификатор выбранного пользователя</returns>
        public int GetSelectedUserID() => SelectedUser.ID;

        /// <summary>
        /// Получение информации о том, выбран ли хоть один пользователь
        /// </summary>
        /// <returns>Флаг выбран ли пользователь</returns>
        public bool IsNoOneSelected() => SelectedUser == null ? true : false;

        /// <summary>
        /// Добавление сообщения в общий список
        /// </summary>
        /// <param name="msg">Сообщение</param>
        public void AddMessageToList(Message msg)
        {
            Shell.UpdateMessageList(msg);
        }

        /// <summary>
        /// Запрос истории переписки
        /// </summary>
        /// <returns>Массив сообщений - история переписки</returns>
        public Message[] GetHistory()
        {
            Message[] Messages = Server.GetMessages(CurrentUser.ID, GetSelectedUserID());
            //Нахождение сообщений текущего пользователя в общем списке
            Messages.Where(x => x.SenderID == CurrentUser.ID).ToList().ForEach(x => x.IsSender = true);
            return Messages;
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message">Сообщение которое необходимо отправить</param>
        public void Message(string message)
        {
            Message Message = MessageFactory(message);
            if (!string.IsNullOrEmpty(message))
            {
                //Обновить данные о последнем сообщении в списке пользователей
                UpdateConnectedUsers(Message.ReceiverID, Message.SendDate);
                //Отправить сообщение на сервер
                Server.Message(Message);
                AddMessageToList(Message);
            }
        }

        /// <summary>
        /// Фабрика сообщений 
        /// </summary>
        /// <param name="message">Содержимое сообщения</param>
        /// <returns>Экземпляр сообщения с установленными полями</returns>
        public Message MessageFactory(string message)
        {
            return new Message()
            {
                MessageContent = message,
                SenderID = CurrentUser.ID,
                ReceiverID = GetSelectedUserID(),
                SendDate = DateTime.Now,
                IsSender = true
            };
        }
    }
}
