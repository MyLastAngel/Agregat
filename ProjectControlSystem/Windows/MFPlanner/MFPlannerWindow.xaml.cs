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
    public partial class MFPlannerWindow : Window
    {
        #region Поля
        #endregion

        #region Свойства

        #endregion


        public MFPlannerWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void btnRedraw_Click(object sender, RoutedEventArgs e)
        {
            cPlan.InvalidateVisual();
        }
    }
}
