using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class AboutWindow : Window
    {
        public AboutWindow(List<LibInfo> libInfo)
        {
            InitializeComponent();

            viewLibraryList.ItemsSource = null;
            viewLibraryList.ItemsSource = libInfo;
        }

        #region Обработчики событий
        void ViewInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = viewLibraryList.SelectedItem as LibInfo;
            if (s == null)
                return;

            txtDescription.Text = s.Description;
        }
        // открываем сайт
        void logo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("http://www.agregatref.ru/");
            }
            catch { }
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Вспомогательные классы
        public class LibInfo
        {
            #region Свойства.
            public string Name { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }
            public string Date { get; private set; }
            #endregion

            public LibInfo(string name, string desrc, string version, DateTime? d)
            {
                Name = name;
                Description = desrc;
                Version = version;

                if (d.HasValue)
                    Date = d.Value.ToString("dd.MM:yyyy hh:mm:ss");
            }
            public LibInfo() { }
        }
        #endregion
    }
}
