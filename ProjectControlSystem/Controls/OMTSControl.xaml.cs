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
    /// Interaction logic for OMTSControl.xaml
    /// </summary>
    public partial class OMTSControl : UserControl, IProjectViewControl, INotifyPropertyChanged
    {
        #region Поля

        #endregion

        #region Свойства
        public ProjectViewMode Mode { get { return ProjectViewMode.OMTS; } }
        public string Header { get { return "Отдел Материально Технического Снабжения"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public OMTSControl()
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

        #region Обработчики событий
        void btnOMTS_G_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new RequestWindow(OMTSRequestType.Hydraulics, (Project)btn.Tag, true) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() != true)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.Requests_G, win.Requests);
        }
        void btnOMTS_E_Complete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new RequestWindow(OMTSRequestType.Electrician, (Project)btn.Tag, true) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() != true)
                return;

            ProjectManager.ChangeProjectPropety(((Project)btn.Tag).ProjectID, ProjectPropertyType.Requests_E, win.Requests);
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

        // Изменение даты план
        void btnSet_Time_OMTS_G_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.OMTS_G_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
        }
        void btnSet_Time_OMTS_E_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            ProjectManager.SetPlanDate(ProjectPropertyType.OMTS_E_Time_Plan, ((Project)btn.Tag), Window.GetWindow(this));
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
