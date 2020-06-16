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
    /// Interaction logic for ITOControl.xaml
    /// </summary>
    public partial class ITOControl : UserControl, IProjectViewControl, INotifyPropertyChanged
    {
        #region Поля
        //static readonly List<Project> items = new List<Project>{
        //    new Project("Проект 1"),
        //    new Project("Проект 2"),
        //    new Project("Проект 3"),
        //    new Project("Проект 4"),
        //     new Project("Проект 5"),
        //    new Project("Проект 6"),
        //    new Project("Проект 7"),
        //    new Project("Проект 8"),
        //     new Project("Проект 1"),
        //    new Project("Проект 2"),
        //    new Project("Проект 3"),
        //    new Project("Проект 4"),
        //     new Project("Проект 1"),
        //    new Project("Проект 2"),
        //    new Project("Проект 3"),
        //    new Project("Проект 4"),
        //     new Project("Проект 1"),
        //    new Project("Проект 2"),
        //    new Project("Проект 3"),
        //    new Project("Проект 4"),
        //};
        #endregion

        #region Свойства
        public ProjectViewMode Mode { get { return ProjectViewMode.ITO; } }
        public string Header { get { return "Отдел Инженерно Технического Обеспечения"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public ITOControl()
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
        void btnITO_G_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить время 'Гидравлические схемы (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.ITO_Hydraulics_Time_Actual, DateTime.Now);
        }
        void btnITO_E_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить время 'Электрические схемы (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.ITO_Electrician_Time_Actual, DateTime.Now);
        }
        void btnITO_R_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var result = MessageBox.Show(string.Format("Установить время 'Рама (факт)' для проекта: '{0}'?", ((Project)btn.Tag).ID), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.ITO_Rama_Time_Actual, DateTime.Now);
        }

        void mnuITO_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_ITO_G_NotNeed, true);
        }
        void mnuITO_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_ITO_E_NotNeed, true);
        }
        void mnuITO_R_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as MenuItem;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.Is_ITO_R_NotNeed, true);
        }

        // место изготовления шу
        void cmbMF_SH_Place_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_SH_Place;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_SH_Place, (string)cmb.SelectedItem);
        }

        // коментарии
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new CommentWindow((Project)btn.Tag) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        void cmbITO_R_Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).ITO_R_Mode;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.ITO_R_Mode, (string)cmb.SelectedItem);
        }

        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
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

        // Установка времени план
        void btnSet_Time_ITO_G_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.ITO_G_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        void btnSet_Time_ITO_E_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.ITO_E_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        void btnSet_Time_ITO_R_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.ITO_R_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
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
