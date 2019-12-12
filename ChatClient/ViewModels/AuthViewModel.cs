using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.Server;

namespace Client.ViewModels
{
    class AuthViewModel : Screen
    {
        #region fields
        private string _username;
        private string _password;
        private bool _isUserLogged;
        private ShellViewModel _shell;
        private ChatUser _currentUser;
        private GeneralDataModel _generalModel;
        #endregion

        //Текущий пользователь
        public ChatUser CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        //Флаг вошел ли пользователь
        public bool IsUserLogged
        {
            get { return _isUserLogged; }
            set
            {
                _isUserLogged = value;
                if (value && CurrentUser != null)
                {
                    Shell.IsUserLogged = true;
                    Shell.CurrentUser = CurrentUser;
                }
                if (!value && CurrentUser != null)
                {
                    Shell.IsUserLogged = false;
                    Shell.CurrentUser = null;
                }
            }
        }

        //Имя пользователя
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(Username);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        //Пароль
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        //Основная модель
        public GeneralDataModel GeneralModel { get => _generalModel; set => _generalModel = value; }

        //Форма-контейнер
        public ShellViewModel Shell { get => _shell; set => _shell = value; }

        //Конструктор
        public AuthViewModel(ShellViewModel shell, GeneralDataModel generalModel)
        {
            Shell = shell;
            GeneralModel = generalModel;
        }

        //Блокировка кнопки входа до заполнения имени пользователя
        public bool CanLogIn
        {
            get
            {
                return (Username?.Length > 0 && Password?.Length > 0);
            }
        }

        //Обработка кнопки входа
        public void LogIn()
        {
            GeneralModel.Connect(Username, Password);
        }
    }
}
