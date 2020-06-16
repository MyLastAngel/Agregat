using System.Windows;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;

namespace ProjectControlSystem.MFPlannerWindows
{
    public partial class MFPlannerSelectProjectWindow : Window
    {
        #region Поля
        // время по старту
        DateTime initStartTime = DateTime.MinValue, initEndTime = DateTime.MinValue;
        #endregion

        #region Свойства
        public Project SelectedProject { get; private set; }
        #endregion

        public MFPlannerSelectProjectWindow(DateTime sTime, DateTime eTime)
        {
            InitializeComponent();

            initStartTime = sTime;
            initEndTime = eTime;

            cFilter.FilterCleared += cFilter_FilterCleared;
            cFilter.FilterConfirmed += cFilter_FilterConfirmed;

            listProjects.ItemsSource = ServiceManager.LoadProjects(sTime, eTime);
        }

        #region Обработчики событий
        void cFilter_FilterConfirmed(object sender, EventArgs e)
        {

            var projects = ServiceManager.LoadProjects(initStartTime, initEndTime);

            var filtred = new List<Project>();
            if (projects != null)
            {
                foreach (var p in projects)
                {
                    if (!cFilter.IsFilterPassed(p))
                        continue;

                    filtred.Add(p);
                }
            }

            listProjects.ItemsSource = null;
            listProjects.ItemsSource = filtred;

        }
        void cFilter_FilterCleared(object sender, EventArgs e)
        {
            listProjects.ItemsSource = null;
            listProjects.ItemsSource = ServiceManager.LoadProjects(initStartTime, initEndTime);
        }

        // Сохранение
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var p = listProjects.SelectedItem as Project;
            if (p == null)
            {
                MessageBox.Show("Ничего не выбранно", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            SelectedProject = p;
            DialogResult = true;
            Close();
        }
        #endregion
    }
}
