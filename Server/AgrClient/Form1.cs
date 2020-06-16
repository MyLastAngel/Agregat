using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Forms;
using ArgDb;

namespace AgrClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();



            //foreach (IWCFService service in wcfservices)
            //{
            //    service.Start();
            //}
            ////Создаем клиента в отдельном потоке 
            //Thread cunsumerThread = new Thread(new ThreadStart(ServiceConsumer));
            //cunsumerThread.Start();

            //Console.ReadLine(); 
            var b = new NetTcpBinding();
            var channelFactory = new ChannelFactory<IRLTTaskManagerService>(b);
            _service = channelFactory.CreateChannel(
                new EndpointAddress(@"net.tcp://localhost:20238/RLTTaskManagerService"));
        }

        private readonly IRLTTaskManagerService _service;
        private void Button1Click(object sender, System.EventArgs e)
        {
            var users = _service.GetUsers();

            //List<int> clientsId = _service.GetClients();
            //foreach (int clientId in clientsId)
            //{
            //    Client client = _service.GetClient(clientId);
            //}

            //List<int> productsId = _service.GetProducts();
            //foreach (int productId in productsId)
            //{
            //    Product product = _service.GetProduct(productId);
            //}

            //List<int> projectsId = _service.GetProjects();
            //foreach (int projectId in projectsId)
            //{
            //    var project = _service.GetProject(projectId);
            //}
        }
    }
}
