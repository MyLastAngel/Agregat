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
    /// Interaction logic for OTKFilterControl.xaml
    /// </summary>
    public partial class OTKFilterControl : UserControl
    {
        public OTKFilterControl()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            // From
            txtOTK_E_From.Text =
            txtOTK_G_From.Text =
            txtMF_Time_Test_Actual_From.Text =
            txtOTK_Plan_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);

            txtOTK_E_From.Tag =
            txtOTK_G_From.Tag =
            txtMF_Time_Test_Actual_From.Tag =
            txtOTK_Plan_From.Tag = DateTime.Now.AddDays(-1);

            // To
            txtOTK_E_To.Text =
            txtOTK_G_To.Text =
            txtMF_Time_Test_Actual_To.Text =
            txtOTK_Plan_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);

            txtOTK_E_To.Tag =
            txtOTK_G_To.Tag =
            txtMF_Time_Test_Actual_To.Tag =
            txtOTK_Plan_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            #region Дата передачи на ОТК (план)
            if (chkOTK_Plan.IsChecked == true)
            {
                if (radioOTK_Time_Plan_Not_Set.IsChecked == true && p.Time_OTK_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtOTK_Plan_From.Tag, eDate = (DateTime)txtOTK_Plan_To.Tag;
                if (radioOTK_Time_Plan_Set.IsChecked == true && (!p.Time_OTK_Plan.HasValue || p.Time_OTK_Plan.Value.Date < sDate || p.Time_OTK_Plan.Value.Date > eDate))
                    return false;
            }
            #endregion

            // Дата постановки на тест (план)
            if (chkMF_Time_Test_Actual.IsChecked == true)
            {
                if (radioMF_Time_Test_Actual_Not_Set.IsChecked == true && p.MF_Time_Test_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtMF_Time_Test_Actual_From.Tag, eDate = (DateTime)txtMF_Time_Test_Actual_To.Tag;
                if (radioMF_Time_Test_Actual_Set.IsChecked == true && (!p.MF_Time_Test_Actual.HasValue || p.MF_Time_Test_Actual.Value.Date < sDate || p.MF_Time_Test_Actual.Value.Date > eDate))
                    return false;
            }

            // Дата испытаний по гидравлике
            if (chkOTK_G.IsChecked == true)
            {
                if (radioOTK_G_NotNeed.IsChecked == true && p.Is_OTK_G_NotNeed != true)
                    return false;

                if (radioOTK_G_Not_Set.IsChecked == true && p.Time_OTK_G_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtOTK_G_From.Tag, eDate = (DateTime)txtOTK_G_To.Tag;
                if (radioOTK_G_Set.IsChecked == true && (!p.Time_OTK_G_Actual.HasValue || p.Time_OTK_G_Actual.Value.Date < sDate || p.Time_OTK_G_Actual.Value.Date > eDate))
                    return false;
            }

            // Дата испытаний по электрике
            if (chkOTK_E.IsChecked == true)
            {
                if (radioOTK_E_NotNeed.IsChecked == true && p.Is_OTK_E_NotNeed != true)
                    return false;

                if (radioOTK_E_Not_Set.IsChecked == true && p.Time_OTK_E_Actual.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtOTK_E_From.Tag, eDate = (DateTime)txtOTK_E_To.Tag;
                if (radioOTK_E_Set.IsChecked == true && (!p.Time_OTK_E_Actual.HasValue || p.Time_OTK_E_Actual.Value.Date < sDate || p.Time_OTK_E_Actual.Value.Date > eDate))
                    return false;
            }

            return true;
        }

        internal void ResetFilter()
        {
            // Дата передачи на ОТК (план)
            chkOTK_Plan.IsChecked =
                // Дата постановки на тест (план)
           chkMF_Time_Test_Actual.IsChecked =
                // Дата испытаний по гидравлике
           chkOTK_G.IsChecked =
                // Дата испытаний по электрике
            chkOTK_E.IsChecked = false;
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
