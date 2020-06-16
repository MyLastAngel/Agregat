using System;
using System.Data;
using System.Windows.Forms;
using ArgDb;

namespace AgrServer
{
    public partial class UsersForm : Form
    {
        public UsersForm()
        {
            InitializeComponent();
        }

        private void UsersFormLoad(object sender, EventArgs e)
        {
            Db.LoadUsers();
            //Type enumTypeDataType = typeof(UserRight);
            //foreach (UserRight type in Enum.GetValues(enumTypeDataType))
            //{
            //    FieldInfo fi = enumTypeDataType.GetField(Enum.GetName(enumTypeDataType, type));
            //    var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            //    if (dna == null)
            //        continue;

            //    group_list.Items.Add(((UserRight)fi.GetValue(fi)).ToString(), dna.Description, -1);
            //}

            FillUser(0, null);
        }

        private void FillUser(int idx, string name)
        {
            _right = 0;
            _group = "";
            user_list.Items.Clear();
            foreach (AgrDataSet.UserRow user in Db.Ds.User)
            {
                ListViewItem item = user_list.Items.Add(user.Name, user.Name, -1);
                item.SubItems.Add(user.Group);
            }
        }

        //private void FillUser(int idx, string name)
        //{
        //    user_list.Items.Clear();
        //    if (user_list.SelectedItems.Count != 1)
        //        return;
        //    ListViewItem groupItem = user_list.SelectedItems[0];
        //    AgrDataSet.GroupRow drGroup = Db.Ds.Group.FindByName(groupItem.Name);
        //    foreach (AgrDataSet.UserRow drUser in drGroup.GetUserRows())
        //    {
        //        ListViewItem userItem = user_list.Items.Add(drUser.Name, drUser.Name, -1);
        //        userItem.SubItems.Add(drUser.Login);
        //    }
        //}

        private void AddUserBtnClick(object sender, EventArgs e)
        {
            var f = new UserForm(null, null, Db.Ds.User.Select(s => s.Name));
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                if (Db.AddUser(f.User, f.Login, "", 0))
                    FillUser(-1, f.User);
            }
        }
        private void remove_user_btn_Click(object sender, EventArgs e)
        {
            if (user_list.SelectedItems.Count != 1)
                return;
            if (MessageBox.Show(this, "Удалить выделенного пользователя?", "Удаление пользователя", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            int idx = user_list.SelectedItems[0].Index;
            foreach (ListViewItem item in user_list.SelectedItems)
            {
                AgrDataSet.UserRow dr = Db.Ds.User.FindByName(item.Name);
                dr.Delete();
            }
            FillUser(idx, null);
        }
        private void UserListSelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Enabled = (user_list.SelectedItems.Count == 1);
            if (user_list.SelectedItems.Count == 1)
            {
                name_text.Text = user_list.SelectedItems[0].Text;
                AgrDataSet.UserRow drUser = Db.Ds.User.FindByName(user_list.SelectedItems[0].Text);
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                login_text.Text = drUser.Login;
                switch (drUser.Group)
                {
                    case "Коммерческий отдел":
                        radioButton1.Checked = true;
                        break;
                    case "Иженерно техничский отдел":
                        radioButton2.Checked = true;
                        break;
                    case "Склад":
                        radioButton3.Checked = true;
                        break;
                    case "ОМТС":
                        radioButton4.Checked = true;
                        break;
                    case "Производство":
                        radioButton5.Checked = true;
                        break;
                    case "ОТК":
                        radioButton6.Checked = true;
                        break;
                    case "Администраторы":
                        radioButton7.Checked = true;
                        break;
                }
                //foreach (ListViewItem item in group_list.Items)
                //    item.Checked = ((ulong)(UserRight)Enum.Parse(typeof(UserRight), item.Name) & drUser.Groups) != 0;
                apply_btn.Enabled = false;
            }
            else
            {
                name_text.Text = "";
                //group_list.Text = "";
                //foreach (ListViewItem item in group_list.Items)
                //    item.Checked = false;
            }
        }

        private void UserListDoubleClick(object sender, EventArgs e)
        {
            //if (user_list.SelectedItems.Count == 0)
            //    return;
            //ListViewItem groupItem = user_list.SelectedItems[0];

            //if (user_list.SelectedItems.Count != 1)
            //    return;
            //ListViewItem userItem = user_list.SelectedItems[0];

            //var f = new UserForm(groupItem.Text, userItem.Text, userItem.SubItems[1].Text, Db.Ds.User.Select(s => s.Name));
            //if (f.ShowDialog(this) == DialogResult.OK)
            //{
            //    if (Db.EditUser(groupItem.Name, userItem.Text, f.User, f.Login))
            //        FillUser(-1, f.User);
            //}
        }

        private void ApplyBtnClick(object sender, EventArgs e)
        {
            if (user_list.SelectedItems.Count == 0)
                return;
            //ulong groups = 0;
            //foreach (ListViewItem gItem in group_list.Items)
            //{
            //    if (gItem.Checked)
            //    {
            //        var g = (ulong)(UserRight) Enum.Parse(typeof (UserRight), gItem.Name);
            //        groups |= g;
            //    }
            //}
            ListViewItem item = user_list.SelectedItems[0];
            if (Db.EditUser(item.Text, name_text.Text, login_text.Text, Group, Right))
            {
                FillUser(-1, name_text.Text);
                apply_btn.Enabled = false;
            }
        }

        private void NameTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = (name_text.Text.Length > 0) && (Group.Length > 0);
        }

        private void LoginTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = (name_text.Text.Length > 0) && (Group.Length > 0);
        }

        private uint _right = 0;
        private string _group = "";
        private void GroupCheckedChanged(object sender, EventArgs e)
        {
            var btn = (RadioButton) sender;
            _right = uint.Parse((string) btn.Tag);
            _group = btn.Text;
            apply_btn.Enabled = (name_text.Text.Length > 0) && (Group.Length > 0);
        }

        private string Group
        {
            get
            {
                return _group;
                //if (radioButton1.Checked)
                //    return "Коммерческий отдел";
                //if (radioButton2.Checked)
                //    return "Иженерно техничский отдел";
                //if (radioButton3.Checked)
                //    return "Склад";
                //if (radioButton4.Checked)
                //    return "ОМТС";
                //if (radioButton5.Checked)
                //    return "Производство";
                //if (radioButton6.Checked)
                //    return "ОТК";
                //if (radioButton7.Checked)
                //    return "Администраторы";
                //return "";
            }
        }

        private uint Right
        {
            get { return _right; }
        }
    }
}
