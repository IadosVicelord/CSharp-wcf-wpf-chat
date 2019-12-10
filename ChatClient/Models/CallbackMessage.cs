using Client.Server;
using System;
using System.Windows;

namespace Client.Models
{
    class CallbackMessage : IMessageContractCallback
    {
        public GeneralDataModel GeneralModel { get; set; }

        //Конструктор
        public CallbackMessage(GeneralDataModel general)
        {
            GeneralModel = general;
        }
        
        //Обработка получения сообщения
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

        //Обновить список подключенных пользователей
        public void UpdateConnected(ChatUser[] Users)
        {
            GeneralModel.UpdateConnectedUsers(Users);
        }

        //Обработка сообщений сервера
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
