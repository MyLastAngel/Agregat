using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ProjectControl.xaml
    /// </summary>
    public partial class ProjectControl : UserControl
    {
        #region Свойства
        public string NewCustomer
        {
            get { return cmbCustomer != null ? cmbCustomer.Text.Trim() : ""; }
            private set { cmbCustomer.Text = value; }
        }
        public string NewCustomerName
        {
            get { return txtCustomerName != null ? txtCustomerName.Text.Trim() : ""; }
            private set { txtCustomerName.Text = value; }
        }
        public string NewProduct
        {
            get { return txtProduct != null ? txtProduct.Text.Trim() : ""; }
            private set { txtProduct.Text = value; }
        }
        public string NewId
        {
            get { return txtProjectId != null ? txtProjectId.Text.Trim() : ""; }
            private set { txtProjectId.Text = value; }
        }
        public string NewOptions
        {
            get { return txtOptions != null ? txtOptions.Text.Trim() : ""; }
            private set { txtOptions.Text = value; }
        }
        public string NewComments
        {
            get { return txtComments != null ? txtComments.Text.Trim() : ""; }
            private set { txtComments.Text = value; }
        }
        public string NewPackageType
        {
            get { return cmbPackageType.SelectedItem as string; }
            private set { cmbPackageType.SelectedItem = value; }
        }

        public bool NewIsManagerSetPlanDate
        {
            get { return cmbIsManagerSetPlanDate.IsChecked == true; }
            private set { cmbIsManagerSetPlanDate.IsChecked = value; }
        }

        public DateTime NewStartTime
        {
            get { return (DateTime)txtStartDate.Tag; }
            private set
            {
                txtStartDate.Text = value.ToString("dd.MM.yyyy");
                txtStartDate.Tag = value;
            }
        }
        public DateTime NewEndTime
        {
            get { return (DateTime)txtFinishDate.Tag; }
            private set
            {
                txtFinishDate.Text = value.ToString("dd.MM.yyyy");
                txtFinishDate.Tag = value;
            }
        }
        #endregion

        public ProjectControl()
        {
            InitializeComponent();

            var customers = ServiceManager.GetClients();
            if (customers != null)
                cmbCustomer.ItemsSource = customers.Select(x => x.Name);

            ClearData();

            cmbPackageType.ItemsSource = Project.packageTypeItems;
        }

        public bool CheckValid()
        {
            if (string.IsNullOrEmpty(txtProjectId.Text))
            {
                MessageBox.Show("Не указан номер проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);

                txtProjectId.Focus();
                return false;
            }

            var customer = cmbCustomer.Text;
            if (string.IsNullOrEmpty(customer))
            {
                MessageBox.Show("Не указан заказчик", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);

                cmbCustomer.IsDropDownOpen = true;
                return false;
            }

            var product = txtProduct.Text;
            if (string.IsNullOrEmpty(product))
            {
                MessageBox.Show("Не указано изделие", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);

                txtProduct.Focus();
                return false;
            }

            // Время уходит в минус не остается времени на разработку
            // предложить самим планировать проекты
            if (NewIsManagerSetPlanDate == false)
            {
                //DateTime minEndDate10 = TimeManager.AddWorkDays(NewStartTime, 10).Date,
                //         minEndDate15 = TimeManager.AddWorkDays(NewStartTime, 15).Date;

                var days = TimeManager.WorkDaysCount(NewStartTime, NewEndTime);

                //if (minEndDate10 > NewEndTime.Date || minEndDate15 > NewEndTime.Date)
                if (days != 10 && days != 15 && days > 10)
                {
                    var res = MessageBox.Show("Недостаточно времени для автоматического планирования дат.\nВозможно не установлен флаг - 'Отделы сами планируют время'.\nПродолжить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res != MessageBoxResult.Yes)
                        return false;
                }
            }

            bool exist;
            // проверка на существование проекта
            if (!ServiceManager.CheckIsExist(NewId, NewCustomer, NewCustomerName, NewProduct, NewOptions, NewStartTime, NewEndTime, out exist))
            {
                MessageBox.Show(!ServiceManager.IsConnected ? "Нет связи с сервером" : "Проект уже существует",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            if (exist)
                return MessageBox.Show("Проект уже существует", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
            return true;
        }
        public bool CheckValid(NewProject p)
        {
            if (string.IsNullOrEmpty(p.Id))
            {
                MessageBox.Show("Не указан номер проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (string.IsNullOrEmpty(p.Customer))
            {
                MessageBox.Show("Не указан заказчик", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (string.IsNullOrEmpty(p.Product))
            {
                MessageBox.Show("Не указано изделие", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            bool exist;
            // проверка на существование проекта
            if (!ServiceManager.CheckIsExist(p.Id, p.Customer, p.CustomerName, p.Product, p.Options, p.StartTime, p.EndTime, out exist))
            {
                MessageBox.Show(!ServiceManager.IsConnected ? "Нет связи с сервером" : "Проект уже существует",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            if (exist)
                return MessageBox.Show("Проект уже существует", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
            return true;
        }

        public bool CheckIsSame(NewProject p)
        {
            if (NewCustomerName != p.CustomerName
                || NewProduct != p.Product
                || NewId != p.Id
                || NewOptions != p.Options
                || NewComments != p.Comments
                || NewPackageType != p.PackageType
                || NewCustomer != p.Customer

                || NewIsManagerSetPlanDate != p.IsManagerSetPlanDate

                || (DateTime)txtStartDate.Tag != p.StartTime
                || (DateTime)txtFinishDate.Tag != p.EndTime)
                return false;

            return true;
        }

        /// <summary>Сбрасывает введенные данные</summary>
        internal void ClearData()
        {
            NewCustomerName =
            NewProduct =
            NewId =
            NewOptions =
            NewComments =
            NewPackageType =
            NewCustomer = "";

            NewIsManagerSetPlanDate = false;

            var date = DateTime.Now;
            txtStartDate.Text = date.ToString("dd.MM.yyyy");
            txtStartDate.Tag = date;

            date = TimeManager.AddWorkDays(date, 15);

            txtFinishDate.Text = date.ToString("dd.MM.yyyy");
            txtFinishDate.Tag = date;
        }

        public void SetData(NewProject p)
        {
            NewCustomerName = p.CustomerName;
            NewProduct = p.Product;
            NewId = p.Id;
            NewOptions = p.Options;
            NewComments = p.Comments;
            NewPackageType = p.PackageType;
            NewCustomer = p.Customer;

            NewIsManagerSetPlanDate = p.IsManagerSetPlanDate;

            var date = p.StartTime;
            txtStartDate.Text = date.ToString("dd.MM.yyyy");
            txtStartDate.Tag = date;

            date = p.EndTime;

            txtFinishDate.Text = date.ToString("dd.MM.yyyy");
            txtFinishDate.Tag = date;
        }

        //Вставить данные из буфера
        public void SetState()
        {
            // Получаем обьект/ы из буфера обмена.
            var obj = Clipboard.GetDataObject();
            if (obj != null && obj.GetDataPresent("NewProjectWindowState"))
            {
                var state = obj.GetData("NewProjectWindowState") as SaveState;
                if (state != null)
                    state.RestoreState(this);
            }
        }
        //Сохранить введенные данные в буфер
        public void GetState()
        {
            var result = MessageBox.Show(Window.GetWindow(this), "Скопировать введенные данные в буфер", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Clipboard.Clear();

            // Копируем..
            var obj = new DataObject("NewProjectWindowState", new SaveState(this));
            Clipboard.SetDataObject(obj);
        }

        #region Обработчики события
        void CalendarSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var txt = popupCalendar.Tag as TextBox;
            if (txt != null && calendar.SelectedDate.HasValue)
            {
                txt.Text = calendar.SelectedDate.Value.ToString("dd.MM.yyyy");
                txt.Tag = calendar.SelectedDate.Value;
            }

            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }

        void BtnEndDateClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;
            calendar.DisplayDateStart = DateTime.Now.AddDays(1);
            calendar.SelectedDate = NewEndTime;

            popupCalendar.Tag = txtFinishDate;

            popupCalendar.IsOpen = true;
        }

        // схема по умолчанию - 10/15 дней
        void btnDefaulScheme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint value = 0;
            var element = sender as FrameworkElement;
            if (element == null || element.Tag == null || !uint.TryParse(element.Tag.ToString(), out value))
                return;

            NewEndTime = TimeManager.AddWorkDays(NewStartTime, value);
        }
        #endregion

        #region Вспомогательные классы
        [Serializable]
        class SaveState
        {
            #region Свойства
            public string Customer { get; set; }
            public string CustomerName { get; set; }
            public string Product { get; set; }
            public string Id { get; set; }
            public string Options { get; set; }
            public string Comments { get; set; }
            public string PackageType { get; set; }

            public bool IsManagerSetPlanDate { get; set; }

            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            #endregion

            public SaveState(ProjectControl target)
            {
                Customer = target.NewCustomer;
                CustomerName = target.NewCustomerName;
                Product = target.NewProduct;
                Id = target.NewId;
                Options = target.NewOptions;
                Comments = target.NewComments;
                PackageType = target.NewPackageType;

                IsManagerSetPlanDate = target.NewIsManagerSetPlanDate;

                StartTime = target.NewStartTime;
                EndTime = target.NewEndTime;
            }

            public void RestoreState(ProjectControl target)
            {
                target.NewCustomer = Customer;
                target.NewCustomerName = CustomerName;
                target.NewProduct = Product;
                target.NewId = Id;
                target.NewOptions = Options;
                target.NewComments = Comments;
                target.NewPackageType = PackageType;

                target.NewIsManagerSetPlanDate = IsManagerSetPlanDate;

                target.NewStartTime = StartTime;
                target.NewEndTime = EndTime;
            }
        }
        #endregion
    }
}
