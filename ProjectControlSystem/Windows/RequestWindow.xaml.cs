using ArgDb;
using ArgLib.Logger;
using Microsoft.Win32;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectControlSystem.Windows
{
    public partial class RequestWindow : Window
    {
        #region Поля
        const string unit = "RequestWindow";

        static bool isExpanderProjectOpen = true;

        OMTSRequestType type = OMTSRequestType.Unknown;
        Project currentProject = null;

        readonly ObservableCollection<OMTSRequest> requests = new ObservableCollection<OMTSRequest>();
        #endregion

        #region Свойства
        public ObservableCollection<OMTSRequest> Requests { get { return requests; } }

        public bool IsReadOnly
        {
            set
            {
                mnuContext.Visibility =
                btnOk.Visibility =
                barMenu.Visibility = value == true ? Visibility.Collapsed : Visibility.Visible;

                listRequest.IsReadOnly = value;
            }
        }

        public string CurrentUser { get { return UserManager.Name; } }
        #endregion

        public RequestWindow(OMTSRequestType t, Project p, bool isCanComplete = false)
        {
            InitializeComponent();

            type = t;
            listRequest.Tag = isCanComplete;

            currentProject = p;

            expProject.IsExpanded = isExpanderProjectOpen;

            InitData(p);

            DataContext = this;
        }

        void InitData(Project currentProject)
        {
            cProjectAbout.DataContext = currentProject;

            switch (type)
            {
                #region Electrician
                case OMTSRequestType.Electrician:
                    {
                        Title = "Карта недокомплекта Электрика";


                        break;
                    }
                #endregion
                #region Hydraulics
                case OMTSRequestType.Hydraulics:
                    {
                        Title = "Карта недокомплекта Гидравлика";

                        break;
                    }
                    #endregion
            }

            requests.Clear();

            var rTemp = currentProject.Requests.Where(x => x.Type == type);
            foreach (var r in rTemp)
                requests.Add(r.Clone());

            listRequest.ItemsSource = Requests;
        }

        #region Обработчики событий
        // Добавляем значение
        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new NewRequestWindow { Owner = this };
            if (win.ShowDialog() != true)
                return;

            var item = requests.SingleOrDefault(x => x.Name == win.NewDetails && x.Article == win.NewArticle && x.Type == type);
            if (item == null)
                requests.Add(new OMTSRequest(type, win.NewDetails, win.NewCount) { Article = win.NewArticle });
            else // если такой уже есть добавляем количество
                item.TotalCount += win.NewCount;
        }
        /// <summary>Вернуть недокомплект</summary>
        void btnRevert_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Control;
            if (btn == null || !(listRequest.SelectedItem is OMTSRequest))
                return;

            var r = (OMTSRequest)listRequest.SelectedItem;

            var rez = MessageBox.Show(string.Format("Уверены что хотите вернуть долг: '{0}'", r.Name), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rez != MessageBoxResult.Yes)
                return;

            r.ExistCount = 0;
            r.DateComplete = r.DateComplete_Plan = null;
        }
        // Удаление
        void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Control;
            if (btn == null || !(listRequest.SelectedItem is OMTSRequest))
                return;

            var r = (OMTSRequest)listRequest.SelectedItem;
            if (r.DebtCount == 0)
                return;

            var rez = MessageBox.Show(string.Format("Уверены что хотите удалить: '{0}'", r.Name), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rez != MessageBoxResult.Yes)
                return;

            requests.Remove(r);
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Control;
            if (btn == null || !(btn.Tag is OMTSRequest))
                return;

            var request = (OMTSRequest)btn.Tag;

            var win = new NewRequestWindow(false)
            {
                Owner = this,
                NewCount = request.DebtCount,
                NewDetails = request.Name,
                DebtCount = request.DebtCount,
                NewArticle = request.Article
            };
            if (win.ShowDialog() != true)
                return;

            // Если частично пришли
            if (request.TotalCount != win.NewCount)
            {
                var count = request.TotalCount - Math.Min(request.TotalCount, win.NewCount);
                var rNew = new OMTSRequest(request.Type, request.Name, count) { Article = win.NewArticle };
                requests.Add(rNew);

                request.ExistCount = request.TotalCount = win.NewCount;
            }
            else  // Если пришли все то закрываем
                request.ExistCount = request.TotalCount;
        }

        // Загрузка файла недокомплекта Excel
        void btnLoadFromExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog { Filter = "Файлы Excel|*.xls" };
                if (dlg.ShowDialog(this) != true)
                    return;

                var rTemp = ExcelManager.LoadRequests(type, dlg.FileName);
                foreach (var r in rTemp)
                {
                    var item = requests.SingleOrDefault(x => x.Name == r.Name && x.Article == r.Article && x.Type == type);
                    if (item == null)
                        requests.Add(r);
                    else // если такой уже есть добавляем количество
                        item.TotalCount += r.TotalCount;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Ошибка загрузки файла Excel: {0}", ex.Message);
                MessageBox.Show(msg, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);

                LogManager.LogError(unit, msg, ex);
            }
        }
        // Сохранение в Excel
        void btnSaveToExcel_Click(object sender, RoutedEventArgs e)

        {
            ExcelManager.SaveRequestFormat(currentProject, requests.ToList());
        }

        // Открыть календарь
        void btnRequestDeliveryPlan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is OMTSRequest))
                return;

            calendar.Tag = null;

            // устанавливаем значения
            popupCalendar.PlacementTarget = btn;

            if (((OMTSRequest)btn.Tag).DateComplete_Plan.HasValue)
                calendar.SelectedDate = ((OMTSRequest)btn.Tag).DateComplete_Plan.Value;

            calendar.Tag = ((OMTSRequest)btn.Tag);
            popupCalendar.IsOpen = true;
        }
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!popupCalendar.IsOpen)
                return;

            var r = calendar.Tag as OMTSRequest;
            if (r == null)
                return;

            if (calendar.SelectedDate.HasValue)
                r.DateComplete_Plan = calendar.SelectedDate.Value;

            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            isExpanderProjectOpen = expProject.IsExpanded;

            DialogResult = true;
            Close();
        }
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion


    }
}
