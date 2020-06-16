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
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            txtAddress.Text = ProjectConfiguration.Address;
            txtReadDays.Text = ProjectConfiguration.LoadDays.ToString();
            txtUpdateTimeSec.Text = ProjectConfiguration.UpdateTimeSec.ToString();

            cmbIsShowMessageMenu.IsChecked = ProjectConfiguration.IsShowMessageMenu;
            chkIsHideAlertWin.IsChecked = ProjectConfiguration.IsHideAlertWin;

            txtAlertShowTimeMin.Text = ProjectConfiguration.AlertShowTimeMin.ToString();

            cmbIsEnableColorsOnMainImportToExcel.IsChecked = ProjectConfiguration.IsEnableColorsOnMainImportToExcel;
        }
        bool Check()
        {
            int value = 0;
            if (!int.TryParse(txtReadDays.Text, out value))
            {
                MessageBox.Show("Введите правильное количество дней", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                txtReadDays.Focus();
                return false;
            }

            if (!int.TryParse(txtUpdateTimeSec.Text, out value) || value <= 0)
            {
                MessageBox.Show("Введите правильное время обновления с сервера ", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                txtUpdateTimeSec.Focus();
                return false;
            }

            if (!int.TryParse(txtAlertShowTimeMin.Text, out value) || value <= 0)
            {
                MessageBox.Show("Введите правильное время скрытия окна оповещения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                txtAlertShowTimeMin.Focus();
                return false;
            }

            return true;
        }

        #region Обработчики событий
        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Check())
                return;

            int value = 0;
            // сколько дней перечитывать
            if (int.TryParse(txtReadDays.Text, out value))
                ProjectConfiguration.LoadDays = value;
            // как часто перечитывать
            if (int.TryParse(txtUpdateTimeSec.Text, out value))
                ProjectConfiguration.UpdateTimeSec = value;
            //через сколько мин закрывать окно оповещения
            if (int.TryParse(txtAlertShowTimeMin.Text, out value))
                ProjectConfiguration.AlertShowTimeMin = value;

            // адпрес сервера
            if (ProjectConfiguration.Address != txtAddress.Text)
            {
                ProjectConfiguration.Address = txtAddress.Text;

                ServiceManager.CloseClient();
                ProjectManager.ReloadProjectsAsync();
            }

            // панель оповещений
            ProjectConfiguration.IsShowMessageMenu = cmbIsShowMessageMenu.IsChecked == true;
            ProjectConfiguration.IsHideAlertWin = chkIsHideAlertWin.IsChecked == true;
            ProjectConfiguration.IsEnableColorsOnMainImportToExcel = cmbIsEnableColorsOnMainImportToExcel.IsChecked == true;

            ProjectConfiguration.Save();
            Close();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
