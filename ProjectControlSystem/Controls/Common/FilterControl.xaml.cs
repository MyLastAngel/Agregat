using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ProjectControlSystem.Controls
{
    public partial class FilterControl : UserControl, IFilterChecker, INotifyPropertyChanged
    {
        #region Поля
        bool isFilterSet = false;
        #endregion

        #region Свойства
        public bool IsFilterSet
        {
            get { return isFilterSet; }
            set
            {
                if (isFilterSet == value)
                    return;

                isFilterSet = value;
                DoPropertyChanged("IsFilterSet");
            }
        }
        #endregion

        public FilterControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void UpdateState(ProjectViewMode mode)
        {
            //pCommerce.Visibility = ((mode & ProjectViewMode.Commercial) != 0) ? Visibility.Visible : Visibility.Collapsed;
            //pITO.Visibility = ((mode & ProjectViewMode.ITO) != 0) ? Visibility.Visible : Visibility.Collapsed;
            //pWH.Visibility = ((mode & ProjectViewMode.Warehouse) != 0) ? Visibility.Visible : Visibility.Collapsed;
            //pMF.Visibility = ((mode & ProjectViewMode.Manufacture) != 0) ? Visibility.Visible : Visibility.Collapsed;
            //pOTK.Visibility = ((mode & ProjectViewMode.OTK) != 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Проверка прохождения фильтра</summary>
        public bool IsFilterPassed(Project p)
        {
            if (!IsFilterSet)
                return true;

            if (!filterMain.IsFilterPassed(p))
                return false;
            if (!filterComerce.IsFilterPassed(p))
                return false;
            if (!filterITO.IsFilterPassed(p))
                return false;
            if (!filterWarehouse.IsFilterPassed(p))
                return false;
            if (!filterManufacture.IsFilterPassed(p))
                return false;
            if (!filterOTK.IsFilterPassed(p))
                return false;

            return true;
        }

        #region Обработчики событий
        void btnSetFilter_Click(object sender, RoutedEventArgs e)
        {
            IsFilterSet = true;

            DoFilterConfirmed();
        }

        void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            IsFilterSet = false;

            filterMain.ResetFilter();
            filterComerce.ResetFilter();
            filterITO.ResetFilter();
            filterWarehouse.ResetFilter();
            filterManufacture.ResetFilter();
            filterOTK.ResetFilter();

            DoFilterCleared();
        }
        #endregion

        #region События.

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region FilterConfirmed
        public event EventHandler FilterConfirmed;
        void DoFilterConfirmed()
        {
            if (FilterConfirmed != null)
                FilterConfirmed(this, null);
        }
        #endregion

        #region FilterCleared
        public event EventHandler FilterCleared;
        void DoFilterCleared()
        {
            if (FilterCleared != null)
                FilterCleared(this, null);
        }
        #endregion

        #endregion
    }
}
