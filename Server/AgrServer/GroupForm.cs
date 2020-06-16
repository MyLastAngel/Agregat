using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class GroupForm : Form
    {
        private readonly IEnumerable<string> _names;
        public string Group
        {
            get { return name_text.Text; }
        }
        public GroupForm(string groupName, IEnumerable<string> names)
        {
            _names = names;
            InitializeComponent();
            Text = groupName == null ? "Добавление группы" : "Изменение группы";
            if (groupName != null)
                name_text.Text = groupName;
        }

        private void NameTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = name_text.Text.Length > 0;
        }

        private void ApplyBtnClick(object sender, EventArgs e)
        {
            if (_names.FirstOrDefault(s => s == name_text.Text) != null)
            {
                MessageBox.Show(this, "Группа с таким номером уже существует!");
                DialogResult = DialogResult.None;
            }
        }
    }
}
