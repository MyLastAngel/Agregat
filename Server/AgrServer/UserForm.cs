using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AgrServer
{
    public partial class UserForm : Form
    {
        private readonly IEnumerable<string> _names;
        public string User
        {
            get { return name_text.Text; }
        }
        public string Login
        {
            get { return login_text.Text; }
        }
       
        public UserForm(string userName, string login, IEnumerable<string> names)
        {
            _names = names;
            InitializeComponent();
            Text = userName == null
                ? ("Добавление пользователя")
                : ("Изменение пользователя");
            if (userName != null)
            {
                name_text.Text = userName;
                login_text.Text = login;
            }
        }

        private void NameTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = name_text.Text.Length > 0;
        }

        private void ApplyBtnClick(object sender, EventArgs e)
        {
            if (_names.FirstOrDefault(s => s == name_text.Text) != null)
            {
                MessageBox.Show(this, "Пользователь с таким номером уже существует!");
                DialogResult = DialogResult.None;
            }
        }
    }
}
