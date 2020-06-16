using ArgDb;
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

namespace ProjectControlSystem.Windows.MFPlanner
{
    public partial class MFPlannerWorkerEditWindow : Window
    {
        #region Свойства
        public MFWorker Result { get; private set; }
        #endregion

        public MFPlannerWorkerEditWindow(MFWorker w = null)
        {
            InitializeComponent();

            if (w != null)
            {
                txtId.Text = w.Id.ToString();
                txtPost.Text = w.Post.ToString();
                txtName.Text = w.Name;
                txtSecondName.Text = w.SecondName;
                if (w.EndWorkTime.HasValue)
                {
                    chkIsFired.IsChecked = true;
                    txtFireDate.Text = w.EndWorkTime.Value.ToString("dd/MM/yyyy");
                }
            }
            else
            {
                Result = new MFWorker();
                Title = "Новый работник";
            }
        }

        #region Обработчики событий
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var post = 0;
            if (string.IsNullOrEmpty(txtPost.Text) || !int.TryParse(txtPost.Text, out post) || string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Пост и Имя - обязательные поля", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                DialogResult = null;
                return;
            }

            var id = 0;
            if (!string.IsNullOrEmpty(txtId.Text))
                int.TryParse(txtId.Text, out id);

            Result = new MFWorker
            {
                Id = id,
                Post = post,
                Name = txtName.Text,
                SecondName = txtSecondName.Text,
            };

            // Если уволили
            if (chkIsFired.IsChecked == true)
                Result.EndWorkTime = DateTime.Now.Date;

            DialogResult = true;
            Close();
        }
        #endregion
    }
}
