using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Client.Models;
using Client.Server;

namespace Client.ViewModels
{
    class ShellViewModel : Conductor<IScreen>
    {
        #region private fields
        private bool _isUserLogged;
        private ChatUser _currentUser;
        private AuthViewModel _currentAuthVM;
        private ChatViewModel _currentChatVM;
        private BindableCollection<ChatUser> _connectedUsers;
        private GeneralDataModel _generalModel;
        private ChatUser _selectedConnectedUser;
        #endregion

        public bool IsUserLogged
        {
            get { return _isUserLogged; }
            set
            {
                _isUserLogged = value;
                if (value)
                {
                    _currentChatVM = new ChatViewModel(this);
                    ActiveItem = _currentChatVM;
                }
                else
                {
                    ActiveItem = new AuthViewModel(this, GeneralModel);
                }
                NotifyOfPropertyChange(nameof(CanAuth));
                NotifyOfPropertyChange(nameof(CanMessage));
            }
        }

        //Основная модель
        public GeneralDataModel GeneralModel
        {
            get => _generalModel;
            set => _generalModel = value;
        }

        //Коллекция подключенных пользователей
        public BindableCollection<ChatUser> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
            set
            {
                _connectedUsers = value;

                if (_currentChatVM != null)
                {
                    _currentChatVM.ConnectedUsers = _connectedUsers;
                }
            }
        }

        public void KeepSelectedStore()
        {
            if(_currentChatVM.SelectedConnectedUser != null)
            {
                _currentChatVM.StoredID = _currentChatVM.SelectedConnectedUser.ID;
            }
        }

        public void KeepSelectedSet()
        {
            if(_currentChatVM.ConnectedUsers != null)
            {
                var a = _currentChatVM.ConnectedUsers.FirstOrDefault(x => x.ID == _currentChatVM.StoredID);
                if (a != null)
                {
                    _currentChatVM.SelectedConnectedUser = a;
                }
            }
        }

        //Текущий выбранный пользователь-получатель
        public ChatUser SelectedConnectedUser
        {
            get
            {
                return _selectedConnectedUser;
            }
            set
            {
                _selectedConnectedUser = value;
                GeneralModel.SetSelectedUser(value);
            }
        }

        //Текущий пользователь
        public ChatUser CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        //Конструктор
        public ShellViewModel()
        {
            GeneralModel = new GeneralDataModel(this);
            _currentAuthVM = new AuthViewModel(this, GeneralModel);
            ActiveItem = _currentAuthVM;
        }

        //Обновить список сообщений
        public void UpdateMessageList(Server.Message msg)
        {
            _currentChatVM.MessageList.Add(msg);
            if (_currentChatVM != null)
            {
                _currentChatVM.UpdateMessageList();
            }
        }

        //Обновить список подключенных пользователей
        public void UpdateConnectedList()
        {
            if(_currentChatVM != null)
            {
                _currentChatVM.UpdateConnectedList();
            }
        }

        //Блокировка кнопок навигации пока пользователь не войдет
        public bool CanAuth => IsUserLogged;
        public bool CanMessage => IsUserLogged;

        //Обработка кнопки выхода
        public void Auth()
        {
            IsUserLogged = false;
            GeneralModel.Disconnect();
        }

        //Обработка кнопки меню сообщений
        public void Message()
        {
            ActiveItem = _currentChatVM;
        }

        //Обработка закрытия формы
        protected override void OnDeactivate(bool close)
        {
            GeneralModel.Disconnect();
        }
    }
}
