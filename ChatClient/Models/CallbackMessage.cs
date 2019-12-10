using Client.Server;
using System;
using System.Windows;

namespace Client.Models
{
    /// <summary>
    /// Методы которые может вызывать сервис
    /// </summary>
    class CallbackMessage : IMessageContractCallback
    {
        public GeneralDataModel GeneralModel { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="general">Модель данных</param>
        public CallbackMessage(GeneralDataModel general)
        {
            GeneralModel = general;
        }

        /// <summary>
        /// Обработка получения сообщения
        /// </summary>
        /// <param name="Msg">Сообщение</param>
        public void GetMessage(Message Msg)
        {
            //Если отправитель - сервер
            if (Msg.SenderID == -1)
                DecodeServerMessage(Msg.MessageContent, Msg.SendDate);
            //Если отправитель другой пользователь
            else
            {
                if (!GeneralModel.IsNoOneSelected())
                    //Если отправитель выбран сейчас в списке контактов
                    if (Msg.SenderID == GeneralModel.GetSelectedUserID())
                    {
                        GeneralModel.AddMessageToList(Msg);
                        GeneralModel.UpdateConnectedUsers(Msg.SenderID, Msg.SendDate);
                    }
                    else
                    {
                        GeneralModel.UpdateConnectedUsers(Msg.SenderID, Msg.SendDate);
                    }
                else
                {
                    GeneralModel.UpdateConnectedUsers(Msg.SenderID, Msg.SendDate);
                }
            }
        }

        /// <summary>
        /// Обновление списка подключенных пользователей
        /// </summary>
        /// <param name="Users">Массив пользователей</param>
        public void UpdateConnected(ChatUser[] Users)
        {
            GeneralModel.UpdateConnectedUsers(Users);
        }

        /// <summary>
        /// Обработка сообщений сервера
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="time">Дата отправления</param>
        public void DecodeServerMessage(string message, DateTime time)
        {
            switch (message)
            {
                case "unregistred":
                    {
                        MessageBox.Show($"{time.ToShortTimeString()}: Incorrect login or password!", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        break;
                    }
                case "alreadylogged":
                    {
                        MessageBox.Show($"{time.ToShortTimeString()}: This user is already logged!", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        break;
                    }
                case "connect":
                    {
                        //Обновить текущего пользователя в случае успешной авторизации
                        GeneralModel.UpdateCurrentUser();
                        break;
                    }
                default:
                    {
                        MessageBox.Show($"{time.ToShortTimeString()}: Unknown server answer!", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        break;
                    }
            }
        }
    }
}
