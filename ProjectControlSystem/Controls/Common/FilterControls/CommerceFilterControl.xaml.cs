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
    /// Interaction logic for CommerceFilterControl.xaml
    /// </summary>
    public partial class CommerceFilterControl : UserControl
    {
        public CommerceFilterControl()
        {
            InitializeComponent();

            txtCOM_End_From.Text = txtCOM_Start_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);
            txtCOM_End_From.Tag = txtCOM_Start_From.Tag = DateTime.Now.AddDays(-1);

            txtCOM_End_To.Text = txtCOM_Start_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);
            txtCOM_End_To.Tag = txtCOM_Start_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            // Запуск в производство
            if (chkCOM_Start.IsChecked == true)
            {
                DateTime sDate = (DateTime)txtCOM_Start_From.Tag, eDate = (DateTime)txtCOM_Start_To.Tag;
                if (p.TimeBegin.Date < sDate || p.TimeBegin.Date > eDate)
                    return false;
            }

            // Дата отгрузки (план)
            if (chkCOM_End_Plan.IsChecked == true)
            {
                DateTime sDate = (DateTime)txtCOM_End_From.Tag, eDate = (DateTime)txtCOM_End_To.Tag;
                if (p.TimeEndPlaned.Date < sDate || p.TimeEndPlaned.Date > eDate)
                    return false;
            }

            //тип упаковки
            if (chkCOM_Package_Type.IsChecked == true)
            {
                if (radioCOM_Package_Type_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.COM_Package_Type))
                    return false;
                if (radioCOM_Package_Type_Wood.IsChecked == true && p.COM_Package_Type != "Доски")
                    return false;
            }

            return true;
        }

        internal void ResetFilter()
        {
            // Дата отгрузки (план)
            chkCOM_End_Plan.IsChecked =
                // Запуск в производство
           chkCOM_Start.IsChecked =
                // Тип упаковки
           chkCOM_Package_Type.IsChecked = false;
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

            // Устанавливаем значения
            popupCalendar.PlacementTarget = element;

            calendar.SelectedDate = (DateTime)element.Tag;
            popupCalendar.Tag = element;

            popupCalendar.IsOpen = true;
        }

        #endregion

    }
}
