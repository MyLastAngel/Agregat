using System.Windows.Forms;
using ArgDb;

namespace AgrServer
{
    public partial class ProductsForm : Form
    {
        public ProductsForm()
        {
            InitializeComponent();
        }

        private void ProductsFormLoad(object sender, System.EventArgs e)
        {
            Db.LoadProducts(false);
            FillProduct(0, -1);
        }
        private void FillProduct(int idx, int id)
        {
            user_list.Items.Clear();
            foreach (AgrDataSet.ProductRow drProduct in Db.Ds.Product.Select())
            {
                ListViewItem item = user_list.Items.Add(drProduct.Id.ToString(), drProduct.Id.ToString(), -1);
                item.SubItems.Add(drProduct.Name);
                item.SubItems.Add(drProduct.Description);
            }
        }
        private void AddUserBtnClick(object sender, System.EventArgs e)
        {
            AgrDataSet.ProductRow id = Db.AddProduct("");
            FillProduct(-1, id.Id);
        }

        private void RemoveUserBtnClick(object sender, System.EventArgs e)
        {

        }

        private void UserListSelectedIndexChanged(object sender, System.EventArgs e)
        {
            splitContainer1.Panel2.Enabled = (user_list.SelectedItems.Count == 1);
            project_list.Items.Clear();
            project_list.Items.Clear();
            if (user_list.SelectedItems.Count == 1)
            {
                AgrDataSet.ProductRow drProduct = Db.Ds.Product.
                    FindById(int.Parse(user_list.SelectedItems[0].Name));
                name_text.Text = drProduct.Name;
                description_text.Text = drProduct.Description;
                FillProjects(drProduct);
            }
            else
            {
                name_text.Text = "";
                description_text.Text = "";
            }
        }

        private void FillProjects(AgrDataSet.ProductRow drProduct)
        {
            foreach (AgrDataSet.ProjectProductRow drProductProject in drProduct.GetProjectProductRows())
            {
                AgrDataSet.ProjectsRow drProjects = drProductProject.ProjectsRow;
                AgrDataSet.ProjectRow[] drsProject = drProjects.GetProjectRows();
                project_list.Items.Add(drProjects.Id.ToString(),
                    drsProject.Length > 0 ? drsProject[0].Name : drProjects.Id.ToString(),
                    -1);
                FillClients(drProjects);
            }
        }

        private void FillClients(AgrDataSet.ProjectsRow drProjects)
        {
            foreach (AgrDataSet.ClientProjectRow drProjectClient in drProjects.GetClientProjectRows())
            {
                AgrDataSet.ClientRow drClient = drProjectClient.ClientRow;
                project_list.Items.Add(drClient.Id.ToString(), drClient.Name, -1);
            }
        }

        private void NameTextTextChanged(object sender, System.EventArgs e)
        {
            apply_btn.Enabled = name_text.Text.Length > 0;
        }

        private void DescriptionTextTextChanged(object sender, System.EventArgs e)
        {
            apply_btn.Enabled = true;
        }

        private void ApplyBtnClick(object sender, System.EventArgs e)
        {
            if (user_list.SelectedItems.Count == 0)
                return;

            ListViewItem item = user_list.SelectedItems[0];
            if (Db.EditProduct(int.Parse(item.Name), name_text.Text, description_text.Text))
                FillProduct(-1, int.Parse(item.Name));
        }
    }
}
