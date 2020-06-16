using System;
using System.Data;
using System.Windows.Forms;
using ArgDb;

namespace AgrServer
{
    public partial class ClientsForm : Form
    {
        public ClientsForm()
        {
            InitializeComponent();
        }
        private void ClientsFormLoad(object sender, EventArgs e)
        {
            Db.LoadClients(false);
            FillClient(0, -1);
        }
        private void FillClient(int idx, int id)
        {
            user_list.Items.Clear();
            foreach (AgrDataSet.ClientRow drClient in Db.Ds.Client.Select())
            {
                ListViewItem item = user_list.Items.Add(drClient.Id.ToString(), drClient.Id.ToString(), -1);
                item.SubItems.Add(drClient.Name);
                //item.SubItems.Add(drClient.Description);
            }
        }

        private void AddUserBtnClick(object sender, EventArgs e)
        {
            //var f = new UserForm(null, null, Db.Ds.User.Select(s => s.Name));
            //if (f.ShowDialog(this) == DialogResult.OK)
            {
                AgrDataSet.ClientRow id = Db.AddClient("");
                FillClient(-1, id.Id);
            }
        }

        private void RemoveUserBtnClick(object sender, EventArgs e)
        {
            if (user_list.Items.Count == 0)
                return;
            if (MessageBox.Show(this, "Удалить выделенных пользователей?", "Удаление пользователей", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            foreach (ListViewItem item in user_list.Items)
            {
                AgrDataSet.ClientRow dr = Db.Ds.Client.FindById(int.Parse(item.Name));
                dr.Delete();
            }
            FillClient(0, -1);
        }

        private void UserListSelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Enabled = (user_list.SelectedItems.Count == 1);
            project_list.Items.Clear();
            project_list.Items.Clear();
            if (user_list.SelectedItems.Count == 1)
            {
                AgrDataSet.ClientRow drClient = Db.Ds.Client.
                    FindById(int.Parse(user_list.SelectedItems[0].Name));
                name_text.Text = drClient.Name;
                //description_text.Text = drClient.Description;
                FillProjects(drClient);
            }
            else
            {
                name_text.Text = "";
                description_text.Text = "";
            }
        }

        private void FillProjects(AgrDataSet.ClientRow drClient)
        {
            foreach (AgrDataSet.ClientProjectRow drClientProject in drClient.GetClientProjectRows())
            {
                AgrDataSet.ProjectsRow drProjects = drClientProject.ProjectRow;
                AgrDataSet.ProjectRow[] drsProject = drProjects.GetProjectRows();
                project_list.Items.Add(drProjects.Id.ToString(),
                    drsProject.Length > 0 ? drsProject[0].Name : drProjects.Id.ToString(),
                    -1);
                FillProducts(drProjects);
            }
        }

        private void FillProducts(AgrDataSet.ProjectsRow drProjects)
        {
            foreach (AgrDataSet.ProjectProductRow drProjectProject in drProjects.GetProjectProductRows())
            {
                AgrDataSet.ProductRow drProduct = drProjectProject.ProductRow;
                project_list.Items.Add(drProduct.Id.ToString(), drProduct.Name, -1);
            }
        }

        private void NameTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = name_text.Text.Length > 0;
        }

        private void DescriptionTextTextChanged(object sender, EventArgs e)
        {
            apply_btn.Enabled = true;
        }

        private void apply_btn_Click(object sender, EventArgs e)
        {
            if (user_list.SelectedItems.Count == 0)
                return;
           
            ListViewItem item = user_list.SelectedItems[0];
            if (Db.EditClient(int.Parse(item.Name), name_text.Text, description_text.Text))
                FillClient(-1, int.Parse(item.Name));
        }
    }
}
