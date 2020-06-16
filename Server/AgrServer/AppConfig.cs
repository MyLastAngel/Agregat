using ArgDb;
using ArgLib.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AgrServer
{
    public static class AppConfig
    {
        #region Поля
        const string unit = "AppConfig";
        #endregion

        #region Свойства
        /// <summary>Максимально количество клиентов для соединения с сервером</summary>
        public static uint MaxClients { get; set; }
        #endregion

        static AppConfig()
        {
            MaxClients = 100;

            Load();
        }

        #region Сохранение/Загрузка
        public static void Load()
        {
            var file = string.Format("{0}\\config.config", AgrStorage.AgrPath);
            if (!File.Exists(file))
            {
                LogManager.LogError(unit, string.Format("Нет файла конфигурации: '{0}' (используем значения по умолчанию)", file));
                Save();
                return;
            }

            try
            {
                var doc = XDocument.Load(file);
                if (doc.Root.Name.LocalName != "Config" || !doc.Root.HasElements)
                {
                    LogManager.LogError(unit, string.Format("Ошибка структуры файла конфигурации: '{0}' (используем значения по умолчанию)", file));
                    Save();
                    return;
                }

                foreach (XElement xElement in doc.Root.Elements())
                {
                    switch (xElement.Name.LocalName.ToLower())
                    {
                        #region MaxClients
                        case "maxclients":
                            {
                                uint v = 0;
                                if (uint.TryParse(xElement.Value, out v))
                                    MaxClients = v;

                                break;
                            }
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка загрузки структуры файла конфигурации: " + file, ex);
            }

        }
        public static void Save()
        {
            var file = string.Format("{0}\\config.config", AgrStorage.AgrPath);
            try
            {
                if (!Directory.Exists(AgrStorage.AgrPath))
                    Directory.CreateDirectory(AgrStorage.AgrPath);
                var doc = new XDocument(new XDeclaration("1.0", "KOI8-R", "yes"));
                doc.Add(new XElement(XName.Get("Config")));

                doc.Root.Add(new XElement(XName.Get("MaxClients"), MaxClients));

                doc.Save(file);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка сохранения файла конфигурации: " + file, ex);
            }

        }

        #endregion

    }
}
