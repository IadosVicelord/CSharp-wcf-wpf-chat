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

        public GeneralDataModel(ShellViewModel Shell)
        {
            //Поля для работы с сервисом
            Instance = new InstanceContext(new CallbackMessage(this));
            Server = new MessageContractClient(Instance);

            //Основная форма-контейнер
            this.Shell = Shell;
        }

        //Подключение пользователя по логину и паролю
        public void Connect(string username, string password)
        {
            CurrentUser = new ChatUser { ID = 0, Name = username };
            Server.Connect(CurrentUser, password);
        }

        //Отключение пользователя
        public void Disconnect()
        {
            if (CurrentUser != null)
            {
                Server.Disconnect(CurrentUser);
                Shell.IsUserLogged = false;
                CurrentUser = null;
            }
        }

        //Обновление ссылки на текущего пользователя при успешной авторизации
        public void UpdateCurrentUser()
        {
            Shell.CurrentUser = CurrentUser;
            Shell.IsUserLogged = true;
        }

        //Обновление списка доступных пользователей
        public void UpdateConnectedUsers(ChatUser[] users)
        {
            CurrentUser.ID = users.FirstOrDefault(x => x.Name == CurrentUser.Name).ID;
            UpdateCurrentUser();
            users = users.Where(x => x.Name != CurrentUser.Name).ToArray();

            Shell.ConnectedUsers = new Caliburn.Micro.BindableCollection<ChatUser>(users);

            Shell.ConnectedUsers.Sort((a, b) => a.LastMessageDate.CompareTo(b.LastMessageDate));
        }

        /// <summary>
        /// Обновить дату последнего сообщения в списке пользователей
        /// </summary>
        /// <param name="updatedID">Идентификатор обновляемого пользователя</param>
        /// <param name="updatedTime">Новая дата последнего сообщения</param>
        public void UpdateConnectedUsers(int updatedID, DateTime updatedTime)
        {
            var u = Shell.ConnectedUsers.FirstOrDefault(x => x.ID == updatedID);
            u.LastMessageDate = updatedTime;
            if (!u.IsMessageExist)
                u.IsMessageExist = true;

            Shell.ConnectedUsers.Sort((a, b) => a.LastMessageDate.CompareTo(b.LastMessageDate));
            Shell.UpdateConnectedList();
        }
        
        //Выбранный пользователь изменился
        public void SetSelectedUser(ChatUser User)
        {
            if(User != null)
                this.SelectedUser = User;
        }

        //Получение идентификатора выбранного пользователя
        public int GetSelectedUserID() => SelectedUser.ID;

        //Выбран ли хоть один пользователь
        public bool IsNoOneSelected() => SelectedUser == null ? true : false;

        //Добавить сообщение в общий список
        public void AddMessageToList(Message msg)
        {
            Shell.UpdateMessageList(msg);
        }

        //Запрос истории переписки
        public Message[] GetHistory()
        {
            Message[] Messages = Server.GetMessages(CurrentUser.ID, GetSelectedUserID());
            //Нахождение сообщений текущего пользователя в общем списке
            Messages.Where(x => x.SenderID == CurrentUser.ID).ToList().ForEach(x => x.IsSender = true);
            return Messages;
        }

        //Отправить сообщение
        public void Message(string message)
        {
            Message Message = MessageFactory(message);
            if (!string.IsNullOrEmpty(message))
            {
                UpdateConnectedUsers(Message.ReceiverID, Message.SendDate);
                Server.Message(Message);
                AddMessageToList(Message);
            }
        }

        //Фабрика сообщений
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
