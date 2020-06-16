using ArgDb;
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
using System.Windows.Shapes;

namespace ProjectControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for EditProjectWindow.xaml
    /// </summary>
    public partial class EditProjectWindow : Window
    {
        #region Поля
        Project pEdit = null;
        #endregion

        public EditProjectWindow(Project p)
        {
            InitializeComponent();

            pEdit = p.Clone();

            Title = string.Format("Изменение проекта: {0}", p.ID);

            DataContext = pEdit;
        }

        #region Обработчики событий

        #region ИТО
        // ИТО Гидравлика
        void btnTime_ITO_G_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_ITO_G_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_ITO_G_Actual.Value;

            calendar.Tag = ProjectPropertyType.ITO_Hydraulics_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_ITO_G_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_G_NotNeed = false;
            pEdit.Time_ITO_G_Actual = null;
        }
        void mnuITO_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_G_NotNeed = true;
        }

        // ИТО Электрика
        void btnTime_ITO_E_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_ITO_G_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_ITO_E_Actual.Value;

            calendar.Tag = ProjectPropertyType.ITO_Electrician_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_ITO_E_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_E_NotNeed = false;
            pEdit.Time_ITO_E_Actual = null;
        }
        void mnuITO_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_E_NotNeed = true;
        }

        // ИТО Рама
        void btnTime_ITO_R_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_ITO_R_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_ITO_R_Actual.Value;

            calendar.Tag = ProjectPropertyType.ITO_Rama_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_ITO_R_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_R_NotNeed = false;
            pEdit.Time_ITO_R_Actual = null;
        }
        void mnuITO_R_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_ITO_R_NotNeed = true;
        }
        #endregion
        #region Склад
        // Гидравлика
        void btnTime_WH_G_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_WH_G_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_WH_G_Actual.Value;

            calendar.Tag = ProjectPropertyType.WH_Hydraulics_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_WH_G_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_G_NotNeed = false;
            pEdit.Time_WH_G_Actual = null;
        }
        void mnuWH_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_G_NotNeed = true;
        }

        // Электрика
        void btnTime_WH_E_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_WH_E_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_WH_E_Actual.Value;

            calendar.Tag = ProjectPropertyType.WH_Electrician_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_WH_E_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_E_NotNeed = false;
            pEdit.Time_WH_E_Actual = null;
        }
        void mnuWH_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_E_NotNeed = true;
        }

        // Рама
        void btnTime_WH_R_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_WH_R_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_WH_R_Actual.Value;

            calendar.Tag = ProjectPropertyType.WH_Rama_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_WH_R_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Time_WH_R_Actual = null;
        }

        // Проект завершен
        void btnTimeEndActual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.TimeEndActual.HasValue)
                calendar.SelectedDate = pEdit.TimeEndActual.Value;

            calendar.Tag = ProjectPropertyType.CompleteProject;
            popupCalendar.IsOpen = true;
        }
        void btnTimeEndActual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_R_NotNeed = false;
            pEdit.TimeEndActual = null;
        }
        void mnuWH_R_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_WH_R_NotNeed = true;
        }
        #endregion
        #region Производство
        //  Производство ориент. время готовности
        void btnMF_Time_Plan_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.MF_Time_Plan.HasValue)
                calendar.SelectedDate = pEdit.MF_Time_Plan.Value;

            calendar.Tag = ProjectPropertyType.MF_Time_Planed;
            popupCalendar.IsOpen = true;
        }
        void btnMF_Time_Plan_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.MF_Time_Plan = null;
        }
        #endregion
        #region ОТК
        //Дата передачи на ОТК (факт)
        void btnMF_Time_Test_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.MF_Time_Test_Actual.HasValue)
                calendar.SelectedDate = pEdit.MF_Time_Test_Actual.Value;

            calendar.Tag = ProjectPropertyType.MF_Time_Test_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnMF_Time_Test_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.MF_Time_Test_Actual = null;
        }

        //Дата испытаний по гидравлике
        void btnTime_OTK_G_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_OTK_G_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_OTK_G_Actual.Value;

            calendar.Tag = ProjectPropertyType.OTK_Hydraulics_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_OTK_G_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_OTK_G_NotNeed = false;
            pEdit.Time_OTK_G_Actual = null;
        }
        void mnuOTK_G_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_OTK_G_NotNeed = true;
        }

        //Дата испытаний по шкафа
        void btnTime_OTK_E_Actual_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            popupCalendar.PlacementTarget = btn;

            if (pEdit.Time_OTK_E_Actual.HasValue)
                calendar.SelectedDate = pEdit.Time_OTK_E_Actual.Value;

            calendar.Tag = ProjectPropertyType.OTK_Electrician_Time_Actual;
            popupCalendar.IsOpen = true;
        }
        void btnTime_OTK_E_Actual_Delete_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_OTK_E_NotNeed = false;
            pEdit.Time_OTK_E_Actual = null;
        }
        void mnuOTK_E_NotNeed_Click(object sender, RoutedEventArgs e)
        {
            pEdit.Is_OTK_E_NotNeed = true;
        }
        #endregion
        #region Комментарии
        // Удаляем все сообещния
        void btnMessagesClean_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Уверены что хотите удалить все сообщения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // очищаем
            if (!ServiceManager.ClearCommentsFromProject(pEdit.ProjectID))
            {
                MessageBox.Show("Ошибка удаления всeх коментарий для проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            pEdit.Messages.Clear();
        }
        // Удаляем выбранное сообщение
        void btnRemoveMessage_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            if (btn == null || !(btn.Tag is AgrProjectComment))
                return;

            var comment = (AgrProjectComment)btn.Tag;

            var result = MessageBox.Show(string.Format("Уверены что хотите удалить сообщение: '{0}'?", comment.Message), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            // удаляем
            if (!ServiceManager.RemoveCommentFromProject(pEdit.ProjectID, comment.Date, comment.Message, comment.User))
            {
                MessageBox.Show("Ошибка удаления коментария для проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            pEdit.Messages.Remove(comment);
        }
        #endregion

        // Изменилось значение времени
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(calendar.Tag is ProjectPropertyType) || !calendar.SelectedDate.HasValue || !popupCalendar.IsOpen)
            {
                popupCalendar.IsOpen = false;
                return;
            }

            var type = (ProjectPropertyType)calendar.Tag;
            switch (type)
            {
                #region ITO_Hydraulics_Time_Actual
                case ProjectPropertyType.ITO_Hydraulics_Time_Actual:
                    {
                        pEdit.Time_ITO_G_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region  ITO_Electrician_Time_Actual
                case ProjectPropertyType.ITO_Electrician_Time_Actual:
                    {
                        pEdit.Time_ITO_E_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region  ITO_Rama_Time_Actual
                case ProjectPropertyType.ITO_Rama_Time_Actual:
                    {
                        pEdit.Time_ITO_R_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion

                #region  WH_Hydraulics_Time_Actual
                case ProjectPropertyType.WH_Hydraulics_Time_Actual:
                    {
                        pEdit.Time_WH_G_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region  WH_Electrician_Time_Actual
                case ProjectPropertyType.WH_Electrician_Time_Actual:
                    {
                        pEdit.Time_WH_E_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region  WH_Rama_Time_Actual
                case ProjectPropertyType.WH_Rama_Time_Actual:
                    {
                        pEdit.Time_WH_R_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region  CompleteProject
                case ProjectPropertyType.CompleteProject:
                    {
                        pEdit.TimeEndActual = calendar.SelectedDate;
                        break;
                    }
                #endregion

                #region MF_Time_Planed
                case ProjectPropertyType.MF_Time_Planed:
                    {
                        pEdit.MF_Time_Plan = calendar.SelectedDate;
                        break;
                    }
                #endregion

                #region MF_Time_Test_Actual
                case ProjectPropertyType.MF_Time_Test_Actual:
                    {
                        pEdit.MF_Time_Test_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region OTK_Hydraulics_Time_Actual
                case ProjectPropertyType.OTK_Hydraulics_Time_Actual:
                    {
                        pEdit.Time_OTK_G_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
                #region OTK_Electrician_Time_Actual
                case ProjectPropertyType.OTK_Electrician_Time_Actual:
                    {
                        pEdit.Time_OTK_E_Actual = calendar.SelectedDate;
                        break;
                    }
                #endregion
            }

            // закрываем выбранную дату
            calendar.SelectedDate = null;
            popupCalendar.IsOpen = false;
        }

        // Сохраняем
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ServiceManager.ChangeProject(pEdit.GetAgrProject()))
            {
                MessageBox.Show("Ошибка изменения проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            DialogResult = true;
            Close();
        }

        // Отмена
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion
    }
}
