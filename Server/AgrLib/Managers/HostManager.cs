using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;

namespace ArgDb.Managers
{
    public static class HostManager
    {
        public static string GetClientHost(string ip)
        {
            return string.Format("net.tcp://{0}:20238/RLTTaskManagerService", ip);
        }


#if !SILVERLIGHT
        public static string GetServerHost()
        {
            var h = GetHost();
            return string.Format("net.tcp://{0}:20238/RLTTaskManagerService", h);
        }
        static string GetHost()
        {
            var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    return addr.ToString();
            }

            return "localhost";
        }
#endif
    }

}
