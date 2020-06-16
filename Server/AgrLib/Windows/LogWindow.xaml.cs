using ArgLib.Logger;
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

namespace ArgLib.Windows
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow(Log log)
        {
            InitializeComponent();

            DataContext = log;
        }

        #region Обработчики событий
        void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        void btnClean_Click(object sender, RoutedEventArgs e)
        {
            var log = DataContext as Log;
            if (log == null || log.Logs.Count == 0)
                return;

            log.Logs.Clear();
        }
        void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            var log = DataContext as Log;
            if (log == null || log.Logs.Count == 0)
                return;

            var strBuider = new StringBuilder();

            for (int i = 0; i < log.Logs.Count; i++)
            {
                string type = log.Logs[i].Type == LogType.Error ? "\n !ОШИБКА \n" : "";
                strBuider.Append(i + " " + type + " - модуль: " + log.Logs[i].Unit + ", текст: " + log.Logs[i].Description + "\n");

                if (log.Logs[i].Type == LogType.Error)
                    strBuider.Append(CreateException(log.Logs[i].Ex));
            }

            IDataObject dataObj = new DataObject();
            dataObj.SetData(DataFormats.StringFormat, strBuider.ToString(), true);
            Clipboard.SetDataObject(dataObj, false);
        }
        #endregion

        static string CreateException(Exception ex)
        {
            string str = "";

            if (ex == null)
                return "";

            str = ex.Message + " \n " + ex.StackTrace + "\n" + CreateException(ex.InnerException) + "\n";
            return str;
        }
    }
}
