using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WForm = System.Windows.Forms;
using WDraw = System.Drawing;
using System.Resources;
using System.IO;
using System.Globalization;
using System.Windows.Threading;
using System.Windows;

namespace ProjectControlSystem.Managers
{
    public static class TrayIconManager
    {
        #region Поля
        static WForm.NotifyIcon trayIcon = new WForm.NotifyIcon();
        static readonly DispatcherTimer baloonHideTimer = new DispatcherTimer();
        #endregion

        #region Свойства
        static ProjectApplication App { get { return Application.Current as ProjectApplication; } }
        #endregion

        static TrayIconManager()
        {
            trayIcon.Visible = true;
            trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;

            trayIcon.Text = "Менеджер проектов";
            trayIcon.BalloonTipText = "";

            trayIcon.BalloonTipClicked += trayIcon_BalloonTipClicked;

            baloonHideTimer.Tick += baloonHideTimer_Tick;
        }

        public static void Init()
        {
            SetDefault();
        }
        public static void SetDefault()
        {
            SetIcon("Images/logo.ico");
        }
        public static void SetMessageDefault()
        {
            SetIcon("Images/logo_mail.ico");
        }

        public static void SetBaloonTip(string text, int time)
        {
            baloonHideTimer.Stop();

            trayIcon.BalloonTipText = text;
            trayIcon.ShowBalloonTip(time);

            baloonHideTimer.Interval = new TimeSpan(0, 0, time);
            baloonHideTimer.Start();
        }
        public static void SetTip(string text)
        {
            trayIcon.Text = text;
        }

         static void SetIcon(string path)
        {
            try
            {
                var assembly = typeof(ProjectApplication).Assembly;
                var n = assembly.GetName().Name + ".g";
                var rm = new ResourceManager(n, assembly);

                using (ResourceSet rs = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true))
                {
                    var stream = (UnmanagedMemoryStream)rs.GetObject(path, true);
                    trayIcon.Icon = new WDraw.Icon(stream);
                }
            }
            catch
            {
            }
        }

        #region Обработчики событий
        static void mnuItem_Click(object sender, EventArgs e)
        {
            //var item = sender as WForm.ToolStripMenuItem;
            //if (item == null || !(item.Tag is ApplicationCommands))
            //    return;

            //MakeCommand((ApplicationCommands)item.Tag);
        }

        static void trayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (App != null && App.MainWindow != null)
            {
                App.MainWindow.Show();
                App.MainWindow.Activate();
            }
        }
        static void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (App != null && App.MainWindow != null)
            {
                App.MainWindow.Show();
                App.MainWindow.Activate();
            }
        }
     
        static void baloonHideTimer_Tick(object sender, EventArgs e)
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Visible = true;
            }

            baloonHideTimer.Stop();
        }
        #endregion

        public static void Dispose()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Icon = null;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }

        #region Статические методы
        static System.Drawing.Bitmap GetBitmap(string name)
        {
            var path = "pack://application:,,,/Images/" + name + ".png";

            var oUri = new Uri(path, UriKind.RelativeOrAbsolute);
            var frame = System.Windows.Media.Imaging.BitmapFrame.Create(oUri);
            using (MemoryStream stream = new MemoryStream())
            {
                var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
                enc.Frames.Add(frame);
                enc.Save(stream);

                using (var tempBitmap = new System.Drawing.Bitmap(stream))
                {
                    // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
                    // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
                    return new System.Drawing.Bitmap(tempBitmap);
                }
            }
        }
        #endregion
    }
}
