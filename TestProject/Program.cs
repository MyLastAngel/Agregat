using ArgDb;
using ArgDb.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace TestProject
{
    class Program
    {
        #region Поля
        static readonly List<ThreadState> threadPool = new List<ThreadState>();
        static Thread threadConsole;


        static string hostIP = "172.20.137.1";
        #endregion

        static void Main(string[] args)
        {
            // аргумент хоста
            if (args.Length == 1)
                hostIP = args[0].Trim();

            StartMultiThreadTest();

            threadConsole = new Thread(WriteConsoleThread);
            threadConsole.Start();


            Console.ReadKey();
        }

        static void StartMultiThreadTest()
        {
            int count = 60;
            for (uint i = 0; i < count; i++)
            {
                var state = new ThreadState
                {
                    Thread = new Thread(ClientThread),
                    Index = i,
                    LastRequest = DateTime.Now
                };
                threadPool.Add(state);

                state.Thread.Start(state);

                Console.Clear();
                Console.WriteLine(string.Format("Запускаем клиентов для '{0}' [{1}/{2}]", hostIP, (i + 1), count));

                Thread.Sleep(200);
            }
        }

        static void WriteConsoleThread()
        {
            while (true)
            {
                Console.Clear();

                foreach (var state in threadPool)
                {
                    if (state.Ex != null)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine(string.Format("{0} [{1}] - {2}", state.IsRequestWait ? "???" : "", state.Index, state.LastRequest.TimeOfDay));
                }

                Thread.Sleep(1000);
            }
        }

        static void ClientThread(object obj)
        {
            //var host = HostManager.GetClientHost("192.168.186.1");
            var host = HostManager.GetClientHost(hostIP);

            var state = (ThreadState)obj;

            var binding = new NetTcpBinding
                              {
                                  Name = "my_binding",
                                  HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                                  MaxBufferSize = UInt16.MaxValue * 16,
                              };
            binding.Security.Mode = SecurityMode.None;
            //binding.
            binding.TransferMode = TransferMode.Buffered;

            //var b = new BasicHttpBinding(BasicHttpSecurityMode.None);

            var channelFactory = new ChannelFactory<IRLTTaskManagerService>(binding);
            var eP = new EndpointAddress(host);
            var service = channelFactory.CreateChannel(eP);
            channelFactory.Faulted += channelFactory_Faulted;

            var fromDate = DateTime.Now.AddDays(-100);

            try
            {
                while (true)
                {
                    state.LastRequest = DateTime.Now;

                    //Console.WriteLine(string.Format("[{0}] Чтение", state.Index));

                    var result = new List<AgrProject>();
                    var previousId = -1;

                    while (true)
                    {
                        state.IsRequestWait = true;

                        var projects = service.GetProjects(fromDate, previousId);
                        if (projects == null)
                            break;

                        state.IsRequestWait = false;

                        result.AddRange(projects);
                        if (projects.Count < 10)
                            break;

                        var previous = projects.Last();
                        previousId = previous.ProjectId;
                        //fromDate = previous.TimeEndActual ?? DateTime.MinValue;

                        //foreach (var p in projects)
                        //    Console.Write(string.Format("{0} [{1}]\n", p.ProjectId, p.ID));


                        //Console.WriteLine(previous.ProjectId + " : " + previous.TimeEndActual);
                    }

                    // ждем 500 мсек
                    Thread.Sleep(3000);
                    //Console.Clear();
                }
            }
            catch (Exception ex)
            {
                state.Ex = ex;
                //Console.WriteLine(string.Format("[{0}] Процесс упал exception: {1}", state.Index, ex.Message));
            }
        }

        static void channelFactory_Faulted(object sender, EventArgs e)
        {
        }
    }

    class ThreadState
    {
        #region Свойства
        public uint Index;
        public Thread Thread;
        public DateTime LastRequest;
        public bool IsRequestWait;
        public Exception Ex;
        #endregion
    }
}
