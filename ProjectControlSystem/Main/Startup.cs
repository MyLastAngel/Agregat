using Microsoft.VisualBasic.ApplicationServices;
using ProjectControlSystem.Windows;
using System;
using System.Linq;

namespace ProjectControlSystem
{
    class Startup
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var wrapper = new SingleInstanceApplicationWrapper();
            try
            {
                //var app = new ProjectApplication();
                //app.Run();

                wrapper.Run(args);
            }
            catch (Exception ex)
            {
                var win = new ExceptionWindow(ex) { Topmost = true };
                win.ShowDialog();
            }
        }
    }

    public class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase
    {
        #region Поля.
        ProjectApplication app;
        #endregion

        public SingleInstanceApplicationWrapper()
        {
            IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs e)
        {
            app = new ProjectApplication();
            app.Run();
            return false;
        }
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            app.Activate(e.CommandLine.ToArray());
        }
    }
}
