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
    /// Interaction logic for WarehouseFilterControl.xaml
    /// </summary>
    public partial class WarehouseFilterControl : UserControl
    {
        public WarehouseFilterControl()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            // From
            txtWH_R_Actual_From.Text =
            txtWH_R_Plan_From.Text =
            txtWH_E_Actual_From.Text =
            txtWH_E_Plan_From.Text =
            txtWH_G_Actual_From.Text =
            txtWH_G_Plan_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);

            txtWH_R_Actual_From.Tag =
            txtWH_R_Plan_From.Tag =
            txtWH_E_Actual_From.Tag =
            txtWH_E_Plan_From.Tag =
            txtWH_G_Actual_From.Tag =
            txtWH_G_Plan_From.Tag = DateTime.Now.AddDays(-1);


            // To
            txtWH_R_Actual_To.Text =
            txtWH_R_Plan_To.Text =
            txtWH_E_Actual_To.Text =
            txtWH_E_Plan_To.Text =
            txtWH_G_Actual_To.Text =
            txtWH_G_Plan_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);

            txtWH_R_Actual_To.Tag =
            txtWH_R_Plan_To.Tag =
            txtWH_E_Actual_To.Tag =
            txtWH_E_Plan_To.Tag =
            txtWH_G_Actual_To.Tag =
            txtWH_G_Plan_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            #region Гидравлика схемы план
            if (chkWH_G_Time_Plan.IsChecked == true)
            {
                if (radioWH_G_Time_Plan_Not_Set.IsChecked == true && p.Time_WH_G_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtWH_G_Plan_From.Tag, eDate = (DateTime)txtWH_G_Plan_To.Tag;
                if (radioWH_G_Time_Plan_Set.IsChecked == true && (!p.Time_WH_G_Plan.HasValue || p.Time_WH_G_Plan.Value.Date < sDate || p.Time_WH_G_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Гидравлика схемы факт
            if (chkWH_G_Actual_Filter.IsChecked == true)
            {
                if (radioWH_G_Not_Complete.IsChecked == true && p.Time_WH_G_Actual.HasValue)
                    return false;

                if (radioWH_G_NotNeed.IsChecked == true && p.Is_WH_G_NotNeed != true)
                    return false;

                DateTime sDate = (DateTime)txtWH_G_Actual_From.Tag, eDate = (DateTime)txtWH_G_Actual_To.Tag;
                if (radioWH_G_Complete.IsChecked == true && (!p.Time_WH_G_Actual.HasValue || p.Time_WH_G_Actual.Value.Date < sDate || p.Time_WH_G_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            #region Электрика схемы план
            if (chkWH_E_Time_Plan.IsChecked == true)
            {
                if (radioWH_E_Time_Plan_Not_Set.IsChecked == true && p.Time_WH_E_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtWH_E_Plan_From.Tag, eDate = (DateTime)txtWH_E_Plan_To.Tag;
                if (radioWH_E_Time_Plan_Set.IsChecked == true && (!p.Time_WH_E_Plan.HasValue || p.Time_WH_E_Plan.Value.Date < sDate || p.Time_WH_E_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Электрика схемы факт
            if (chkWH_E_Actual_Filter.IsChecked == true)
            {
                if (radioWH_E_NotNeed.IsChecked == true && p.Is_WH_E_NotNeed != true)
                    return false;

                if (radioWH_E_Not_Complete.IsChecked == true && p.Time_WH_E_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtWH_E_Actual_From.Tag, eDate = (DateTime)txtWH_E_Actual_To.Tag;
                if (radioWH_E_Complete.IsChecked == true && (!p.Time_WH_E_Actual.HasValue || p.Time_WH_E_Actual.Value.Date < sDate || p.Time_WH_E_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            #region Рама план
            if (chkWH_R_Time_Plan.IsChecked == true)
            {
                if (radioWH_R_Time_Plan_Not_Set.IsChecked == true && p.Time_WH_R_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtWH_R_Plan_From.Tag, eDate = (DateTime)txtWH_R_Plan_To.Tag;
                if (radioWH_R_Time_Plan_Set.IsChecked == true && (!p.Time_WH_R_Plan.HasValue || p.Time_WH_R_Plan.Value.Date < sDate || p.Time_WH_R_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Рама  факт
            if (chkWH_R_Actual_Filter.IsChecked == true)
            {
                if (radioWH_R_NotNeed.IsChecked == true && p.Is_WH_R_NotNeed != true)
                    return false;

                if (radioWH_R_Not_Complete.IsChecked == true && p.Time_WH_R_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtWH_R_Actual_From.Tag, eDate = (DateTime)txtWH_R_Actual_To.Tag;
                if (radioWH_R_Complete.IsChecked == true && (!p.Time_WH_R_Actual.HasValue || p.Time_WH_R_Actual.Value.Date < sDate || p.Time_WH_R_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            return true;
        }

        internal void ResetFilter()
        {
            // Гидравлика схемы план
            chkWH_G_Time_Plan.IsChecked =
                // Гидравлика схемы факт
            chkWH_G_Actual_Filter.IsChecked =
                // Электрика схемы план
           chkWH_E_Time_Plan.IsChecked =
                // Электрика схемы факт
           chkWH_E_Actual_Filter.IsChecked =
                // Рама план
            chkWH_R_Time_Plan.IsChecked =
                // Рама  факт
           chkWH_R_Actual_Filter.IsChecked = false;
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
