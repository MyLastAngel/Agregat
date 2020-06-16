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
    /// Interaction logic for NewRequestWindow.xaml
    /// </summary>
    public partial class NewRequestWindow : Window
    {
        #region Поля
        uint? debtCount = null;
        #endregion

        #region Свойства
        public string NewDetails
        {
            get { return cmbDetail != null ? cmbDetail.Text : ""; }
            set
            {
                if (cmbDetail != null)
                    cmbDetail.Text = value;
            }
        }
        public string NewArticle
        {
            get { return txtArticle != null ? txtArticle.Text : ""; }
            set
            {
                if (txtArticle != null)
                    txtArticle.Text = value;
            }
        }
        public uint NewCount
        {
            get
            {
                uint v = 0;
                if (txtCount == null || string.IsNullOrEmpty(txtCount.Text) || !uint.TryParse(txtCount.Text, out v) || v <= 0)
                    return 0;
                else
                    return v;
            }
            set
            {
                if (txtCount != null)
                    txtCount.Text = value.ToString();
            }
        }

        public uint? DebtCount { set { debtCount = value; } }
        #endregion

        public NewRequestWindow(bool isNew = true)
        {
            InitializeComponent();

            txtArticle.IsEnabled = cmbDetail.IsEnabled = isNew;
            if (!isNew) // если не новый то список очистить нельзя т.к. нельзя его посмотреть
                btnClearDetaisList.Visibility = System.Windows.Visibility.Collapsed;

            cmbDetail.ItemsSource = DetailsManager.Details;
        }

        bool CheckValid()
        {
            var value = cmbDetail.Text;
            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Введите название", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            value = txtCount.Text;
            uint v = 0;
            if (string.IsNullOrEmpty(value) || !uint.TryParse(value, out v) || v <= 0)
            {
                MessageBox.Show("Введите количество товара", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (debtCount.HasValue && v > debtCount.Value)
            {
                MessageBox.Show("Количество товара не может быть больше " + debtCount.Value, "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            return true;
        }

        #region Обработчики событий
        // очистка списка деталей
        void btnClearDetaisList_Click(object sender, RoutedEventArgs e)
        {
            var rez = MessageBox.Show(this, "Вы уверены что хотите очисть список деталей с локального компьютера?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rez != MessageBoxResult.Yes)
                return;

            cmbDetail.ItemsSource = null;
            DetailsManager.Details.Clear();
            DetailsManager.Save();
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValid())
                return;

            DetailsManager.Add(NewDetails);

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
