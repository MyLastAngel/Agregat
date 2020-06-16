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
    /// <summary>
    /// Interaction logic for ChangeDateWindow.xaml
    /// </summary>
    public partial class ChangeDateWindow : Window
    {
        #region Поля
        bool isNeedComment = true;
        #endregion

        #region Свойства
        public DateTime? NewDate { get { return calendar.SelectedDate; } }
        public string NewComment { get { return txtComment.Text; } }
        #endregion

        public ChangeDateWindow(DateTime timeStart, bool isNeedComment = true)
        {
            InitializeComponent();

            this.isNeedComment = isNeedComment;

            if (isNeedComment)
                gBoxComment.Header = "Введите коммнетарий:";
            else
                gBoxComment.Header = "Коммнетарий (не обязательно):";

            calendar.DisplayDateStart = timeStart.AddDays(1);
        }

        bool Check()
        {
            if (!NewDate.HasValue)
            {
                MessageBox.Show("Выберите новую дату", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (isNeedComment && string.IsNullOrEmpty(NewComment))
            {
                MessageBox.Show("Введите комментарий", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            return true;
        }

        #region Обработчики событий
        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Check())
                return;


            DialogResult = true;
            Close();
        }
        #endregion
    }
}
