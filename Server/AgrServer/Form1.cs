using System;
using System.ServiceModel;
using System.Windows.Forms;
using ArgDb;
using ArgDb.Managers;

namespace AgrServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //var binding = new NetTcpBinding();
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            //binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

            //var _host = new ServiceHost(typeof(RLTTaskManagerService));
            //_host.AddServiceEndpoint(typeof(IRLTTaskManagerService), binding, HostManager.GetServerHost());
            //_host.Open();
        }



        private void UsersBtnClick(object sender, EventArgs e)
        {
            var f = new UsersForm();
            f.Show(this);
        }

        private void ClientsBtnClick(object sender, EventArgs e)
        {
            var f = new ClientsForm();
            f.Show(this);
        }

        private void ProductsBtnClick(object sender, EventArgs e)
        {
            var f = new ProductsForm();
            f.Show(this);
        }

        private void ProjectsBtnClick(object sender, EventArgs e)
        {
            var f = new ProjectsForm();
            f.Show(this);
        }
    }


}
