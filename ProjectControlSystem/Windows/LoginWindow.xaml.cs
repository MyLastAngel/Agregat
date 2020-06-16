using ArgDb;
using ProjectControlSystem.Managers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Поля
        const string cAllGroups = "ВСЕ ГРУППЫ";

        readonly List<AgrUser> users = new List<AgrUser>();
        #endregion

        public LoginWindow()
        {
            InitializeComponent();

            InitData();
        }

        void InitData()
        {
            cmbGroups.ItemsSource = null;
            users.Clear();

            // Грузим пользователей
            var readUsers = ServiceManager.GetUsers();
            if (readUsers != null)
                users.AddRange(readUsers);

            var groups = new List<string> { cAllGroups };
            foreach (var u in users)
            {
                if (!groups.Contains(u.Group))
                    groups.Add(u.Group);
            }

            cmbGroups.ItemsSource = groups;

            cmbGroups.SelectedIndex = 0;
            cmbGroups.IsDropDownOpen = true;
        }

        #region Обработчики событий
        // Пользователи для групп
        void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbUsers.ItemsSource = null;

            var group = cmbGroups.SelectedItem as string;
            if (string.IsNullOrEmpty(group))
                return;

            var userList = new List<AgrUser>();
            foreach (var u in users)
            {

                if (u.Group == group || group == cAllGroups)
                    userList.Add(u);
            }

            cmbUsers.ItemsSource = userList;
            cmbUsers.IsDropDownOpen = true;
        }

        // Регистрация
        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = cmbUsers.SelectedItem as AgrUser;
            if (user == null)
            {
                MessageBox.Show("Выберите пользователя", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (!UserManager.Login(user, txtPassword.Password))
            {
                MessageBox.Show("Не удалось выполнить вход пользователем.\nПроверьте пользователя/пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            Close();
        }

        // регистрация гостем
        void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserManager.Logout();
            Close();
        }

        // отмена
        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
