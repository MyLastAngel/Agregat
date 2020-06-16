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

namespace ProjectControlSystem.MFPlannerWindows
{
    public partial class MFPlannerCommentWindow : Window
    {
        #region Свойства
        public string Comment
        {
            get { return txtComment.Text; }
            set { txtComment.Text = value; }
        }
        #endregion

        public MFPlannerCommentWindow()
        {
            InitializeComponent();
        }

        #region Обработчики событий
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Comment))
            {
                var res = MessageBox.Show("Сохранить пустое описание?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Stop);
                if (res != MessageBoxResult.Yes)
                    return;
            }

            DialogResult = true;
            Close();
        }
        #endregion
    }
}
