using ArgDb;
using ArgLib.Logger;
using ProjectControlSystem.Managers;
using ProjectControlSystem.MFPlannerWindows;
using ProjectControlSystem.Windows;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using about = ProjectControlSystem.Windows.AboutWindow;

namespace ProjectControlSystem
{
    public class ProjectApplication : Application
    {
        #region Поля
        readonly Log log = new Log();

        internal static readonly List<about.LibInfo> libInfo = new List<about.LibInfo>();
        #endregion

        #region Свойства
        public Log Log { get { return log; } }
        #endregion

        public ProjectApplication()
        {
            this.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;

            LogManager.LogEmit += LogManager_LogEmit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            #region загрузка версий библиотек
            var assembly = Assembly.GetAssembly(typeof(MainWindow));
            Object[] attr = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);

            var info = new about.LibInfo();
            info.Name = "ProjectControlSystem.exe (Менеджер проектов)";
            info.Description = "Клиент менеджера управления проектами 'Агрегат'";
            info.Version = (attr == null || attr.Length == 0) ? "не известна" : ((AssemblyFileVersionAttribute)attr[0]).Version;
            libInfo.Add(info);


            assembly = Assembly.GetAssembly(typeof(IRLTTaskManagerService));
            attr = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);

            info = new about.LibInfo();
            info.Name = "AgrLib.dll";
            info.Description = "Базовая библиотека связи.";
            info.Version = (attr == null || attr.Length == 0) ? "не известна" : ((AssemblyFileVersionAttribute)attr[0]).Version;
            libInfo.Add(info);
            #endregion

            //TrayIconManager.SetIcon(@"Images/package.ico");

            Activate(e.Args);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            TrayIconManager.Dispose();

            base.OnExit(e);
        }

        internal void Activate(string[] p)
        {
//#if Develop
//            if (MainWindow == null)
//                MainWindow = new MFPlannerWindow();
//#else
            if (MainWindow == null)
                MainWindow = new MainWindow();
//#endif

            MainWindow.Show();
            MainWindow.Activate();

            TrayIconManager.Init();
        }

        #region Обработчики событий
        void LogManager_LogEmit(object sender, LogEventArgs e)
        {
            if (Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                Dispatcher.Invoke(new Action(() => LogManager_LogEmit(sender, e)));
                return;
            }

            log.Add(e);
        }
        #endregion
    }
}
