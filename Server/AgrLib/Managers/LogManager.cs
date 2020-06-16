using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ArgLib.Logger
{
    public static class LogManager
    {
        public static void LogError(string unit, string descr, Exception ex)
        {
            DoLogEmit(LogType.Error, unit, descr, ex);
        }
        public static void LogError(string unit, string descr)
        {
            DoLogEmit(LogType.Error, unit, descr, null);
        }

        public static void LogDebug(string unit, string descr)
        {
            DoLogEmit(LogType.Debug, unit, descr, null);
        }

        public static void LogInfo(string unit, string descr)
        {
            DoLogEmit(LogType.Info, unit, descr, null);
        }
        public static void LogTrace(string unit, string descr)
        {
            DoLogEmit(LogType.Trace, unit, descr, null);
        }

        #region События.
        public static event EventHandler<LogEventArgs> LogEmit;

        static void DoLogEmit(LogType type, string unit, string descr, Exception ex)
        {
            if (LogEmit != null)
                LogEmit(null, new LogEventArgs(type, unit, descr, ex));
        }
        #endregion
    }

    public enum LogType
    {
        Trace,
        Debug,
        Info,
        Error
    }
    public class LogEventArgs : EventArgs
    {
        #region Свойства.
        public LogType Type { get; private set; }

        public DateTime Time { get; private set; }
        public string Unit { get; private set; }
        public string Description { get; private set; }

        public Exception Ex { get; private set; }
        #endregion

        internal LogEventArgs(LogType type, string unit, string descr, Exception ex)
        {
            Type = type;
            Time = DateTime.Now;
            Unit = unit;
            Description = descr;
            Ex = ex;
        }
    }

    public class Log : INotifyPropertyChanged, IDisposable
    {
        #region Поля.
        readonly ObservableCollection<LogEventArgs> logs = new ObservableCollection<LogEventArgs>();

        int max = 300;
        #endregion

        #region Свойства.
        public ObservableCollection<LogEventArgs> Logs
        {
            get
            {
                return logs;
            }
        }

        public int Max
        {
            get { return max; }
            set
            {
                if (max == value)
                    return;

                max = value;
                CheckMax();
            }
        }
        #endregion

        public Log()
        {
        }

        public void Add(LogEventArgs log)
        {
            logs.Insert(0, log);

            CheckMax();
        }
        public void AddRange(IEnumerable<LogEventArgs> log)
        {
            foreach (var l in log)
                Add(l);

            CheckMax();
        }
        public void Remove(LogEventArgs log)
        {
            logs.Remove(log);
            DoPropertyChanged("Logs");
        }

        void CheckMax()
        {
            if (logs.Count <= max)
                return;

            var count = logs.Count - max;

            for (int i = 0; i < count; i++)
                logs.RemoveAt(logs.Count - 1);
        }

        public void Dispose()
        {
            PropertyChanged = null;
            logs.Clear();
        }

        #region События
        public event PropertyChangedEventHandler PropertyChanged;

        void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
