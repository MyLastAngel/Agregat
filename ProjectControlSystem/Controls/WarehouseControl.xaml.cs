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
    /// Interaction logic for WarehouseControl.xaml
    /// </summary>
    public partial class WarehouseControl : UserControl, IProjectViewControl, INotifyPropertyChanged
    {
        #region Поля

        #endregion

        #region Свойства
        public ProjectViewMode Mode { get { return ProjectViewMode.Warehouse; } }
        public string Header { get { return "Склад"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public WarehouseControl()
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

            ProjectManager.ChangeProjectPropety(project.ProjectID, type, win.Requests);
        }

        #region Обработчики событий
        void btnWH_G_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            // Если нет задолженностей
            if (((Project)btn.Tag).Is_WH_G_CanComplete == true && ((Project)btn.Tag).Time_ITO_G_Actual.HasValue)
            {
                var result = MessageBox.Show(string.Format("Установить время 'Гидравлика (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.WH_Hydraulics_Time_Actual, DateTime.Now);
            }
            else    //открываем окно недокомплекта
                EditRequest(ProjectPropertyType.Requests_G, OMTSRequestType.Hydraulics, (Project)btn.Tag);
        }
        void btnWH_E_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            // Если нет задолженностей
            if (((Project)btn.Tag).Is_WH_E_CanComplete == true && ((Project)btn.Tag).Time_ITO_E_Actual.HasValue)
            {
                var result = MessageBox.Show(string.Format("Установить время 'Электрика (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.WH_Electrician_Time_Actual, DateTime.Now);
            }
            else    //открываем окно недокомплекта
                EditRequest(ProjectPropertyType.Requests_E, OMTSRequestType.Electrician, (Project)btn.Tag);
        }
        void btnWH_R_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить время 'Рама (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.WH_Rama_Time_Actual, DateTime.Now);
        }

        void mnuWH_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            var currentProject = ((Project)mnu.Tag);
            if (currentProject.Requests.Any(x => x.Type == OMTSRequestType.Hydraulics))
            {
                MessageBox.Show("Невозможно установить свойство 'Не требуется' для Гидравлики в проекте с недокомплектом", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            ProjectManager.ChangeProjectPropety(currentProject.ProjectID, ProjectPropertyType.Is_WH_G_NotNeed, true);
        }
        void mnuWH_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            var currentProject = ((Project)mnu.Tag);
            if (currentProject.Requests.Any(x => x.Type == OMTSRequestType.Electrician))
            {
                MessageBox.Show("Невозможно установить свойство 'Не требуется' для Электрики в проекте с недокомплектом", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            ProjectManager.ChangeProjectPropety(currentProject.ProjectID, ProjectPropertyType.Is_WH_E_NotNeed, true);
        }
        void mnuWH_R_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_WH_R_NotNeed, true);
        }

        // добавляем недокомлект электрика
        void btnWH_E_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_E, OMTSRequestType.Electrician, (Project)btn.Tag);
        }
        // добавляем недокомлект гидравлика
        void btnWH_G_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_G, OMTSRequestType.Hydraulics, (Project)btn.Tag);
        }

        void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Завершить проект: '{0}'", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;


            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.CompleteProject, DateTime.Now);
        }
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new CommentWindow((Project)btn.Tag) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        // Прокрутка когда источник изменился
        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
        }

        // установка времени план
        void btnSet_Time_WH_G_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.WH_G_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        void btnSet_Time_WH_E_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.WH_E_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        void btnSet_Time_WH_R_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.WH_R_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
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
    }
}
