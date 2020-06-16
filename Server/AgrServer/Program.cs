using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace AgrServer
{
    static class Startup
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var wrapper = new SingleInstanceApplicationWrapper();
            try
            {
                wrapper.Run(args);
            }
            catch (Exception ex)
            {
                //var win = new ExceptionWindow(ex) { Topmost = true };
                //win.ShowDialog();
            }
        }
    }
    public class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase
    {
        #region  Поля.
        AgrServerApplication _pctrlApp;
     
        #endregion

        public SingleInstanceApplicationWrapper()
        {
            IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs e)
        {
            _pctrlApp = new AgrServerApplication();
            _pctrlApp.Run();
            return false;
        }
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            //_pctrlApp.Activate(e.CommandLine.ToArray());
        }
        protected override bool OnUnhandledException(Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            //var win = new ExceptionWindow(e.Exception) { Topmost = true };
            //win.ShowDialog();

            return base.OnUnhandledException(e);
        }
    }
}
