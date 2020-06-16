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
    /// Interaction logic for ManufactureFilterControl.xaml
    /// </summary>
    public partial class ManufactureFilterControl : UserControl
    {
        public ManufactureFilterControl()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            // From
            //txtTime_OTK_Plan_From.Text =
            txtMF_Complete_From.Text = DateTime.Now.AddDays(-1).ToString(ProjectConfiguration.DateFormat);

            //txtTime_OTK_Plan_From.Tag =
            txtMF_Complete_From.Tag = DateTime.Now.AddDays(-1);


            // To
            //txtTime_OTK_Plan_To.Text =
            txtMF_Complete_To.Text = DateTime.Now.ToString(ProjectConfiguration.DateFormat);

            //txtTime_OTK_Plan_To.Tag =
            txtMF_Complete_To.Tag = DateTime.Now;
        }

        public bool IsFilterPassed(Project p)
        {
            // Пост
            if (!string.IsNullOrEmpty(txtPost.Text) && !p.MF_Post.ToLower().Contains(txtPost.Text))
                return false;

            // Участок
            if (chkMF_Part.IsChecked == true)
            {
                if (radioMF_Part_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_Level))
                    return false;

                if (radioMF_Part_G.IsChecked == true && p.MF_Level != "Гидравлика")
                    return false;

                if (radioMF_Part_E.IsChecked == true && p.MF_Level != "Электрика")
                    return false;
            }

            // Рама состояние
            if (chkMF_R_State.IsChecked == true)
            {
                if (radioMF_R_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_Rama))
                    return false;

                if (radioMF_R_Work.IsChecked == true && p.MF_Rama != Project.inProgress)
                    return false;

                if (radioMF_R_Complete.IsChecked == true && p.MF_Rama != Project.complete)
                    return false;

                if (radioMF_R_NotNeed.IsChecked == true && p.MF_Rama != Project.notNeed)
                    return false;
            }

            // Коллектор состояние
            if (chkMF_Collector.IsChecked == true)
            {
                if (radioMF_Collector_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_Collector))
                    return false;

                if (radioMF_Collector_Work.IsChecked == true && p.MF_Collector != Project.inProgress)
                    return false;

                if (radioMF_Collector_Complete.IsChecked == true && p.MF_Collector != Project.complete)
                    return false;

                if (radioMF_Collector_NotNeed.IsChecked == true && p.MF_Collector != Project.notNeed)
                    return false;
            }

            // Стадия готовности агрегата
            if (chkMF_Complete_Percentage.IsChecked == true)
            {
                if (radioMF_Complete_Percentage_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_Complete_Percentage))
                    return false;

                if (radioMF_Complete_Percentage_0.IsChecked == true && p.MF_Complete_Percentage != Project.inStart)
                    return false;

                if (radioMF_Complete_Percentage_50.IsChecked == true && p.MF_Complete_Percentage != Project.inHalf)
                    return false;

                if (radioMF_Complete_Percentage_100.IsChecked == true && p.MF_Complete_Percentage != Project.inFinish)
                    return false;
            }


            //Расключение агрегата
            if (chkMF_Agregat.IsChecked == true)
            {
                if (radioMF_Agregat_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_Agregat))
                    return false;

                if (radioMF_Agregat_Work.IsChecked == true && p.MF_Agregat != Project.inProgress)
                    return false;

                if (radioMF_Agregat_Complete.IsChecked == true && p.MF_Agregat != Project.complete)
                    return false;

                if (radioMF_Agregat_NotNeed.IsChecked == true && p.MF_Agregat != Project.notNeed)
                    return false;
            }

            //Изготовление ШУ
            if (chkMF_SH_Place.IsChecked == true)
            {
                if (radioMF_SH_Place_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_SH_Place))
                    return false;

                if (radioMF_SH_Place_Agregat.IsChecked == true && p.MF_SH_Place != "Агрегат")
                    return false;

                if (radioMF_SH_Place_SEB.IsChecked == true && p.MF_SH_Place != "СЭБ")
                    return false;

                if (radioMF_SH_Place_RKM.IsChecked == true && p.MF_SH_Place != "РКМ")
                    return false;
            }

            //ШУ
            if (chkMF_SH.IsChecked == true)
            {
                if (radioMF_SH_Not_Set.IsChecked == true && !string.IsNullOrEmpty(p.MF_SH))
                    return false;

                if (radioMF_SH_Dev.IsChecked == true && p.MF_SH != "Согласование")
                    return false;

                if (radioMF_SH_Work.IsChecked == true && p.MF_SH != Project.inProgress)
                    return false;

                if (radioMF_SH_Complete.IsChecked == true && p.MF_SH != Project.complete)
                    return false;

                if (radioMF_SH_NotNeed.IsChecked == true && p.MF_SH != Project.notNeed)
                    return false;
            }

            // Ориентировочная дата готовности
            if (chkMF_End_Plan.IsChecked == true)
            {
                if (radioMF_End_Plan_Not_Set.IsChecked == true && p.MF_Time_Plan.HasValue)
                    return false;

                DateTime sDate = (DateTime)txtMF_Complete_From.Tag, eDate = (DateTime)txtMF_Complete_To.Tag;
                if (radioMF_End_Plan_Complete.IsChecked == true && (!p.MF_Time_Plan.HasValue || p.MF_Time_Plan.Value.Date < sDate || p.MF_Time_Plan.Value.Date > eDate))
                    return false;
            }

            //// Дата передачи на ОТК (план)
            //if (chkTime_OTK_Plan.IsChecked == true)
            //{
            //    DateTime sDate = (DateTime)txtTime_OTK_Plan_From.Tag, eDate = (DateTime)txtTime_OTK_Plan_To.Tag;
            //    if (p.Time_OTK_Plan.Date < sDate || p.Time_OTK_Plan.Date > eDate)
            //        return false;
            //}

            return true;
        }

        internal void ResetFilter()
        {
            // Пост
            txtPost.Text = "";

            // Участок
            chkMF_Part.IsChecked =
                // Рама состояние
            chkMF_R_State.IsChecked =
                // Коллектор состояние
            chkMF_Collector.IsChecked =
                //Расключение агрегата
            chkMF_Agregat.IsChecked =
                //Изготовление ШУ
            chkMF_SH_Place.IsChecked =
                //ШУ
            chkMF_SH.IsChecked =
                // Ориентировочная дата готовности
           chkMF_End_Plan.IsChecked = false;
            //     // Дата постановки на тест (план)
            //chkTime_OTK_Plan.IsChecked = false;
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

            // уст. значения
            popupCalendar.PlacementTarget = element;

            calendar.SelectedDate = (DateTime)element.Tag;
            popupCalendar.Tag = element;

            popupCalendar.IsOpen = true;
        }

        #endregion
    }
}
