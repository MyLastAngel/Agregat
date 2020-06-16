using ArgDb;
using ArgLib.Logger;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectControlSystem
{
    public class NotNeedDateTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return "ОШИБКА!";

            if ((bool?)values[0] == true)
                return "не требуется";

            if (values[1] is DateTime)
                return ((DateTime)values[1]).ToString("dd.MM.yyyy");
            else if (values[1] is DateTime? && ((DateTime?)values[1]).HasValue)
                return (((DateTime?)values[1]).Value).ToString("dd.MM.yyyy");
            else
            {
                if (parameter as string == "-")
                    return "";

                return "__.__.____";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsManagerSetPlanDateToBrushSourceConverter : IMultiValueConverter
    {
        #region Поля
        static Brush dError = new SolidColorBrush(Color.FromArgb(255, 178, 100, 100));
        #endregion

        static IsManagerSetPlanDateToBrushSourceConverter()
        {
            dError.Freeze();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is bool))
                return Brushes.Red;

            if (values[0] is bool && (bool)values[0] == false)
                return SystemColors.ControlBrushKey;

            if (values[1] is DateTime? && ((DateTime?)values[1]).HasValue)
                return SystemColors.ControlBrushKey;

            return dError;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class TimePlanStateToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Visibility.Visible;

            //<Binding Path="IsManagerSetPlanDate" Mode="OneWay"/>
            //<Binding Path="Is_ITO_G_NotNeed" Mode="OneWay"/>
            if (!(values[0] is bool) || !(values[1] is bool))
            {
                LogManager.LogError("TimePlanStateToVisibilityConverter", "Не совпадение типов данных");
                result = Visibility.Visible;
            }
            else
            {
                if ((bool)values[0] == false || (bool)values[1] == true)
                    result = Visibility.Collapsed;
                else
                    result = Visibility.Visible;
            }

            if (parameter as string == "invert")
                return result == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            else
                return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
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
    public class FullDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return ((DateTime)value).ToString("dd.MM.yyyy HH:mm:ss");
            else if (value is DateTime? && ((DateTime?)value).HasValue)
                return (((DateTime?)value).Value).ToString("dd.MM.yyyy HH:mm:ss");
            else
                return "__.__.____";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    public class NullDateTimeToFalseConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime? && ((DateTime?)value).HasValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    public class NullDateTimeToCollapseConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime? && ((DateTime?)value).HasValue)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    public class TimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
                return "--:--:--";
            else
                return ((DateTime)value).ToString("HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class StateToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            var path = "empty:";
            switch ((bool)value)
            {
                case true:
                    path = "pack://application:,,,/Images/true.png";
                    break;
                case false:
                    path = "pack://application:,,,/Images/false.png";
                    break;
            }

            Uri oUri = new Uri(path, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class UserToEditBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = 0;
            if (string.IsNullOrEmpty(parameter as string) || !int.TryParse(parameter as string, out id))
                return null;

            var path = "pack://application:,,,/Images/index_view.png";

            if ((UserManager.Rights & (UserRight)id) != 0)
                path = "pack://application:,,,/Images/index_preferences.png";

            Uri oUri = new Uri(path, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class FilterToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            var path = "empty:";
            switch ((bool)value)
            {
                case true:
                    path = "pack://application:,,,/Images/funnel_new.png";
                    break;
                case false:
                    path = "pack://application:,,,/Images/funnel.png";
                    break;
            }

            Uri oUri = new Uri(path, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class BoolToBrushSourceConverter : IValueConverter
    {
        #region Поля
        internal static Brush bError = Brushes.Transparent, bOk = Brushes.Green;
        #endregion

        static BoolToBrushSourceConverter()
        {
            bOk = new SolidColorBrush(Color.FromArgb(125, 144, 238, 144));
            ((SolidColorBrush)bOk).Freeze();

            bError = new SolidColorBrush(Color.FromArgb(125, 178, 100, 100));
            ((SolidColorBrush)bError).Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return Brushes.Transparent;

            var result = (bool?)value;
            if (!result.HasValue)
                return Brushes.Transparent;

            if (result.Value == true)
                return bOk;
            else
                return Brushes.Transparent;
            //return bError;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class ProjectStateToBrushSourceConverter : IValueConverter
    {
        #region Поля
        public static SolidColorBrush dOk = new SolidColorBrush(Color.FromArgb(125, 144, 238, 144)),
                                  dWarn = new SolidColorBrush(Color.FromArgb(125, 216, 224, 130)),
                                  dError = new SolidColorBrush(Color.FromArgb(125, 178, 100, 100)),
                                  dStop = new SolidColorBrush(Color.FromArgb(125, 0, 50, 90)),
                                  dOk_Selected = new SolidColorBrush(Color.FromArgb(200, 144, 238, 144)),
                                  dWarn_Selected = new SolidColorBrush(Color.FromArgb(200, 216, 224, 130)),
                                  dError_Selected = new SolidColorBrush(Color.FromArgb(200, 178, 100, 100)),
                                  dStop_Selected = new SolidColorBrush(Color.FromArgb(125, 0, 50, 90));
        #endregion

        static ProjectStateToBrushSourceConverter()
        {
            dOk.Freeze();
            dOk_Selected.Freeze();

            dWarn.Freeze();
            dWarn_Selected.Freeze();

            dError.Freeze();
            dError_Selected.Freeze();

            dStop.Freeze();
            dStop_Selected.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ProjectState))
                return Brushes.Transparent;

            switch ((ProjectState)value)
            {
                #region OK
                case ProjectState.Ok:
                    {
                        if (parameter as string == "selected")
                            return dOk_Selected;
                        else
                            return dOk;
                    }
                #endregion
                #region Warning
                case ProjectState.Warning:
                    {
                        if (parameter as string == "selected")
                            return dWarn_Selected;
                        else
                            return dWarn;
                    }
                #endregion
                #region Error
                case ProjectState.Error:
                    {
                        if (parameter as string == "selected")
                            return dError_Selected;
                        else
                            return dError;
                    }
                #endregion
                #region Stop
                case ProjectState.Stop:
                    {
                        if (parameter as string == "selected")
                            return dStop_Selected;
                        else
                            return dStop;
                    }
                #endregion
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class StringToBrushSourceConverter : IValueConverter
    {
        #region Поля
        static string[] sGood = new string[] { "Да", "Готов", "Не надо" };
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return Brushes.Transparent;

            if (sGood.Contains((string)value))
                return BoolToBrushSourceConverter.bOk;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolToStringSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return "----";

            var result = (bool?)value;
            if (!result.HasValue)
                return "----";

            if (result.Value == true)
                return "Нет";
            else
                return "Да";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value == true)
                return "Да";
            else
                return "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && ((string)value).ToLower() == "да")
                return true;
            else
                return false;
        }
    }

    public class RequestToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<OMTSRequest> && parameter is string)
            {
                switch ((string)parameter)
                {
                    case "e":
                        {
                            if (((IEnumerable<OMTSRequest>)value).Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Electrician) > 0)
                                return Brushes.Red;

                            break;
                        }
                    case "g":
                        {
                            if (((IEnumerable<OMTSRequest>)value).Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Hydraulics) > 0)
                                return Brushes.Red;

                            break;
                        }
                }
            }

            return SystemColors.ControlBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && ((string)value).ToLower() == "да")
                return true;
            else
                return false;
        }
    }
    public class RequestToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<OMTSRequest> && parameter is string)
            {
                switch ((string)parameter)
                {
                    case "e":
                        {
                            if (((IEnumerable<OMTSRequest>)value).Count(x => x.Type == OMTSRequestType.Electrician) > 0)
                                return Visibility.Visible;

                            break;
                        }
                    case "g":
                        {
                            if (((IEnumerable<OMTSRequest>)value).Count(x => x.Type == OMTSRequestType.Hydraulics) > 0)
                                return Visibility.Visible;

                            break;
                        }
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && ((string)value).ToLower() == "да")
                return true;
            else
                return false;
        }
    }

    public class NullToVisibleSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class TrueToVisibleSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class FalseToVisibleSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value == false)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
    public class TrueToFalseSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return false;

            var result = (bool?)value;
            if (!result.HasValue)
                return false;

            return !result.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class ViewModeToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = 0;

            if (!(value is ProjectViewMode) || string.IsNullOrEmpty(parameter as string) || !int.TryParse(parameter as string, out id))
                return false;

            return (ProjectViewMode)value == (ProjectViewMode)id;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = 0;

            if (value is bool && (bool)value && !string.IsNullOrEmpty(parameter as string) && int.TryParse(parameter as string, out id))
                return (ProjectViewMode)id;
            else
                return ProjectViewMode.Main;


        }

        #endregion
    }

    public class UserToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = 0;

            //if (UserManager.IsGuest || string.IsNullOrEmpty(parameter as string) || !int.TryParse(parameter as string, out id))
            //    return false;

            if (string.IsNullOrEmpty(parameter as string) || !int.TryParse(parameter as string, out id))
                return false;

            switch ((ProjectViewMode)id)
            {
                #region Commercial
                case ProjectViewMode.Commercial:
                    return (UserManager.Rights & UserRight.EditCommercial) != 0;
                #endregion
                #region ITO
                case ProjectViewMode.ITO:
                    return (UserManager.Rights & UserRight.EditITO) != 0;
                #endregion
                #region Manufacture
                case ProjectViewMode.Manufacture:
                    return (UserManager.Rights & UserRight.EditManufacture) != 0;
                #endregion
                #region OMTS
                case ProjectViewMode.OMTS:
                    return (UserManager.Rights & UserRight.EditOMTS) != 0;
                #endregion
                #region OTK
                case ProjectViewMode.OTK:
                    return (UserManager.Rights & UserRight.EditOTK) != 0;
                #endregion
                #region Warehouse
                case ProjectViewMode.Warehouse:
                    return (UserManager.Rights & UserRight.EditWarehouse) != 0;
                #endregion
            }

            throw new NotFiniteNumberException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = 0;

            if (value is bool && (bool)value && !string.IsNullOrEmpty(parameter as string) && int.TryParse(parameter as string, out id))
                return (ProjectViewMode)id;
            else
                return ProjectViewMode.Main;


        }

        #endregion
    }

    public class DebtToBrushSourceConverter : IValueConverter
    {
        #region Поля
        internal static Brush bError = Brushes.Red;
        #endregion

        static DebtToBrushSourceConverter()
        {
            bError = new SolidColorBrush(Color.FromArgb(50, 178, 100, 100));
            ((SolidColorBrush)bError).Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint && (uint)value > 0)
                return bError;

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class OnlineToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) && (bool)(value))
                return Colors.Green;
            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class UToTextDecorationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ProjectConfiguration.HasU(value as string))
                return TextDecorations.Underline;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
