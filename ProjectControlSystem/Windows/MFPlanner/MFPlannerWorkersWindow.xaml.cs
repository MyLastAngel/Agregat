using System;
using System.Collections.ObjectModel;
using System.Windows;
using ArgDb;
using ProjectControlSystem.Managers;
using ProjectControlSystem.Windows.MFPlanner;

namespace ProjectControlSystem.MFPlannerWindows
{
    public partial class MFPlannerWorkersWindow : Window
    {
        #region Поля
        #endregion

        #region Свойства
        #endregion

        public MFPlannerWorkersWindow()
        {
            InitializeComponent();

            DataContext = this;

            MFPlannerManager.WorkersChanged += MFPlannerManager_WorkersChanged;
            MFPlannerManager.ReloadWorkersAsync();
        }

        #region Обработчики событий
        // Изменились работники
        void MFPlannerManager_WorkersChanged(object sender, EventArgs e)
        {
            listWorkers.ItemsSource = null;
            listWorkers.ItemsSource = MFPlannerManager.GetWorkers(true);
        }

        // Добавляем работника
        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new MFPlannerWorkerEditWindow() { Owner = this };
            if (win.ShowDialog() != true)
                return;

            MFPlannerManager.CreateNewWorker(win.Result);
        }
        // Изменить работника
        void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var w = listWorkers.SelectedItem as MFWorker;
            if (w == null)
                return;

            var win = new MFPlannerWorkerEditWindow(w) { Owner = this };
            if (win.ShowDialog() != true)
                return;

            if (!MFPlannerManager.MFPlannerChangeWorker(win.Result))
                MessageBox.Show("Не удалось сохранить изменения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        // Уволить
        void btnEndWork_Click(object sender, RoutedEventArgs e)
        {
            var w = listWorkers.SelectedItem as MFWorker;
            if (w == null)
                return;

            var res = MessageBox.Show("Уверены что хотите уволить работника?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes)
                return;

            w.EndWorkTime = DateTime.Now.Date;
            if (!MFPlannerManager.MFPlannerChangeWorker(w))
                MessageBox.Show("Не удалось сохранить изменения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion


        // Удалить
        void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var w = listWorkers.SelectedItem as MFWorker;
            if (w == null)
                return;

            var res = MessageBox.Show("Уверены что хотите удалить работника?\nВсе связанные проекты удалятся тоже.\n(Пока не реализованно)", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes)
                return;

            //w.EndWorkTime = DateTime.Now.Date;
            //if (!MFPlannerManager.MFPlannerChangeWorker(w))
            //    MessageBox.Show("Не удалось сохранить изменения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
