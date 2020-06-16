using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectControlSystem.Windows
{
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow(string text)
        {
            InitializeComponent();

            info.Text = text;
        }
        public ExceptionWindow(Exception ex)
        {
            InitializeComponent();

            info.Text = GetText(ex);
        }

        string GetText(Exception ex)
        {
            string value = ex.Message + "\n";
            value += ex.StackTrace;

            if (ex.InnerException != null)
                value += "\n-------------\n" + GetText(ex.InnerException);

            return value;
        }

        #region Обработчики сигналов.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void SendButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //Адрес SMTP-сервера
        //    string smtpHost = "smtp.mail.ru";
        //    //Порт SMTP-сервера
        //    int smtpPort = 25;

        //    //Создание подключения
        //    SmtpClient client = new SmtpClient(smtpHost, smtpPort);

        //    //Адрес для поля "От"
        //    string msgFrom = "LOGIN@SERVER.RU";
        //    //Адрес для поля "Кому" (адрес получателя)
        //    string msgTo = "s.naiden@rlt.ru";
        //    //Тема письма
        //    string msgSubject = "Письмо от C#";
        //    //Текст письма
        //    string msgBody = "Привет!\r\n\r\nЭто тестовое письмо\r\n\r\n--\r\nС уважением, C# :-)";

        //    //Создание сообщения
        //    var message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);

        //    try
        //    {
        //        //Отсылаем сообщение
        //        client.Send(message);
        //    }
        //    catch (SmtpException ex)
        //    {
        //        //В случае ошибки при отсылке сообщения можем увидеть, в чем проблема
        //        Console.WriteLine(ex.InnerException.Message.ToString());
        //    }
        //}

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            // регистрируем свой собственный формат данных либо получаем его, если он уже зарегистрирован
            IDataObject dataObj = new DataObject();
            dataObj.SetData(DataFormats.StringFormat, info.Text, true);
            Clipboard.SetDataObject(dataObj, false);
        }
        #endregion


        private void Main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
