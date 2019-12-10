using Caliburn.Micro;
using Client.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    class ChatViewModel : Screen
    {
        #region fields
        private BindableCollection<ChatUser> _connectedUsers;
        private ChatUser _selectedConnectedUser;
        private string _message;
        private ShellViewModel _shell;
        private BindableCollection<Server.Message> _messageList;
        #endregion

        //Форма-контейнер
        public ShellViewModel Shell
        {
            get => _shell;
            set => _shell = value;
        }

        //Выбранный пользователь-получатель
        public ChatUser SelectedConnectedUser
        {
            get
            {
                return _selectedConnectedUser;
            }
            set
            {
                _selectedConnectedUser = value;
                if (value != null)
                {
                    Shell.SelectedConnectedUser = value;
                    MessageList = new BindableCollection<Server.Message>(Shell.GeneralModel.GetHistory());
                    UpdateMessageList();
                }
            }
        }

        //Поле сообщения пользователя
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyOfPropertyChange(Message);
            }
        }

        //Список сообщений
        public BindableCollection<Server.Message> MessageList
        {
            get
            {
                return _messageList;
            }
            set
            {
                _messageList = value;
            }
        }

        //Список подключенных пользователей
        public BindableCollection<ChatUser> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
            set
            {
                _connectedUsers = value;
                NotifyOfPropertyChange(() => this.ConnectedUsers);
            }
        }

        //Конструктор
        public ChatViewModel(ShellViewModel shell)
        {
            Shell = shell;
        }

        //Оповещение об обновлении списка сообщений
        public void UpdateMessageList() => NotifyOfPropertyChange(() => this.MessageList);

        public void UpdateConnectedList() => NotifyOfPropertyChange(() => this.ConnectedUsers);

        //Блокировка кнопки отправить при пустом поле сообщения
        public bool CanSend(string message) => string.IsNullOrEmpty(message) || SelectedConnectedUser == null ? false : true;

        //Обработка кнопки отправить
        public void Send(string message)
        {
            Shell.GeneralModel.Message(message);
            Message = string.Empty;
        }
    }
}
