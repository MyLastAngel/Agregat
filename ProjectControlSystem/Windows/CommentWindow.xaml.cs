using ProjectControlSystem.Managers;
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
    /// Interaction logic for CommentWindow.xaml
    /// </summary>
    public partial class CommentWindow : Window
    {
        #region Поля
        Project targetProject = null;
        #endregion

        public CommentWindow(Project p)
        {
            InitializeComponent();

            targetProject = p;
            listMessages.ItemsSource = targetProject.Messages;
        }

        #region Обработчики событий
        // импорт в Excel
        void btnToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (targetProject.Messages.Count ==0)
            {
                MessageBox.Show(this, "Нет комментариев для печати", "Внимание",  MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ExcelManager.SaveComment(targetProject);
        }

        void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if( UserManager.IsGuest)
           {
               MessageBox.Show(this, "Гость не может оставлять сообщения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
               return;
           }

            if (string.IsNullOrEmpty(txtMessage.Text))
            {
                MessageBox.Show(this, "Введите сообщение", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            ProjectManager.AddComment(targetProject.ProjectID, txtMessage.Text);
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

       
    }
}
