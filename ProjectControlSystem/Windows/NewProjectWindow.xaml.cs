using ProjectControlSystem.Managers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        #region Свойства
        public string NewCustomer { get { return cProject.NewCustomer; } }
        public string NewCustomerName { get { return cProject.NewCustomerName; } }
        public string NewProduct { get { return cProject.NewProduct; } }
        public string NewId { get { return cProject.NewId; } }
        public string NewOptions { get { return cProject.NewOptions; } }
        public string NewComments { get { return cProject.NewComments; } }
        public string NewPackageType { get { return cProject.NewPackageType; } }

        public bool NewIsManagerSetPlanDate { get { return cProject.NewIsManagerSetPlanDate; } }

        public DateTime NewStartTime { get { return cProject.NewStartTime; } }
        public DateTime NewEndTime { get { return cProject.NewEndTime; } }
        #endregion

        public NewProjectWindow()
        {
            InitializeComponent();
        }

        #region Обработчики события
        // вставить из буфера
        void btnSet_Click(object sender, RoutedEventArgs e)
        {
            cProject.SetState();
        }
        // копировать в буфер
        void btnGet_Click(object sender, RoutedEventArgs e)
        {
            cProject.GetState();
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!cProject.CheckValid())
            {
                DialogResult = null;
                return;
            }

            DialogResult = true;
            Close();
        }
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}

