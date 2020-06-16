using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjectControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for AlarmWindow.xaml
    /// </summary>
    public partial class AlarmWindow : Window
    {
        #region Поля
        readonly DispatcherTimer timerHide = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
        #endregion

        public AlarmWindow()
        {
            InitializeComponent();



            ProjectManager.EventMessages.CollectionChanged += EventMessages_CollectionChanged;
            timerHide.Tick += timerHide_Tick;
        }

        #region Перегрузка стандартных обработчиков сигналов
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var primaryArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            Left = primaryArea.Right - ActualWidth;
            Top = primaryArea.Bottom - ActualHeight;
        }
        #endregion

        #region Обработчики событий
        void btnShowMain_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            Hide();

            var msg = DataContext as EventMessage;
            if (msg != null)
                DoSelectedChanged(msg.ProjectID);
        }

        void EventMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!Owner.IsActive)
            {
                Show();

                // если запущен таймер скрытия оповещения
                if (ProjectConfiguration.IsHideAlertWin)
                {
                    timerHide.Interval = TimeSpan.FromMinutes(ProjectConfiguration.AlertShowTimeMin);
                    timerHide.Start();
                }
            }

            DataContext = null;
            if (ProjectManager.EventMessages.Count > 0)
                DataContext = ProjectManager.EventMessages[0];
        }

        void timerHide_Tick(object sender, EventArgs e)
        {
            timerHide.Stop();
            Hide();
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
