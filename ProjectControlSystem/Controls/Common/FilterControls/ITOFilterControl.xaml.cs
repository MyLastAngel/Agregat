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
    /// Interaction logic for ITOFilterControl.xaml
    /// </summary>
    public partial class ITOFilterControl : UserControl
    {
        public ITOFilterControl()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            // From
            txtITO_R_Actual_From.Text =
            txtITO_R_Plan_From.Text =
            txtITO_E_Actual_From.Text =
            txtITO_E_Plan_From.Text =
            txtITO_G_Actual_From.Text =
            txtITO_G_Time_Plan_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);

            txtITO_R_Actual_From.Tag =
            txtITO_R_Plan_From.Tag =
            txtITO_E_Actual_From.Tag =
            txtITO_E_Plan_From.Tag =
            txtITO_G_Actual_From.Tag =
            txtITO_G_Time_Plan_From.Tag = DateTime.Now.AddDays(-1);


            // To
            txtITO_R_Actual_To.Text =
            txtITO_R_Plan_To.Text =
            txtITO_E_Actual_To.Text =
            txtITO_E_Plan_To.Text =
            txtITO_G_Actual_To.Text =
            txtITO_G_Time_Plan_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);

            txtITO_R_Actual_To.Tag =
            txtITO_R_Plan_To.Tag =
            txtITO_E_Actual_To.Tag =
            txtITO_E_Plan_To.Tag =
            txtITO_G_Actual_To.Tag =
            txtITO_G_Time_Plan_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            #region Гидравлика схемы план
            if (chkITO_G_Time_Plan.IsChecked == true)
            {
                if (radioITO_G_Time_Plan_Not_Set.IsChecked == true && p.Time_ITO_G_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_G_Time_Plan_From.Tag, eDate = (DateTime)txtITO_G_Time_Plan_To.Tag;
                if (radioITO_G_Time_Plan_Set.IsChecked == true && (!p.Time_ITO_G_Plan.HasValue || p.Time_ITO_G_Plan.Value.Date < sDate || p.Time_ITO_G_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Гидравлика схемы факт
            if (chkITO_G_Actual_Filter.IsChecked == true)
            {
                if (radioITO_G_NotNeed.IsChecked == true && p.Is_ITO_G_NotNeed != true)
                    return false;

                if (radioITO_G_Not_Complete.IsChecked == true && p.Time_ITO_G_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_G_Actual_From.Tag, eDate = (DateTime)txtITO_G_Actual_To.Tag;
                if (radioITO_G_Complete.IsChecked == true && (!p.Time_ITO_G_Actual.HasValue || p.Time_ITO_G_Actual.Value.Date < sDate || p.Time_ITO_G_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            #region Электрика схемы план
            if (chkITO_E_Time_Plan.IsChecked == true)
            {
                if (radioITO_E_Time_Plan_Not_Set.IsChecked == true && p.Time_ITO_E_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_E_Plan_From.Tag, eDate = (DateTime)txtITO_E_Plan_To.Tag;
                if (radioITO_E_Time_Plan_Set.IsChecked == true && (!p.Time_ITO_E_Plan.HasValue || p.Time_ITO_E_Plan.Value.Date < sDate || p.Time_ITO_E_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Электрика схемы факт
            if (chkITO_E_Actual_Filter.IsChecked == true)
            {
                if (radioITO_E_NotNeed.IsChecked == true && p.Is_ITO_E_NotNeed != true)
                    return false;

                if (radioITO_E_Not_Complete.IsChecked == true && p.Time_ITO_E_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_E_Actual_From.Tag, eDate = (DateTime)txtITO_E_Actual_To.Tag;
                if (radioITO_E_Complete.IsChecked == true && (!p.Time_ITO_E_Actual.HasValue || p.Time_ITO_E_Actual.Value.Date < sDate || p.Time_ITO_E_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            #region Рама план
            if (chkITO_R_Time_Plan.IsChecked == true)
            {
                if (radioITO_R_Time_Plan_Not_Set.IsChecked == true && p.Time_ITO_R_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_R_Plan_From.Tag, eDate = (DateTime)txtITO_R_Plan_To.Tag;
                if (radioITO_R_Time_Plan_Set.IsChecked == true && (!p.Time_ITO_R_Plan.HasValue || p.Time_ITO_R_Plan.Value.Date < sDate || p.Time_ITO_R_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion
            #region Рама  факт
            if (chkITO_R_Actual_Filter.IsChecked == true)
            {
                if (radioITO_R_NotNeed.IsChecked == true && p.Is_ITO_R_NotNeed != true)
                    return false;

                if (radioITO_R_Not_Complete.IsChecked == true && p.Time_ITO_R_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtITO_R_Actual_From.Tag, eDate = (DateTime)txtITO_R_Actual_To.Tag;
                if (radioITO_R_Complete.IsChecked == true && (!p.Time_ITO_R_Actual.HasValue || p.Time_ITO_R_Actual.Value.Date < sDate || p.Time_ITO_R_Actual.Value.Date > eDate))
                    return false;
            }
            #endregion

            // тип рамы
            if (chkITO_R_Type.IsChecked == true)
            {
                if (radioITO_R_None.IsChecked == true && !string.IsNullOrEmpty(p.MF_Rama))
                    return false;

                if (radioITO_R_Combined.IsChecked == true && p.MF_Rama != "Сборная")
                    return false;

                if (radioITO_R_Welded.IsChecked == true && p.MF_Rama != "Сварная")
                    return false;

                if (radioITO_R_NetNeed.IsChecked == true && p.MF_Rama != Project.notNeed)
                    return false;
            }

            return true;
        }

        internal void ResetFilter()
        {
            // Гидравлика схемы план
            chkITO_G_Time_Plan.IsChecked =
                // Гидравлика схемы факт
            chkITO_G_Actual_Filter.IsChecked =
                // Электрика схемы план
           chkITO_E_Time_Plan.IsChecked =
                // Электрика схемы факт
            chkITO_E_Actual_Filter.IsChecked =
                // Рама план
            chkITO_R_Time_Plan.IsChecked =
                // Рама  факт
            chkITO_R_Actual_Filter.IsChecked =
                // тип рамы
            chkITO_R_Type.IsChecked = false;
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
