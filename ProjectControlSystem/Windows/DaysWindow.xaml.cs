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
    public partial class DaysWindow : Window
    {
        #region Свойства
        public int Count { get; private set; }
        public int MaxCount { get; set; }
        public int MinCount { get; set; }
        #endregion

        public DaysWindow(int count)
        {
            InitializeComponent();

            txtCount.Text = count.ToString();

            MinCount = 1;
            MaxCount = 30;
        }

        #region Обработчики событий
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var c = 0;
            if (!int.TryParse(txtCount.Text, out c))
            {
                MessageBox.Show("Введите правильное количество дней", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (c > MaxCount || c < MinCount)
            {
                MessageBox.Show(string.Format("Число долюно быть в промежутке от {0} до {1}", MinCount, MaxCount), "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            Count = c;
            DialogResult = true;
            Close();
        }
        #endregion
    }
}
