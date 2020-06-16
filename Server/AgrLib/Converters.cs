using ArgLib.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ArgLib
{
    #region LogWindow Конвертеры.
    public class ImageSourceConverter : IValueConverter
    {
        #region Поля.
        static BitmapImage error;
        static BitmapImage info;
        #endregion

        static ImageSourceConverter()
        {
            var uri = new Uri("/ArgDb;component/Images/log_error.png", UriKind.RelativeOrAbsolute);
            error = new BitmapImage(uri);

            uri = new Uri("/ArgDb;component/Images/log_info.png", UriKind.RelativeOrAbsolute);
            info = new BitmapImage(uri);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LogType))
                return null;

            switch ((LogType)value)
            {
                case LogType.Error:
                    return error;
                case LogType.Info:
                case LogType.Debug:
                    return info;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ExceptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetMessage(value as Exception);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        string GetMessage(Exception ex)
        {
            if (ex == null)
                return "";

            var str = ex.Message;

            if (ex.InnerException != null)
                str += Environment.NewLine + GetMessage(ex.InnerException);

            str += Environment.NewLine + ex.ToString();

            return str;
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return ((DateTime)value).ToString("dd.MM.yyyy");
            else if (value is DateTime? && ((DateTime?)value).HasValue)
                return (((DateTime?)value).Value).ToString("dd.MM.yyyy");
            else
                return "__.__.____";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    #endregion
}
