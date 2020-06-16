using ArgDb;
using ProjectControlSystem.Managers;
using ProjectControlSystem.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectControlSystem.Controls
{
    /// <summary>
    /// Interaction logic for OTKControl.xaml
    /// </summary>
    public partial class OTKControl : UserControl, IProjectViewControl, INotifyPropertyChanged
    {
        #region Поля
        #endregion

        #region Свойства
        public ProjectViewMode Mode { get { return ProjectViewMode.OTK; } }
        public string Header { get { return "Отдел Технического контроля"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public OTKControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            listProjects.ItemsSource = ProjectManager.FiltredProjects;
        }
        public void Unload()
        {
            listProjects.ItemsSource = null;
        }

        public void Select(int pID)
        {
            var sProject = ProjectManager.FiltredProjects.SingleOrDefault(x => x.ProjectID == pID);
            if (sProject != null)
            {
                listProjects.SelectedItem = sProject;
                listProjects.ScrollIntoView(sProject);
            }
        }

        // Редактируем недокомплекты
        void EditRequest(ProjectPropertyType type, OMTSRequestType rType, Project project)
        {
            var win = new RequestWindow(rType, project) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() != true)
                return;
        }

        #region Обработчики событий
        void btnOTK_G_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить дату испытания 'Гидравлики (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.OTK_Hydraulics_Time_Actual, DateTime.Now);
        }
        void btnOTK_E_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить дату испытания 'Электрики (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.OTK_Electrician_Time_Actual, DateTime.Now);
        }

        // Выбор времени для испытания шкафа
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!popupCalendar.IsOpen)
                return;

            var project = calendar.Tag as Project;
            if (project == null || !calendar.SelectedDate.HasValue || !(popupCalendar.Tag is ProjectPropertyType))
                return;

            var name = "Гидравлики (факт)";
            if ((ProjectPropertyType)popupCalendar.Tag == ProjectPropertyType.OTK_Electrician_Time_Actual)
                name = "Электрики (факт)";

            var result = MessageBox.Show(string.Format("Установить дату '{0}' испытания '{1}' для проекта: '{2}'?",
               calendar.SelectedDate.Value.ToString(ProjectConfiguration.DateFormat), name, project.ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(project.ProjectID, (ProjectPropertyType)popupCalendar.Tag, calendar.SelectedDate.Value);

            popupCalendar.Tag = null;
            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }
        void mnuOTK_Time_E_Select_Click(object sender, RoutedEventArgs e)
        {
            var btn = FindButton(sender as MenuItem);
            if (!(btn.Tag is Project))
                return;

            calendar.Tag = null;
            popupCalendar.Tag = ProjectPropertyType.OTK_Electrician_Time_Actual;

            // устанавливаем значения
            popupCalendar.PlacementTarget = btn;

            calendar.DisplayDateStart = DateTime.Now.Date.AddDays(-11);
            calendar.DisplayDateEnd = DateTime.Now.Date;
            calendar.Tag = (Project)btn.Tag;
            popupCalendar.IsOpen = true;
        }
        void mnuOTK_Time_G_Select_Click(object sender, RoutedEventArgs e)
        {
            var btn = FindButton(sender as MenuItem);
            if (!(btn.Tag is Project))
                return;

            calendar.Tag = null;
            popupCalendar.Tag = ProjectPropertyType.OTK_Hydraulics_Time_Actual;

            // устанавливаем значения
            popupCalendar.PlacementTarget = btn;

            calendar.DisplayDateStart = DateTime.Now.Date.AddDays(-11);
            calendar.DisplayDateEnd = DateTime.Now.Date;
            calendar.Tag = (Project)btn.Tag;
            popupCalendar.IsOpen = true;
        }

        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new CommentWindow((Project)btn.Tag) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        void mnuOTK_Not_Complete_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.MF_Time_Test_Actual, null);
        }

        void btnMF_Time_Test_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить дату постановки на тест для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.MF_Time_Test_Actual, DateTime.Now);
        }

        // Прокрутка когда источник изменился
        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
        }

        void mnuOTK_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_OTK_G_NotNeed, true);
        }
        void mnuOTK_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_OTK_E_NotNeed, true);
        }

        // недокомплект
        void btn_G_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_G, OMTSRequestType.Hydraulics, (Project)btn.Tag);
        }
        void btn_E_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_E, OMTSRequestType.Electrician, (Project)btn.Tag);
        }

        // Время план
        void btnSet_Time_OTK_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.OTK_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        #endregion

        #region События.
        public event PropertyChangedEventHandler PropertyChanged;
        private void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Вспомогательные методы
        static Button FindButton(MenuItem mnu)
        {
            if (mnu == null || !(mnu.Parent is ContextMenu))
                return null;

            return ((ContextMenu)mnu.Parent).PlacementTarget as Button;
        }
        #endregion
    }
}
