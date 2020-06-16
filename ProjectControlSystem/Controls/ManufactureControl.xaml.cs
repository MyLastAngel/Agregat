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
    /// Interaction logic for ManufactureControl.xaml
    /// </summary>
    public partial class ManufactureControl : UserControl, IProjectViewControl
    {
        #region Поля
        #endregion

        #region Свойства
        public ProjectViewMode Mode { get { return ProjectViewMode.Manufacture; } }
        public string Header { get { return "Производство"; } }

        public Project SelectedProject { get { return listProjects.SelectedItem as Project; } }
        #endregion

        public ManufactureControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            if (listProjects.IsInitialized)
                listProjects.EndInit();

            listProjects.BeginInit();
            //listProjects.ItemsSource = null;
            listProjects.ItemsSource = ProjectManager.FiltredProjects;
            listProjects.EndInit();
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
        // Календарь изменился
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!popupCalendar.IsOpen)
                return;

            var project = popupCalendar.Tag as Project;
            if (project != null && calendar.Tag is ProjectPropertyType && calendar.SelectedDate.HasValue)
                ProjectManager.ChangeProjectPropety(project.ProjectID, (ProjectPropertyType)calendar.Tag, calendar.SelectedDate.Value);

            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }
        // выбор планируемого времени производства
        void btnMF_Time_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            popupCalendar.Tag = null;

            try
            {
                popupCalendar.PlacementTarget = btn;
                calendar.DisplayDateStart = DateTime.Now.AddDays(1);

                if (((Project)btn.Tag).MF_Time_Plan.HasValue)
                    calendar.SelectedDate = ((Project)btn.Tag).MF_Time_Plan.Value;

                calendar.Tag = ProjectPropertyType.MF_Time_Planed;
                popupCalendar.Tag = (Project)btn.Tag;

                popupCalendar.IsOpen = true;
            }
            catch { }
            finally
            {
                //calendar.SelectedDatesChanged += calendar_SelectedDatesChanged;
            }
        }

        //void btnMF_Time_Test_Actual_Click(object sender, RoutedEventArgs e)
        //{
        //    var element = sender as Button;
        //    if (element == null || !(element.Tag is Project))
        //        return;

        //    ProjectManager.ChangeProjectPropety(((Project)element.Tag).ID, ProjectPropertyType.MF_Time_Test_Actual, DateTime.Now);
        //}

        // Фокус у поста изменился
        void txtPost_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null || !(txt.Tag is Project))
                return;

            // проврека на совпадение значений
            string oldValue = ((Project)txt.Tag).MF_Post, newValue = txt.Text;
            if (oldValue == newValue)
                return;

            ProjectManager.ChangeProjectPropety(((Project)txt.Tag).ProjectID, ProjectPropertyType.MF_Post, newValue);
        }
        // Прогресс изменился
        void cmbMF_Complete_Percentage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_Complete_Percentage;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_Complete_Percentage, (string)cmb.SelectedItem);
        }

        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is Project))
                return;

            var win = new CommentWindow((Project)btn.Tag) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        void cmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_Level;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_Level, (string)cmb.SelectedItem);

            Init();
        }
        void cmbRama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_Rama;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_Rama, (string)cmb.SelectedItem);
        }
        // коллектор
        void cmbCollector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_Collector;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_Collector, (string)cmb.SelectedItem);
        }
        void cmbAgregat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_Agregat;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_Agregat, (string)cmb.SelectedItem);
        }
        //void cmbTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var cmb = sender as ComboBox;
        //    if (cmb == null || !(cmb.Tag is Project))
        //        return;


        //    ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ID, ProjectPropertyType.MF_Test, cmb.SelectedItem as string);
        //}
        void cmbSH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null || !(cmb.Tag is Project) || !(cmb.SelectedItem is string))
                return;

            // проврека на совпадение значений
            var oldValue = ((Project)cmb.Tag).MF_SH;
            if (oldValue == (string)cmb.SelectedItem)
                return;

            ProjectManager.ChangeProjectPropety(((Project)cmb.Tag).ProjectID, ProjectPropertyType.MF_SH, (string)cmb.SelectedItem);
        }

        // недокомплект
        void btnWH_G_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_G, OMTSRequestType.Hydraulics, (Project)btn.Tag);
        }
        void btnWH_E_Request_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is Project))
                return;

            EditRequest(ProjectPropertyType.Requests_E, OMTSRequestType.Electrician, (Project)btn.Tag);
        }

        // Прокрутка когда источник изменился
        void listProjects_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listProjects.SelectedItem != null)
                listProjects.ScrollIntoView(listProjects.SelectedItem);
        }
        #endregion

        #region События.
        public event PropertyChangedEventHandler PropertyChanged;
        void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion


    }
}
