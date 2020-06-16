using ArgLib.Logger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectControlSystem.Managers
{
    public static class ProjectConfiguration
    {
        #region Поля
        const string unit = "ProjectConfiguration";

        static int updateTimeSec = 30, alertShowTimeMin = 5;

        public const string DateFormat = "dd/MM/yyyy";
        //static string address = @"net.tcp://localhost:20238/RLTTaskManagerService";
        static string address = @"localhost";

        static bool isShowMessageMenu = true,
            isHideAlertWin = true,
            isEnableColorsOnMainImportToExcel = false;
        #endregion

        #region Cвойства
        public static int LoadDays { get; set; }
        public static int UpdateTimeSec
        {
            get { return updateTimeSec; }
            set
            {
                if (updateTimeSec == value || value <= 0)
                    return;

                // 10 минут максимум
                if (value > 599)
                    updateTimeSec = 599;
                else
                    updateTimeSec = value;
            }
        }
        public static string Address
        {
            get { return address; }
            set
            {
                if (address == value)
                    return;

                address = value;
            }
        }

        // сколько минут висит окно оповещений об изменении проекта
        public static int AlertShowTimeMin
        {
            get { return alertShowTimeMin; }
            set
            {
                if (alertShowTimeMin == value || value <= 0)
                    return;

                alertShowTimeMin = value;
            }
        }

        // показывать поле сообщений в программе
        public static bool IsShowMessageMenu
        {
            get { return isShowMessageMenu; }
            set
            {
                if (isShowMessageMenu == value)
                    return;

                isShowMessageMenu = value;
            }
        }
        // скрывать окно оповещений
        public static bool IsHideAlertWin
        {
            get { return isHideAlertWin; }
            set
            {
                if (isHideAlertWin == value)
                    return;

                isHideAlertWin = value;
            }
        }

        // Сохранять цвета в Excel для главной страницы
        public static bool IsEnableColorsOnMainImportToExcel
        {
            get { return isEnableColorsOnMainImportToExcel; }
            set
            {
                if (isEnableColorsOnMainImportToExcel == value)
                    return;

                isEnableColorsOnMainImportToExcel = value;
            }
        }
        #endregion

        static ProjectConfiguration()
        {
            LoadDays = 30;

            Load();
        }

        public static void Load()
        {
            try
            {
                LoadDays = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "LoadDays", 30);
                Address = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "Address", address);
                UpdateTimeSec = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "UpdateTimeSec", UpdateTimeSec);
                AlertShowTimeMin = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "AlertShowTimeMin", AlertShowTimeMin);

                IsShowMessageMenu = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsShowMessageMenu", IsShowMessageMenu ? 1 : 0) == 1;
                IsHideAlertWin = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsHideAlertWin", IsHideAlertWin ? 1 : 0) == 1;

                IsEnableColorsOnMainImportToExcel = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsEnableColorsOnMainImportToExcel", IsEnableColorsOnMainImportToExcel ? 1 : 0) == 1;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка загрузки конфигурации", ex);
            }
        }
        public static void Save()
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "LoadDays", LoadDays);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "Address", Address);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "UpdateTimeSec", UpdateTimeSec);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "AlertShowTimeMin", AlertShowTimeMin);

                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsShowMessageMenu", IsShowMessageMenu ? 1 : 0);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsHideAlertWin", IsHideAlertWin ? 1 : 0);

                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Agregat\Config", "IsEnableColorsOnMainImportToExcel", IsEnableColorsOnMainImportToExcel ? 1 : 0);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка загрузки конфигурации", ex);
            }
        }

        /// <summary>Файл конфигурации</summary>
        public static string GetConfigFile(string fileName)
        {
            var dir = string.Format(@"{0}\Agregat\Config", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return string.Format(@"{0}\{1}.config", dir, fileName);
        }

        /// <summary>Возвращает флаг показывающий есть ли опция U в настройках</summary>
        public static bool HasU(string options)
        {
            if (string.IsNullOrEmpty(options))
                return false;

            var values = options.Split(',');
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v))
                    continue;

                var fValues = v.Split(' ');
                foreach (var final in fValues)
                {
                    if (string.IsNullOrEmpty(final))
                        continue;

                    if (final.Trim().ToLower() == "u")
                        return true;
                }
            }

            return false;
        }
    }
}
