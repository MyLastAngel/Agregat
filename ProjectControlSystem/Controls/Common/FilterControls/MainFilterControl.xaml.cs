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

namespace ProjectControlSystem.FilterControls
{
    /// <summary>
    /// Interaction logic for MainFilterControl.xaml
    /// </summary>
    public partial class MainFilterControl : UserControl
    {
        public MainFilterControl()
        {
            InitializeComponent();

            txtEnd_Time_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);
            txtEnd_Time_From.Tag = DateTime.Now.AddDays(-1);

            txtEnd_Time_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);
            txtEnd_Time_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            // id
            if (!string.IsNullOrEmpty(txtID.Text))
            {
                // несколько условий
                if (txtID.Text.Contains(";"))
                {
                    var isOk = false;
                    var filterList = txtID.Text.Split(';');
                    foreach (var f in filterList)
                    {
                        if (p.ID.ToLower().Contains(f.Trim().ToLower()))
                        {
                            isOk = true;
                            break;
                        }
                    }

                    if (!isOk)
                        return false;
                }
                else // конкретное
                {
                    if (!p.ID.ToLower().Contains(txtID.Text.ToLower()))
                        return false;
                }
            }

            // Customer
            if (!string.IsNullOrEmpty(txtCustomer.Text) && !p.Customer.ToLower().Contains(txtCustomer.Text.ToLower()))
                return false;

            // Product
            if (!string.IsNullOrEmpty(txtProduct.Text) && !p.Product.ToLower().Contains(txtProduct.Text.ToLower()))
                return false;

            // Options
            if (!string.IsNullOrEmpty(txtOptions.Text) && !p.Options.ToLower().Contains(txtOptions.Text.ToLower()))
                return false;

            // project state
            if (chkProject_State_Filter.IsChecked == true)
            {
                // нужен завершенный проект а у нас в процессе
                if (radioProject_State_Complete.IsChecked == true && !p.TimeEndActual.HasValue)
                    return false;

                // нужен проект в процессе а у нас завершенный
                if (radioProject_State_Not_Complete.IsChecked == true && p.TimeEndActual.HasValue)
                    return false;
            }

            // Цвет состояния
            if (chkProject_State_Color_Filter.IsChecked == true)
            {
                var isContains = false;

                if (chkProject_State_Color_Ok.IsChecked == true && p.State == ProjectState.Ok)
                    isContains = true;

                if (chkProject_State_Color_Warning.IsChecked == true && p.State == ProjectState.Warning)
                    isContains = true;

                if (chkProject_State_Color_Error.IsChecked == true && p.State == ProjectState.Error)
                    isContains = true;

                if (!isContains)
                    return false;
            }

            // дата отгрузки факт
            if (chkCOM_End_Time.IsChecked == true)
            {
                if (radioCOM_End_Time_Not_Set.IsChecked == true && p.TimeEndActual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtEnd_Time_From.Tag, eDate = (DateTime)txtEnd_Time_To.Tag;
                if (radioCOM_End_Time_Set.IsChecked == true && (!p.TimeEndActual.HasValue || p.TimeEndActual.Value.Date < sDate || p.TimeEndActual.Value.Date > eDate))
                    return false;
            }

            // долги электрика
            if (chkDebt_E_State_Filter.IsChecked == true)
            {
                // если нужны с долгами а нет долгов
                if (radioDebt_E_Exist.IsChecked == true && (p.IsDebt_E_None == true || !p.IsDebt_E_None.HasValue))
                    return false;

                // если нужны без долгов а они есть
                if (radioDebt_E_Not_Exist.IsChecked == true && p.IsDebt_E_None == false)
                    return false;
            }

            // долги гидравлика
            if (chkDebt_G_State_Filter.IsChecked == true)
            {
                // если нужны с долгами а нет долгов
                if (radioDebt_G_Exist.IsChecked == true && (p.IsDebt_G_None == true || !p.IsDebt_G_None.HasValue))
                    return false;

                // если нужны без долгов а они есть
                if (radioDebt_G_Not_Exist.IsChecked == true && p.IsDebt_G_None == false)
                    return false;
            }

            return true;
        }

        public void ResetFilter()
        {
            // id/Customer/Product/Options
            txtOptions.Text =
            txtProduct.Text =
            txtCustomer.Text =
            txtID.Text = "";

            // долги гидравлика
            chkDebt_G_State_Filter.IsChecked =
                // долги электрика
            chkDebt_E_State_Filter.IsChecked =
                // дата отгрузки факт
            chkCOM_End_Time.IsChecked =
                // project state
            chkProject_State_Filter.IsChecked = false;
        }

        #region Обработчики событий

        // Устанавливаем время
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!popupCalendar.IsOpen)
                return;

            var txt = popupCalendar.Tag as TextBlock;
            if (txt != null && calendar.SelectedDate.HasValue)
            {
                txt.Text = calendar.SelectedDate.Value.ToString(ProjectConfiguration.DateFormat);
                txt.Tag = calendar.SelectedDate.Value;
            }

            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }

        // Открываем выбор времни
        void txtTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null || !(element.Tag is DateTime))
                return;

            popupCalendar.Tag = null;

            // устанавливаем значения
            popupCalendar.PlacementTarget = element;

            calendar.SelectedDate = (DateTime)element.Tag;
            popupCalendar.Tag = element;

            popupCalendar.IsOpen = true;
        }

        #endregion
    }
}
