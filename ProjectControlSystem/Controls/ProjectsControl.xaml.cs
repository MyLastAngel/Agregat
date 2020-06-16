using ProjectControlSystem.Managers;
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
    /// Interaction logic for ProjectsControl.xaml
    /// </summary>
    public partial class ProjectsControl : UserControl, IProjectViewControl, INotifyPropertyChanged
    {
        #region Поля

        #endregion

        #region Cвойства
        public ProjectViewMode Mode { get { return ProjectViewMode.Main; } }
        public string Header { get { return "Система управления проектами"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public ProjectsControl()
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
        private void test_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Мышка кликнула");
        }

        // Прокрутка когда источник изменился
        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
        }

        // прокручиваем двойной заголовок
        void listProjects_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Интерестна только горизонтальная прокрутка
            if (e.HorizontalChange == 0)
                return;


            foreach (Border control in gridDoubleHeaderControls.Children.OfType<Border>())
                control.Margin = new Thickness(control.Margin.Left - e.HorizontalChange, control.Margin.Top, control.Margin.Right, control.Margin.Bottom);
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
