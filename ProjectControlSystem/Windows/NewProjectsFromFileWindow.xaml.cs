using ArgLib.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for NewProjectsFromFileWindow.xaml
    /// </summary>
    public partial class NewProjectsFromFileWindow : Window
    {
        #region Поля
        const string unit = "NewProjectsFromFileWindow";

        readonly ObservableCollection<NewProject> projects = new ObservableCollection<NewProject>();
        #endregion

        #region Свойства
        public ObservableCollection<NewProject> Projects { get { return projects; } }
        #endregion

        public NewProjectsFromFileWindow(string path = @"D:\Dropbox\Develop\Visual Studio\Agregat\04308 Греленс.CSV")
        {
            InitializeComponent();

            var lines = File.ReadAllLines(path);
            string id = lines[0].Trim(), customer = lines[1].Trim(), customerName = lines[2].Trim();

            DateTime time = DateTime.Now;
            var index = 3;
            while (index < lines.Length)
            {
                if (DateTime.TryParse(lines[index].TrimEnd(';'), out time))
                    break;

                var sub = lines[index].Split(';');
                if (sub.Length != 2)
                {
                    LogManager.LogError(unit, string.Format("Ошибка формата: '{0}'", lines[index]));
                    continue;
                }

                var p = new NewProject { Id = id, Customer = customer, CustomerName = customerName, Product = sub[0].Trim(), Options = sub[1].Trim() };
                projects.Add(p);

                index++;
            }

            index++;
            var comment = lines[index].Trim();

            foreach (var p in projects)
            {
                p.Comments = comment;
                p.EndTime = time;
            }

            listProject.ItemsSource = projects;

            //№Проекта
            //Контрагент
            //Проект
            //Изделие сторым столлбиком Опции к нему. (Изделий может быть много, 
            //либо делать много файликов, лиюо , как сейчас, будут создоваться доп. строки с реквизитом изделие и опции)
            //Дата отгрузки
            //комментарии   (Сюда поместил через запятую марку фриона и дополнения к заказу)
        }

        #region Обработчики события
        void listProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewProject p;

            // Если у нас был предыдущий элемент то сохраняем изменения
            if (e.RemovedItems.Count == 1 && e.RemovedItems[0] is NewProject)
            {
                p = ((NewProject)e.RemovedItems[0]);
                if (!cProject.CheckIsSame(p))
                {
                    var rez = MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (rez == MessageBoxResult.Yes)
                        p.Set(cProject);
                }
            }

            // утснавливаем значение
            p = listProject.SelectedItem as NewProject;
            cProject.ClearData();

            mnuConfirm.IsEnabled = mnuDelete.IsEnabled =
            btnDelete.IsEnabled = btnConfirm.IsEnabled =
            cProject.IsEnabled = false;

            if (p == null)
                return;

            mnuConfirm.IsEnabled = mnuDelete.IsEnabled =
            btnDelete.IsEnabled = btnConfirm.IsEnabled =
            cProject.IsEnabled = true;

            cProject.SetData(p);
        }

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

        // Подтверждение
        void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var p = listProject.SelectedItem as NewProject;
            if (p == null)
                return;

            if (!cProject.CheckValid(p))
                return;

            p.IsConfirm = true;
        }

        // удаляем
        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var p = listProject.SelectedItem as NewProject;
            if (p == null || MessageBox.Show("Уверены что хотите удалить выбранный проект?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            listProject.SelectedItem = null;
            projects.Remove(p);
        }

        void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // Если у нас был предыдущий элемент то сохраняем изменения
            if (listProject.SelectedItem is NewProject)
            {
                var p = (NewProject)listProject.SelectedItem;
                if (!cProject.CheckIsSame(p))
                {
                    var rez = MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (rez == MessageBoxResult.Yes)
                        p.Set(cProject);
                }
            }

            var nonConfirm = projects.FirstOrDefault(x => !x.IsConfirm);
            if (nonConfirm != null)
            {
                MessageBox.Show("Подтвердите проект", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                listProject.SelectedItem = nonConfirm;

                DialogResult = null;
                return;
            }

            // проверяем
            foreach (var p in projects)
            {
                if (!cProject.CheckValid(p))
                {
                    listProject.SelectedItem = p;

                    DialogResult = null;
                    return;
                }
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
