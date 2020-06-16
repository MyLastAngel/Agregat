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
    /// Interaction logic for CommercialControl.xaml
    /// </summary>
    public partial class CommercialControl : UserControl, IProjectViewControl, INotifyPropertyChanged
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
        public ProjectViewMode Mode { get { return ProjectViewMode.Commercial; } }
        public string Header { get { return "Коммерческий отдел"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public CommercialControl()
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
            var win = new RequestWindow(rType, project) { Owner = Window.GetWindow(this), IsReadOnly = true };
            if (win.ShowDialog() != true)
                return;

            ProjectManager.ChangeProjectPropety(project.ProjectID, type, win.Requests);
        }

        #region Обработчики событий
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new CommentWindow((Project)btn.Tag) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        // запускает проект если тот в стопе
        void mnuPlay_Click(object sender, RoutedEventArgs e)
        {
            var p = listProjects.SelectedItem as Project;
            if (p == null)
                return;

            ProjectManager.ChangeProjectPropety(p.ProjectID, ProjectPropertyType.IsStop, false);
        }
        // останавливает проект
        void mnuStop_Click(object sender, RoutedEventArgs e)
        {
            var p = listProjects.SelectedItem as Project;
            if (p == null)
                return;

            ProjectManager.ChangeProjectPropety(p.ProjectID, ProjectPropertyType.IsStop, true);
        }

        // Смещаем время проекта
        void btnCom_New_Time_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new ChangeDateWindow(((Project)btn.Tag).TimeBegin) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() != true)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.Com_New_Time, win.NewDate);
            ProjectManager.AddComment(((Project)btn.Tag).ProjectID, win.NewComment);
        }

        // Прокрутка когда источник изменился
        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
        }

        // изменился тип упаковки
        void cmbPackage_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).COM_Package_Type;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.COM_Package_Type, (string)cmb.SelectedItem);
        }

        // Недокомплект Электрика/Гидравлика
        void btnWH_E_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            // Если нет задолженностей
            EditRequest(ProjectPropertyType.Requests_E, OMTSRequestType.Electrician, (Project)btn.Tag);
        }
        void btnWH_E_Close_With_Request_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as FrameworkElement;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.WH_Electrician_Time_Actual, DateTime.Now);
        }

        void btnWH_G_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            // Если нет задолженностей
            EditRequest(ProjectPropertyType.Requests_G, OMTSRequestType.Hydraulics, (Project)btn.Tag);
        }
        void btnWH_G_Close_With_Request_Click(object sender, RoutedEventArgs e)
        {
            var mnu = sender as FrameworkElement;
            if (mnu == null || !(mnu.Tag is Project))
                return;

            ProjectManager.ChangeProjectPropety(((Project)mnu.Tag).ProjectID, ProjectPropertyType.WH_Hydraulics_Time_Actual, DateTime.Now);
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
