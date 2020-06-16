using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectControlSystem.Managers
{
    public static class DetailsManager
    {
        #region Поля
        const string fileName = "details";

        readonly static List<string> details = new List<string>();
        #endregion

        #region Свойства
        public static List<string> Details { get { return details; } }
        #endregion

        static DetailsManager()
        {
            Load();
        }

        public static void Load()
        {
            details.Clear();

            var path = ProjectConfiguration.GetConfigFile(fileName);
            if (!File.Exists(path))
                return;

            details.AddRange(File.ReadAllLines(path));
        }
        public static void Save()
        {
            var path = ProjectConfiguration.GetConfigFile(fileName);
            File.WriteAllLines(path, details.ToArray());
        }
        public static void Add(string d)
        {
            if (string.IsNullOrEmpty(d))
                return;

            if (!details.Any(x => x.ToLower() == d.ToLower()))
            {
                details.Add(d);
                Save();
            }
        }
    }
}
