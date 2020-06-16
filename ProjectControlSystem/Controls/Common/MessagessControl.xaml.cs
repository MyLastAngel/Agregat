using ProjectControlSystem.Managers;
using ProjectControlSystem.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProjectControlSystem
{
    public partial class MessagessControl : UserControl
    {
        #region Поля
        #endregion

        public MessagessControl()
        {
            InitializeComponent();

            listEvents.ItemsSource = ProjectManager.EventMessages;
            //parent = p;
        }

        #region Обработчики событий
        // Выбрать проект
        void mnuSelectProject_Click(object sender, RoutedEventArgs e)
        {
            var message = listEvents.SelectedItem as EventMessage;
            if (message == null)
                return;

            DoSelectedChanged(message.ProjectID);
        }
        void mnuClear_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.EventMessages.Clear();
        }

        #endregion

        #region События

        #region SelectedChanged
        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;
        public void DoSelectedChanged(int ID)
        {
            if (SelectedChanged != null)
                SelectedChanged(this, new SelectedChangedEventArgs(ID));
        }
        #endregion

        #endregion
    }
}
