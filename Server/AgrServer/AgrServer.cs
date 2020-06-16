using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using ArgDb;
using ArgDb.Managers;
using ArgLib.Logger;
using ArgLib.Windows;
using System.ServiceModel.Description;

namespace AgrServer
{
    internal class AgrServerApplication : System.Windows.Application
    {
        #region Поля
        const string unit = "AgrServerApplication";

        readonly NotifyIcon _mNotifyIcon = new NotifyIcon();

        // Хост
        static ServiceHost host;

        UsersForm _userForm;

        // лог
        LogWindow logWin = null;
        readonly Log log = new Log();
        #endregion

        public AgrServerApplication()
        {
            LogManager.LogEmit += LogManager_LogEmit;

            var binding = new NetTcpBinding();
            binding.Name = "my_binding";
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.Security.Mode = SecurityMode.None;
            binding.MaxConnections = (int)AppConfig.MaxClients;

            // лимит на соединения
            var throttling = new ServiceThrottlingBehavior();
            throttling.MaxConcurrentSessions = (int)AppConfig.MaxClients;

            host = new ServiceHost(typeof(RLTTaskManagerService));
            host.Description.Behaviors.Add(throttling);
            host.AddServiceEndpoint(typeof(IRLTTaskManagerService), binding, HostManager.GetServerHost());
            host.Open();

            LogManager.LogInfo(unit, string.Format("Запуск сервера. Максимальное количество клиентов: {0}", AppConfig.MaxClients));

            _mNotifyIcon.MouseClick += MNotifyIconMouseClick;
            _mNotifyIcon.BalloonTipClicked += MNotifyIconClick;
            var menu = new ContextMenuStrip();

            ToolStripItem item = menu.Items.Add("Настройка", AgrServer.Properties.Resources.users_1);
            item.Click += MNotifyIconClick;

            item = menu.Items.Add("Сообщения", AgrServer.Properties.Resources.info);
            item.Click += InfoIconClick;

            menu.Items.Add("-");

            item = menu.Items.Add("Выход", AgrServer.Properties.Resources.Turn2);
            item.Click += ItemClick;
            _mNotifyIcon.ContextMenuStrip = menu;
            _mNotifyIcon.Icon = AgrServer.Properties.Resources.user_green1;

            _mNotifyIcon.Visible = true;

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        void host_Faulted(object sender, EventArgs e)
        {
            LogManager.LogError(unit, "Ошибка создания хоста (нужно перезагрузить сервер)");
        }

        void InfoIconClick(object sender, EventArgs e)
        {
            if (logWin != null)
            {
                try { logWin.Close(); }
                catch { }
            }

            logWin = new LogWindow(log) { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            logWin.Show();
        }


        void LogManager_LogEmit(object sender, LogEventArgs e)
        {
            if (Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                Dispatcher.Invoke(new Action(() => LogManager_LogEmit(sender, e)));
                return;
            }
            log.Add(e);

            DateTime dt = e.Time;
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\Agr\\" + dt.Year + "\\" + dt.Month + "\\" + dt.Day;
            Directory.CreateDirectory(dir);
            TextWriter sw = new StreamWriter(dir + "\\Server." + dt.Hour + ".txt", true);
            sw.WriteLine(dt.ToString("mm.ss.ff") + " | " + e.Unit + " | " + e.Type + " | " + e.Description + " | " + ((e.Ex == null) ? "" : e.Ex.Message));
            sw.Close();
        }

        void MNotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MNotifyIconClick(sender, e);
        }

        void MNotifyIconClick(object sender, EventArgs e)
        {
            if (_userForm == null)
            {
                _userForm = new UsersForm();
                _userForm.Closed += _userForm_Closed;
            }
            //_userForm.ApplyEvent += CfgWindowApplyEvent;
            //_cfgWindow.ReadStatus = _readStatus;
            //_cfgWindow.ReadState = _readState;
            _userForm.Show();
        }

        void _userForm_Closed(object sender, EventArgs e)
        {
            _userForm = null;
        }
        void ItemClick(object sender, EventArgs e)
        {
            Shutdown();
        }

    }
}
