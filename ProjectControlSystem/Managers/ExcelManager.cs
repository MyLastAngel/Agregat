using ArgDb;
using ArgLib.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProjectControlSystem.Managers
{
    public static class ExcelManager
    {
        #region Поля
        const string unit = "ExcelManager";

        static readonly Color cWekend = Color.FromArgb(255, 255, 193, 193);

        // Структура для чтения Request из Excel 
        static readonly Dictionary<string, RequestExcelType> eRequestStructureNames = new Dictionary<string, RequestExcelType>();

        // поток сохранения в Excel
        static readonly BackgroundWorker workerToExcel = new BackgroundWorker { WorkerReportsProgress = true },
                                         workerProjectToExcel = new BackgroundWorker { WorkerReportsProgress = true };

        // цвето стопа
        static Color cStop = Color.FromArgb(255, 0, 50, 90);
        #endregion

        #region Свойства
        /// <summary>Флаг занятости менеджера</summary>
        public static bool IsBusy { get { return workerToExcel.IsBusy; } }
        #endregion

        static ExcelManager()
        {
            eRequestStructureNames["№пп"] = RequestExcelType.Index;
            eRequestStructureNames["Артикул"] = RequestExcelType.Article;
            eRequestStructureNames["Необходимо приобрести"] = RequestExcelType.Name;
            eRequestStructureNames["Кол-во"] = RequestExcelType.Count;
            eRequestStructureNames["Срок"] = RequestExcelType.DateCompletePlan;

            workerToExcel.ProgressChanged += workerToExcel_ProgressChanged;
            workerToExcel.RunWorkerCompleted += workerToExcel_RunWorkerCompleted;
            workerToExcel.DoWork += workerToExcel_DoWork;

            workerProjectToExcel.ProgressChanged += workerProjectToExcel_ProgressChanged;
            workerProjectToExcel.DoWork += workerProjectToExcel_DoWork;
            workerProjectToExcel.RunWorkerCompleted += workerToExcel_RunWorkerCompleted;
        }

        // Импорт в EXCEL
        public static void ToExcel(ProjectViewMode mode)
        {
            if (workerToExcel.IsBusy)
                return;

            workerToExcel.RunWorkerAsync(mode);
        }
        static void ToExcelCore(ProjectViewMode mode)
        {
            if (ProjectManager.FiltredProjects.Count == 0)
                return;

            var app = new Excel.ApplicationClass();
            var books = app.Workbooks.Add(Type.Missing);
            var ws = (Excel.Worksheet)books.ActiveSheet;

            try
            {
                switch (mode)
                {
                    #region Main
                    case ProjectViewMode.Main:
                        {
                            ToExcelMain(ws);
                            break;
                        }
                    #endregion
                    #region Commercial
                    case ProjectViewMode.Commercial:
                        {
                            ToExcelCommercial(ws);
                            break;
                        }
                    #endregion
                    #region ITO
                    case ProjectViewMode.ITO:
                        {
                            ToExcelITO(ws);
                            break;
                        }
                    #endregion
                    #region Warehouse
                    case ProjectViewMode.Warehouse:
                        {
                            ToExcelWarehouse(ws);
                            break;
                        }
                    #endregion
                    #region OMTS
                    case ProjectViewMode.OMTS:
                        {
                            ToExcelOMTS(ws);
                            break;
                        }
                    #endregion
                    #region Manufacture
                    case ProjectViewMode.Manufacture:
                        {
                            ToExcelManufacture(ws);
                            break;
                        }
                    #endregion
                    #region OTK
                    case ProjectViewMode.OTK:
                        {
                            ToExcelOTK(ws);
                            break;
                        }
                    #endregion
                    default:
                        return;
                }

                // Сохраняем
                app.Visible = true;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Импорта недокомплекта в Excel"), ex);
            }
            finally
            {
                ReleaseComObject(ws);
                ReleaseComObject(books);
                ReleaseComObject(app);

                GC.Collect();
            }
        }



        /// <summary>Импорт в Excel отдельного проекта</summary>
        public static void ToExcel(Project p)
        {
            if (workerProjectToExcel.IsBusy)
                return;

            workerProjectToExcel.RunWorkerAsync(p);
        }
        static void ToExcelCore(Project p)
        {
            var app = new Excel.ApplicationClass();
            var books = app.Workbooks.Add(Type.Missing);
            var ws = (Excel.Worksheet)books.ActiveSheet;

            try
            {
                int row = 1, column = 0;

                var structure = new List<string>
                {
                    "Планирование задания",
                    "Обьект", "Изделие", "Условия","Прочие условия", "Дата заполнения","Ответственный", 
                    "Месяц","День",
                    "ИТО Гидравлика","ОМТС Гидравлика", 
                    "ИТО Электрика",  "ОМТС Электрика",
                    "ИТО Рама",                    
                    "Производство",
                    "ОТК Гидравлика", "ОТК Электрика", 
                    "Дата отгрузки"
                };

                var sz = new Dictionary<string, string>();
                ToExcelHeader(ws, structure, out sz, row, false);

                var range = ((Excel.Range)ws.UsedRange);
                range.ColumnWidth = 30;

                var dTime = new Dictionary<string, KeyValuePair<DateTime?, DateTime?>>();
                if (!p.Is_ITO_G_NotNeed)
                    dTime["ИТО Гидравлика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_ITO_G_Plan, p.Time_ITO_G_Actual);
                if (!p.Is_ITO_E_NotNeed)
                    dTime["ИТО Электрика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_ITO_E_Plan, p.Time_ITO_E_Actual);
                if (!p.Is_ITO_R_NotNeed)
                    dTime["ИТО Рама"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_ITO_R_Plan, p.Time_ITO_R_Actual);

                if (!p.Is_OTK_G_NotNeed)
                    dTime["ОМТС Гидравлика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_OMTS_G_Plan, p.Time_OMTS_G_Actual);
                if (!p.Is_OTK_E_NotNeed)
                    dTime["ОМТС Электрика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_OMTS_E_Plan, p.Time_OMTS_E_Actual);

                dTime["Производство"] = new KeyValuePair<DateTime?, DateTime?>(p.MF_Time_Plan, p.MF_Time);

                if (!p.Is_OTK_G_NotNeed)
                    dTime["ОТК Гидравлика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_OTK_Plan, p.Time_OTK_G_Actual);
                if (!p.Is_OTK_E_NotNeed)
                    dTime["ОТК Электрика"] = new KeyValuePair<DateTime?, DateTime?>(p.Time_OTK_Plan, p.Time_OTK_E_Actual);

                dTime["Дата отгрузки"] = new KeyValuePair<DateTime?, DateTime?>(p.TimeEndPlaned, p.TimeEndActual);

                DateTime sTime = DateTime.MinValue, eTime = sTime;
                foreach (var kv in dTime)
                {
                    var t = kv.Value.Key;
                    if (t.HasValue)
                    {
                        if (sTime > t.Value || sTime == DateTime.MinValue)
                            sTime = t.Value;
                        if (eTime < t.Value || eTime == DateTime.MinValue)
                            eTime = t.Value;
                    }

                    t = kv.Value.Value;
                    if (t.HasValue)
                    {
                        if (sTime > t.Value || sTime == DateTime.MinValue)
                            sTime = t.Value;
                        if (eTime < t.Value || eTime == DateTime.MinValue)
                            eTime = t.Value;
                    }
                }

                // проверяем время на правильность ввода
                if (eTime == DateTime.MinValue || sTime == DateTime.MinValue)
                {
                    LogManager.LogError(unit, "Ошибка получения времени");
                    return;
                }

                range = null;

                // добавляем отступ 3 дня по краям
                sTime = sTime.AddDays(-3); eTime = eTime.AddDays(3);
                DateTime tmp = sTime.Date, prev = tmp;

                //188 210 238 серый
                //60 179 113 зеленый
                //222 184 135 выхи

                column = 3;
                for (int sColumn = 3; tmp <= eTime.Date; tmp = tmp.AddDays(1), column++)
                {
                    range = ((Excel.Range)ws.Cells[9, column]);
                    range.ColumnWidth = 5;
                    range.BorderAround();

                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = tmp.Day;

                    // если у нас выходные
                    if (tmp.DayOfWeek == DayOfWeek.Saturday || tmp.DayOfWeek == DayOfWeek.Sunday)
                    {
                        range = (Excel.Range)ws.get_Range(GetChar(column - 1) + "9", GetChar(column - 1) + structure.Count.ToString());
                        range.Interior.Color = (135 << 16) + (184 << 8) + 222;
                    }

                    // если ищем совпадения по датам
                    foreach (var kv in dTime)
                    {
                        row = structure.IndexOf(kv.Key) + 1;

                        var t = kv.Value.Key;
                        if (t.HasValue && t.Value.Date == tmp)
                        {
                            range = ((Excel.Range)ws.Cells[row, column]);
                            //range.Value2 = kv.Key;
                            range.Interior.Color = (113 << 16) + (179 << 8) + 60;
                        }

                        t = kv.Value.Value;
                        if (t.HasValue && t.Value.Date == tmp)
                        {
                            range = ((Excel.Range)ws.Cells[row, column]);
                            //range.Value2 = kv.Key;
                            range.Interior.Color = (238 << 16) + (210 << 8) + 188;

                        }
                    }

                    // делаем бордюр 
                    // вертикальный
                    range = (Excel.Range)ws.get_Range(GetChar(column - 1) + "9", GetChar(column - 1) + structure.Count.ToString());
                    range.BorderAround();

                    // Пишем месяц (если переход или предпоследняя запись)
                    if (prev.Month != tmp.Month || tmp == eTime.Date)
                    {
                        if (tmp == eTime.Date)
                            column++;

                        range = (Excel.Range)ws.get_Range(GetChar(sColumn - 1) + "8", GetChar(column - 2) + "8");
                        range.Merge();
                        range.BorderAround();
                        range.Font.Bold = true;
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        range.Value2 = prev.ToString("MMMM");

                        sColumn = column;

                        if (tmp == eTime.Date)
                            column--;
                    }

                    prev = tmp;
                }

                // горизонтальный
                for (row = 9; row <= structure.Count; row++)
                {
                    range = (Excel.Range)ws.get_Range(GetChar(2) + row.ToString(), GetChar(column - 2) + row.ToString());
                    range.BorderAround();
                }

                // заголовок значения
                for (row = 0; row < structure.Count; row++)
                {
                    var isStop = false;
                    object value = null;
                    string format = "";

                    switch (structure[row])
                    {
                        #region Планирование задания
                        case "Планирование задания":
                            {
                                range = (Excel.Range)ws.get_Range(GetChar(1) + (row + 1).ToString(), GetChar(column - 2) + (row + 1).ToString());
                                range.Merge();
                                range.BorderAround();
                                if (!string.IsNullOrEmpty(format))
                                    range.NumberFormat = format;

                                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                range.Value2 = structure[row];

                                continue;
                            }
                        #endregion
                        #region Обьект
                        case "Обьект":
                            {
                                value = p.CustomerName;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = p.Product;
                                break;
                            }
                        #endregion
                        #region Условия
                        case "Условия":
                            {
                                value = p.Options;
                                break;
                            }
                        #endregion
                        #region Дата заполнения
                        case "Дата заполнения":
                            {
                                format = "ДД.ММ.ГГГГ";
                                value = DateTime.Now.Date;
                                break;
                            }
                        #endregion
                        case "Месяц":
                            isStop = true;
                            break;
                    }

                    if (isStop)
                        break;

                    range = (Excel.Range)ws.get_Range(GetChar(2) + (row + 1).ToString(), GetChar(column - 2) + (row + 1).ToString());
                    range.Merge();
                    range.BorderAround();
                    if (!string.IsNullOrEmpty(format))
                        range.NumberFormat = format;

                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    range.Value2 = value;
                }

                range = ((Excel.Range)ws.UsedRange);
                range.Font.Size = 16;
                range.Font.Name = "Calibri";

                // Сохраняем
                app.Visible = true;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка импорта проекта: '{0}' в Excel", p.ID), ex);
            }
            finally
            {
                ReleaseComObject(ws);
                ReleaseComObject(books);
                ReleaseComObject(app);

                GC.Collect();
            }
        }

        static void SetBorder(Excel.Range range)
        {
            range.BorderAround();

            //range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 1d;
            //range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 1d;
            //range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 1d;
            //range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 1d;

            //range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlDouble;
            //range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlDouble;
            //range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;
            //range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
        }

        #region Для каждого окна
        // Создаем заголовок
        public static void ToExcelHeader(Excel.Worksheet ws, List<string> structure, out Dictionary<string, string> size, int statRow = 9, bool isHorizontal = true)
        {
            var column = 2;

            size = new Dictionary<string, string>();

            foreach (var v in structure)
            {
                var range = ((Excel.Range)ws.Cells[statRow, column]);

                if (isHorizontal)
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                else
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                range.Font.Bold = true;
                range.Value2 = v;

                if (v == "Опции")
                    range.ColumnWidth = 50;

                range.BorderAround();

                //range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 1d;
                //range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 1d;
                //range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 1d;
                //range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 1d;

                if (isHorizontal)
                {
                    range.WrapText = true;
                    range.RowHeight = 30;
                }

                if (!size.ContainsKey(v) || size[v].Length < v.Length)
                {
                    size[v] = v;
                    range.Columns.AutoFit();
                }

                if (isHorizontal)
                    column++;
                else
                    statRow++;
            }
        }

        // Главное окно
        public static void ToExcelMain(Excel.Worksheet ws)
        {
            ws.Name = "Общая информация";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции","Коммерческий отдел",
            "ИТО Гидравлика","ИТО Электрика","ИТО Рама",
            "Склад Гидравлика","Склад Электрика", "Склад Рама",
            "ОМТС Гидравлика", "ОМТС Электрика",
            "Производство", "ОТК", "Долг Гидравлика", "Долг Электрика",
            "Дата отгрузки план", "Дата отгрузки факт"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            // Создаем заголовок
            ToExcelHeader(ws, structure, out size, 2);

            bool isUseColor = true;

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[3 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                value = project.Options;
                                break;
                            }
                        #endregion

                        #region Коммерческий отдел
                        case "Коммерческий отдел":
                            {
                                value = project.IsCommercialDepartamentOk ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.IsCommercialDepartamentOk)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ИТО Гидравлика
                        case "ИТО Гидравлика":
                            {
                                value = project.Is_ITO_G_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_ITO_G_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ИТО Электрика
                        case "ИТО Электрика":
                            {
                                value = project.Is_ITO_E_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_ITO_E_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ИТО Рама
                        case "ИТО Рама":
                            {
                                value = project.Is_ITO_R_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_ITO_R_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Склад Гидравлика
                        case "Склад Гидравлика":
                            {
                                value = project.Is_WH_G_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_WH_G_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Склад Электрика
                        case "Склад Электрика":
                            {
                                value = project.Is_WH_E_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_WH_E_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Склад Рама
                        case "Склад Рама":
                            {
                                value = project.Is_WH_R_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_WH_R_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ОМТС Гидравлика
                        case "ОМТС Гидравлика":
                            {
                                value = project.Is_OMTS_G_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_OMTS_G_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ОМТС Электрика
                        case "ОМТС Электрика":
                            {
                                value = project.Is_OMTS_E_Ok ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.Is_OMTS_E_Ok)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Производство
                        case "Производство":
                            {
                                value = project.IsManufactureDepartamentOk ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.IsManufactureDepartamentOk)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region ОТК
                        case "ОТК":
                            {
                                value = project.IsOTKDepartamentOk ? "+" : "-";

                                if (isUseColor)
                                {
                                    if (project.IsOTKDepartamentOk)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Долг Гидравлика
                        case "Долг Гидравлика":
                            {
                                if (!project.IsDebt_G_None.HasValue)
                                    break;

                                value = project.IsDebt_G_None.Value ? "Нет" : "Да";

                                if (isUseColor)
                                {
                                    if (project.IsDebt_G_None.Value)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Долг Электрика
                        case "Долг Электрика":
                            {
                                if (!project.IsDebt_E_None.HasValue)
                                    break;

                                value = project.IsDebt_E_None.Value ? "Нет" : "Да";

                                if (isUseColor)
                                {
                                    if (project.IsDebt_E_None.Value)
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                    else
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                }

                                break;
                            }
                        #endregion
                        #region Дата отгрузки план
                        case "Дата отгрузки план":
                            {
                                value = project.TimeEndPlaned;
                                break;
                            }
                        #endregion
                        #region Дата отгрузки факт
                        case "Дата отгрузки факт":
                            {
                                if (project.TimeEndActual.HasValue)
                                    value = project.TimeEndActual;

                                break;
                            }
                        #endregion

                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }
                }

                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }

        // Коммерческий отдел
        public static void ToExcelCommercial(Excel.Worksheet ws)
        {
            ws.Name = "Коммерческий отдел";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции","Запуск в производство",
            "Дата готовности(план)","Планируемая дата отгрузки","Дата переноса",
            "Электрика (факт)","Гидравлика (факт)","Упаковка"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            // Создаем заголовок
            ToExcelHeader(ws, structure, out size);

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                value = project.Options;
                                break;
                            }
                        #endregion
                        #region Запуск в производство
                        case "Запуск в производство":
                            {
                                value = project.TimeBegin;
                                break;
                            }
                        #endregion
                        #region Дата готовности(план)
                        case "Дата готовности(план)":
                            {
                                value = project.MF_Time_Plan;
                                break;
                            }
                        #endregion
                        #region Планируемая дата отгрузки
                        case "Планируемая дата отгрузки":
                            {
                                value = project.TimeEndPlaned;
                                break;
                            }
                        #endregion
                        #region Дата переноса
                        case "Дата переноса":
                            {
                                value = project.Com_New_Time;
                                break;
                            }
                        #endregion
                        #region Электрика (факт)
                        case "Электрика (факт)":
                            {
                                value = project.Time_WH_E_Actual;
                                break;
                            }
                        #endregion
                        #region Гидравлика (факт)
                        case "Гидравлика (факт)":
                            {
                                value = project.Time_WH_G_Actual;
                                break;
                            }
                        #endregion
                        #region Упаковка
                        case "Упаковка":
                            {
                                value = project.COM_Package_Type;
                                break;
                            }
                        #endregion

                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }
                }

                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }
        // Отдел ИТО
        public static void ToExcelITO(Excel.Worksheet ws)
        {
            ws.Name = "Инженерный отдел";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции",
            "Гидравлические схемы (план)","Гидравлические схемы (факт)",
            "Электрические схемы (план)", "Электрические схемы (факт)",
            "Рама (план)", "Тип рамы","Рама (факт)"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            ToExcelHeader(ws, structure, out size);

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                range.ColumnWidth = 30;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                if (!project.Is_ITO_E_Ok || !project.Is_ITO_G_Ok || !project.Is_ITO_R_Ok)
                                    value = project.Options;

                                range.ColumnWidth = 35;

                                break;
                            }
                        #endregion
                        #region Гидравлические схемы (план)
                        case "Гидравлические схемы (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_ITO_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_G_Plan;

                                if (project.Is_ITO_G_NotNeed == true || project.Time_ITO_G_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        #region Гидравлические схемы (факт)
                        case "Гидравлические схемы (факт)":
                            {
                                if (project.Is_ITO_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_G_Actual;

                                if (project.Is_ITO_G_NotNeed == true || project.Time_ITO_G_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        #region Электрические схемы (план)
                        case "Электрические схемы (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_ITO_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_E_Plan;

                                if (project.Is_ITO_E_NotNeed == true || project.Time_ITO_E_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        #region Электрические схемы (факт)
                        case "Электрические схемы (факт)":
                            {
                                if (project.Is_ITO_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_E_Actual;

                                if (project.Is_ITO_E_NotNeed == true || project.Time_ITO_E_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        #region Рама (план)
                        case "Рама (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_ITO_R_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_R_Plan;

                                if (project.Is_ITO_R_NotNeed == true || project.Time_ITO_R_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        #region Тип рамы
                        case "Тип рамы":
                            {
                                value = project.ITO_R_Mode;
                                range.ColumnWidth = 15;
                                break;
                            }
                        #endregion
                        #region Рама (факт)
                        case "Рама (факт)":
                            {
                                if (project.Is_ITO_R_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_ITO_R_Actual;

                                if (project.Is_ITO_R_NotNeed == true || project.Time_ITO_R_Actual.HasValue)
                                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                                range.ColumnWidth = 15;

                                break;
                            }
                        #endregion
                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }
                }

                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }
        // Cклад
        public static void ToExcelWarehouse(Excel.Worksheet ws)
        {
            ws.Name = "Склад";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции",
            "Гидравлика (план)", "Гидравлика (факт)",
            "Электрика (план)", "Электрика (факт)",
            "Рама (план)", "Рама (факт)", "Планируемая дата отгрузки", "Ориентировочная дата готовности", "Фактическая дата отгрузки", "Упаковка"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            ToExcelHeader(ws, structure, out size);

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                value = project.Options;
                                break;
                            }
                        #endregion

                        #region Гидравлика (план)
                        case "Гидравлика (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_WH_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_G_Plan;

                                break;
                            }
                        #endregion
                        #region Гидравлика (факт)
                        case "Гидравлика (факт)":
                            {
                                if (project.Is_WH_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_G_Actual;

                                break;
                            }
                        #endregion
                        #region Электрика (план)
                        case "Электрика (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_WH_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_E_Plan;

                                break;
                            }
                        #endregion
                        #region Электрика (факт)
                        case "Электрика (факт)":
                            {
                                if (project.Is_WH_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_E_Actual;

                                break;
                            }
                        #endregion
                        #region Рама (план)
                        case "Рама (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_WH_R_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_R_Plan;

                                break;
                            }
                        #endregion
                        #region Рама (факт)
                        case "Рама (факт)":
                            {
                                if (project.Is_WH_R_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_WH_R_Actual;

                                break;
                            }
                        #endregion
                        #region Планируемая дата отгрузки
                        case "Планируемая дата отгрузки":
                            {
                                value = project.TimeEndPlaned;
                                break;
                            }
                        #endregion
                        #region Ориентировочная дата готовности
                        case "Ориентировочная дата готовности":
                            {
                                value = project.MF_Time_Plan;
                                break;
                            }
                        #endregion

                        #region Фактическая дата отгрузки
                        case "Фактическая дата отгрузки":
                            {
                                value = project.TimeEndActual;
                                break;
                            }
                        #endregion
                        #region Упаковка
                        case "Упаковка":
                            {
                                value = project.COM_Package_Type;
                                break;
                            }
                        #endregion
                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }
                }

                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }
        // ОМТС
        public static void ToExcelOMTS(Excel.Worksheet ws)
        {
            ws.Name = "ОМТС";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции",
            "Гидравлика (план)", "Гидравлика (факт)", 
            "Электрика (план)", "Электрика (факт)"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            ToExcelHeader(ws, structure, out size);

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                value = project.Options;
                                break;
                            }
                        #endregion

                        #region Гидравлика (план)
                        case "Гидравлика (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_WH_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OMTS_G_Plan;

                                break;
                            }
                        #endregion
                        #region Гидравлика (факт)
                        case "Гидравлика (факт)":
                            {
                                if (project.Is_WH_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OMTS_G_Actual;

                                break;
                            }
                        #endregion
                        #region Электрика (план)
                        case "Электрика (план)":
                            {
                                if (project.IsManagerSetPlanDate && project.Is_WH_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OMTS_E_Plan;

                                break;
                            }
                        #endregion
                        #region Электрика (факт)
                        case "Электрика (факт)":
                            {
                                if (project.Is_WH_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OMTS_E_Actual;

                                break;
                            }
                        #endregion
                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }
                }

                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }
        // производство
        public static void ToExcelManufacture_old(Excel.Worksheet ws)
        {
            //ws.Name = "ОМТС";

            //var structure = new List<string>
            //{
            //"Номер","Контрагент","Изделие","Опции",
            //"Участок", "Тип рамы", "Рама", "Номер поста",
            //"Расключение агрегата", "Изготовление ШУ",  "ШУ",
            //"Планируемая дата отгрузки", "Ориентировочная дата готовности", 
            //"Дата постановки на тест (план)","Дата постановки на тест (факт)"
            //};

            //var size = new Dictionary<string, string>();
            //var column = 0;

            //ToExcelHeader(ws, structure, out size);

            //// Сохраняем тело
            //for (int row = 0; row < ProjectManager.FiltredProjects.Count; row++)
            //{
            //    var project = ProjectManager.FiltredProjects[row];

            //    column = 0;
            //    foreach (var v in structure)
            //    {
            //        var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
            //        column++;

            //        object value = null;

            //        switch (v)
            //        {
            //            #region Номер
            //            case "Номер":
            //                {
            //                    value = project.ID;

            //                    switch (project.State)
            //                    {
            //                        case ProjectState.Ok:
            //                            range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
            //                            break;
            //                        case ProjectState.Warning:
            //                            range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
            //                            break;
            //                        case ProjectState.Error:
            //                            range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
            //                            break;
            //                    }

            //                    break;
            //                }
            //            #endregion
            //            #region Контрагент
            //            case "Контрагент":
            //                {
            //                    value = project.Customer;

            //                    switch (project.State)
            //                    {
            //                        case ProjectState.Ok:
            //                            range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
            //                            break;
            //                        case ProjectState.Warning:
            //                            range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
            //                            break;
            //                        case ProjectState.Error:
            //                            range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
            //                            break;
            //                    }

            //                    break;
            //                }
            //            #endregion
            //            #region Изделие
            //            case "Изделие":
            //                {
            //                    value = project.Product;

            //                    switch (project.State)
            //                    {
            //                        case ProjectState.Ok:
            //                            range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
            //                            break;
            //                        case ProjectState.Warning:
            //                            range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
            //                            break;
            //                        case ProjectState.Error:
            //                            range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
            //                            break;
            //                    }

            //                    break;
            //                }
            //            #endregion
            //            #region Опции
            //            case "Опции":
            //                {
            //                    value = project.Options;

            //                    switch (project.State)
            //                    {
            //                        case ProjectState.Ok:
            //                            range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
            //                            break;
            //                        case ProjectState.Warning:
            //                            range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
            //                            break;
            //                        case ProjectState.Error:
            //                            range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
            //                            break;
            //                    }

            //                    break;
            //                }
            //            #endregion

            //            #region Участок
            //            case "Участок":
            //                {
            //                    value = project.MF_Level;

            //                    // Если стоит Участок + Пост красим участок в цвет проекта
            //                    if (!string.IsNullOrEmpty(project.MF_Level) && !string.IsNullOrEmpty(project.MF_Post))
            //                    {
            //                        switch (project.State)
            //                        {
            //                            case ProjectState.Ok:
            //                                range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
            //                                break;
            //                            case ProjectState.Warning:
            //                                range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
            //                                break;
            //                            case ProjectState.Error:
            //                                range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
            //                                break;
            //                        }
            //                    }

            //                    break;
            //                }
            //            #endregion
            //            #region Тип рамы
            //            case "Тип рамы":
            //                {
            //                    value = project.ITO_R_Mode;
            //                    break;
            //                }
            //            #endregion
            //            #region Рама
            //            case "Рама":
            //                {
            //                    value = project.MF_Rama;

            //                    if (!string.IsNullOrEmpty(project.MF_Rama) && project.MF_Rama == Project.complete)
            //                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

            //                    break;
            //                }
            //            #endregion
            //            #region Номер поста
            //            case "Номер поста":
            //                {
            //                    value = project.MF_Post;
            //                    break;
            //                }
            //            #endregion

            //            #region Расключение агрегата
            //            case "Расключение агрегата":
            //                {
            //                    value = project.MF_Agregat;

            //                    if (!string.IsNullOrEmpty(project.MF_Agregat) && project.MF_Agregat == Project.complete)
            //                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

            //                    break;
            //                }
            //            #endregion
            //            #region Изготовление ШУ
            //            case "Изготовление ШУ":
            //                {
            //                    value = project.MF_SH_Place;
            //                    break;
            //                }
            //            #endregion
            //            #region ШУ
            //            case "ШУ":
            //                {
            //                    value = project.MF_SH;

            //                    if (!string.IsNullOrEmpty(project.MF_SH) && project.MF_SH == Project.complete)
            //                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

            //                    break;
            //                }
            //            #endregion

            //            #region Планируемая дата отгрузки
            //            case "Планируемая дата отгрузки":
            //                {
            //                    if (project.IsManufactureNotEnoughTime)
            //                        range.Font.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;

            //                    value = project.TimeEndPlaned;
            //                    break;
            //                }
            //            #endregion
            //            #region Ориентировочная дата готовности
            //            case "Ориентировочная дата готовности":
            //                {
            //                    value = project.MF_Time_Plan;
            //                    break;
            //                }
            //            #endregion

            //            #region Дата передачи на ОТК (план)
            //            case "Дата передачи на ОТК (план)":
            //                {
            //                    value = project.Time_OTK_Plan;
            //                    break;
            //                }
            //            #endregion
            //            #region Дата постановки на тест (факт)
            //            case "Дата постановки на тест (факт)":
            //                {
            //                    value = project.MF_Time_Test_Actual;
            //                    break;
            //                }
            //            #endregion

            //            default:
            //                LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
            //                break;
            //        }

            //        if (value == null)
            //            continue;

            //        if (value is DateTime)
            //            range.NumberFormat = "ДД.ММ.ГГГГ";

            //        // Установка значения
            //        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        range.Value2 = value;

            //        if (!string.IsNullOrEmpty(value.ToString()))
            //        {
            //            if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
            //            {
            //                size[v] = value.ToString();
            //                range.Columns.AutoFit();
            //            }
            //        }
            //    }
            //}
        }
        // производство v2
        public static void ToExcelManufacture(Excel.Worksheet ws)
        {
            ws.Name = "Производство";

            var cIndex = GetChar(0);
            var range = (Excel.Range)ws.get_Range(cIndex + "1", cIndex + "1");
            CreateRequestHeader(ws, DateTime.Now.ToString("dd MMMM yyyy года HH:mm"), "", "A1", GetChar(13) + "1", false);
            range.RowHeight = 30;
            range.Font.Name = "Comic Sans MS";
            range.Font.Bold = true;
            range.Font.Size = 15;

            // Заголовок
            cIndex = GetChar(0);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            range.RowHeight = 30;
            range.Font.Bold = true;

            #region Заголовок
            // Номер
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Номер", 10, true);
            // Заказчик
            cIndex = GetChar(1);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Контрагент", 20, true);
            // Изделие
            cIndex = GetChar(2);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Изделие", 20, true);
            // Тип рамы
            cIndex = GetChar(3);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Тип рамы", 15, true);
            // Рама
            cIndex = GetChar(4);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Рама", 15, true);
            // Коллектор
            cIndex = GetChar(5);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Коллектор", 15, true);
            // Номер поста
            cIndex = GetChar(6);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Номер поста", 15, true);
            // Прогресс
            cIndex = GetChar(7);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Прогресс", 15, true);

            // Расключение агрегата
            cIndex = GetChar(8);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Расключение агрегата", 15, true);
            // Дата испытаний по гидравлике
            cIndex = GetChar(9);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Дата испытаний по гидравлике", 15, true);
            // ШУ
            cIndex = GetChar(10);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "ШУ", 15, true);
            // Изготовл. ШУ
            cIndex = GetChar(11);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Изготовл. ШУ", 15, true);
            // Планируемая дата отгрузки
            cIndex = GetChar(12);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Планируемая дата отгрузки", 15, true);
            // Ориентировочная дата готовности
            cIndex = GetChar(13);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Ориентировочная дата готовности", 15, true);
            // Дифицит
            cIndex = GetChar(14);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Дифицит", 5, true);
            range.RowHeight = 50;
            // Упаковка
            cIndex = GetChar(15);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Упаковка", 15, true);
            // Примечание
            cIndex = GetChar(16);
            range = (Excel.Range)ws.get_Range(cIndex + "2", cIndex + "2");
            CreateManufactureCell(ws, range, "Примечание", 15, true);
            range.RowHeight = 70;
            #endregion
            //
            // Создаем поля
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var color = Colors.White;

                var p = ProjectManager.FiltredProjects[row];
                if (!string.IsNullOrEmpty(p.MF_Level)) // Если нет участка то не красить цветом
                {
                    switch (p.State)
                    {
                        case ProjectState.Error:
                        case ProjectState.Warning:
                        case ProjectState.Ok:
                            color = Colors.LightGreen;
                            break;
                        case ProjectState.Stop:
                            color = cStop;
                            break;

                        //case ProjectState.Ok:
                        //    color = Colors.LightGreen;
                        //    break;
                        //case ProjectState.Warning:
                        //    color = Colors.LightGoldenrodYellow;
                        //    break;
                        //case ProjectState.Error:
                        //    color = Colors.Red;
                        //    break;
                    }
                }
                var isHasUOption = ProjectConfiguration.HasU(p.Options);

                // Номер
                cIndex = GetChar(0);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.ID, 10, true, false);
                range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;
                range.Font.Bold = true;
                range.Font.Size = 20;
                if (isHasUOption)
                    range.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleSingle;

                // Контрагент
                cIndex = GetChar(1);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.Customer, 10, true, false);
                range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;

                // Изделие
                cIndex = GetChar(2);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                var value = p.IsStop ? string.Format("СТОП! {0}", p.Product) : p.Product;
                CreateManufactureCell(ws, range, value, 20, true, false);
                range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;

                // Тип рамы
                cIndex = GetChar(3);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.ITO_R_Mode, 10, true, false);
                if (!string.IsNullOrEmpty(p.ITO_R_Mode))
                {
                    switch (p.ITO_R_Mode)
                    {
                        case "Сборная":
                            range.Interior.Color = (Colors.DarkGray.B << 16) + (Colors.DarkGray.G << 8) + Colors.DarkGray.R;
                            range.Font.Color = (Colors.White.B << 16) + (Colors.White.G << 8) + Colors.White.R;
                            break;
                        case "Сварная":
                            range.Interior.Color = (Colors.White.B << 16) + (Colors.White.G << 8) + Colors.White.R;
                            range.Font.Color = (Colors.Black.B << 16) + (Colors.Black.G << 8) + Colors.Black.R;
                            break;
                        default:
                            range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;
                            break;
                    }
                }

                // Рама
                cIndex = GetChar(4);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_Rama, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_Rama) && p.MF_Rama == Project.complete)
                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                // Коллектор
                cIndex = GetChar(5);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_Collector, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_Collector) && p.MF_Collector == Project.complete)
                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                // Номер поста
                cIndex = GetChar(6);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_Post, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_Post))
                    range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;

                cIndex = GetChar(7);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_Complete_Percentage.ToString(), 10, true, false);

                // Расключение агрегата
                cIndex = GetChar(8);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_Agregat, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_Agregat) && p.MF_Agregat == Project.complete)
                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                // Дата испытаний по гидравлике
                cIndex = GetChar(9);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, (p.Time_OTK_G_Actual.HasValue ? p.Time_OTK_G_Actual.Value.ToString("dd.MM.yyyy") : ""), 15, true, false);
                if (p.Time_OTK_G_Actual.HasValue)
                    range.Interior.Color = (color.B << 16) + (color.G << 8) + color.R;

                // ШУ
                cIndex = GetChar(10);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_SH, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_SH) && p.MF_SH == Project.complete)
                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                // Изготовл. ШУ
                cIndex = GetChar(11);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.MF_SH_Place, 10, true, false);
                if (!string.IsNullOrEmpty(p.MF_SH_Place))
                    range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;

                // Планируемая дата отгрузки
                cIndex = GetChar(12);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.TimeEndPlaned.ToString("dd.MM.yyyy"), 15, true, false);

                // Ориентировочная дата готовности
                cIndex = GetChar(13);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                if (p.IsManufactureNotEnoughTime)
                    range.Font.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                CreateManufactureCell(ws, range, (p.MF_Time_Plan.HasValue ? p.MF_Time_Plan.Value.ToString("dd.MM.yyyy") : ""), 15, true, true);

                // Дифицит
                cIndex = GetChar(14);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                value = "";
                if (p.IsDebt_E_None == false)
                    value += " Э ";
                if (p.IsDebt_G_None == false)
                    value += " Г ";

                CreateManufactureCell(ws, range, value, 5, true, false);

                //Упаковка
                cIndex = GetChar(15);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, p.COM_Package_Type, 20, true, false);

                // Примечание
                cIndex = GetChar(16);
                range = (Excel.Range)ws.get_Range(cIndex + (row + 3).ToString(), cIndex + (row + 3).ToString());
                CreateManufactureCell(ws, range, "", 20, true, false);


                // прогресс
                workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
            }
        }
        /// <summary>Создание заголовка для недокомплекта</summary>
        static void CreateManufactureCell(Excel.Worksheet ws, Excel.Range range, string value, int columnWidth, bool isUseHeaderBorder = false, bool isBold = true)
        {
            range.Merge();
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

            if (isUseHeaderBorder)
            {
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            }
            range.Font.Name = "Comic Sans MS";
            range.Font.Bold = isBold;
            range.WrapText = true;
            range.Value2 = value;
            range.RowHeight = 30;
            //range.Rows.AutoFit();

            range.ColumnWidth = columnWidth;
        }

        // ОТК
        public static void ToExcelOTK(Excel.Worksheet ws)
        {
            ws.Name = "ОТК";

            var structure = new List<string>
            {
            "Номер","Контрагент","Изделие","Опции",
            "Дата передачи на ОТК (план)", "Дата передачи на ОТК (факт)",
            "Дата испытаний по гидравлике", "Дата испытаний шкафа", "Дата отгрузки (план)",
            "Дата отгрузки (факт)", "Долги по комплектации"
            };

            var size = new Dictionary<string, string>();
            var column = 0;

            ToExcelHeader(ws, structure, out size);

            // Сохраняем тело
            var count = ProjectManager.FiltredProjects.Count;
            for (int row = 0; row < count; row++)
            {
                var project = ProjectManager.FiltredProjects[row];

                column = 0;
                foreach (var v in structure)
                {
                    var range = ((Excel.Range)ws.Cells[10 + row, 2 + column]);
                    column++;

                    object value = null;

                    switch (v)
                    {
                        #region Номер
                        case "Номер":
                            {
                                value = project.ID;

                                switch (project.State)
                                {
                                    case ProjectState.Ok:
                                        range.Interior.Color = (Colors.LightGreen.B << 16) + (Colors.LightGreen.G << 8) + Colors.LightGreen.R;
                                        break;
                                    case ProjectState.Warning:
                                        range.Interior.Color = (Colors.LightGoldenrodYellow.B << 16) + (Colors.LightGoldenrodYellow.G << 8) + Colors.LightGoldenrodYellow.R;
                                        break;
                                    case ProjectState.Error:
                                        range.Interior.Color = (Colors.Red.B << 16) + (Colors.Red.G << 8) + Colors.Red.R;
                                        break;
                                    case ProjectState.Stop:
                                        range.Interior.Color = (cStop.B << 16) + (cStop.G << 8) + cStop.R;
                                        break;
                                }

                                break;
                            }
                        #endregion
                        #region Контрагент
                        case "Контрагент":
                            {
                                value = project.Customer;
                                break;
                            }
                        #endregion
                        #region Изделие
                        case "Изделие":
                            {
                                value = project.IsStop ? string.Format("СТОП! {0}", project.Product) : project.Product;
                                break;
                            }
                        #endregion
                        #region Опции
                        case "Опции":
                            {
                                value = project.Options;
                                break;
                            }
                        #endregion

                        #region Дата передачи на ОТК (план)
                        case "Дата передачи на ОТК (план)":
                            {
                                value = project.Time_OTK_Plan;
                                break;
                            }
                        #endregion
                        #region Дата передачи на ОТК (факт)
                        case "Дата передачи на ОТК (факт)":
                            {
                                value = project.MF_Time_Test_Actual;
                                break;
                            }
                        #endregion

                        #region Дата испытаний по гидравлике
                        case "Дата испытаний по гидравлике":
                            {
                                if (project.Is_OTK_G_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OTK_G_Actual;

                                break;
                            }
                        #endregion
                        #region Дата испытаний шкафа
                        case "Дата испытаний шкафа":
                            {
                                if (project.Is_OTK_E_NotNeed == true)
                                    value = "не требуется";
                                else
                                    value = project.Time_OTK_E_Actual;

                                break;
                            }
                        #endregion
                        #region Дата отгрузки (план)
                        case "Дата отгрузки (план)":
                            {
                                value = project.TimeEndPlaned;
                                break;
                            }
                        #endregion

                        #region Дата отгрузки (факт)
                        case "Дата отгрузки (факт)":
                            {
                                value = project.TimeEndActual;
                                break;
                            }
                        #endregion
                        #region Долги по комплектации
                        case "Долги по комплектации":
                            {
                                value = project.IsDebtOk == true ? "Нет" : "Да";
                                break;
                            }
                        #endregion

                        default:
                            LogManager.LogError(unit, string.Format("Незивестный запрос: {0} для {1}", v, ws.Name));
                            break;
                    }

                    if (value == null)
                        continue;

                    if (value is DateTime)
                        range.NumberFormat = "ДД.ММ.ГГГГ";

                    // Установка значения
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Value2 = value;

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        if (!size.ContainsKey(v) || size[v].Length < value.ToString().Length)
                        {
                            size[v] = value.ToString();
                            range.Columns.AutoFit();
                        }
                    }

                    // прогресс
                    workerToExcel.ReportProgress((int)((double)row * 100.0 / (double)count), string.Format("Импорт '{0}'", ws.Name));
                }
            }
        }
        #endregion

        #region Недокомплект

        /*Загрузка*/
        /// <summary>Загрузка файла недокомплекта </summary>
        public static List<OMTSRequest> LoadRequests(OMTSRequestType type, string path = @"D:\Dropbox\Develop\Visual Studio\Agregat\Документы\test.xls")
        {
            var result = new List<OMTSRequest>();

            LogManager.LogInfo(unit, string.Format("Загружаем недокомплект из файла: {0}", path));
            if (!File.Exists(path))
            {
                LogManager.LogError(unit, string.Format("Не существует файла: {0}", path));
                return result;
            }

            var app = new Excel.ApplicationClass();
            Excel.Workbook books = null;
            Excel.Worksheet ws = null;

            try
            {
                books = app.Workbooks.Open(path);
                ws = (Excel.Worksheet)books.ActiveSheet;

                //var count = ((Excel.Range)ws.UsedRange).Cells.Count;

                var rProperties = new Dictionary<RequestExcelType, object>();
                var excelStructure = new Dictionary<int, RequestExcelType>();

                foreach (Excel.Range cell in ((Excel.Range)ws.UsedRange).Cells)
                {
                    var text = cell.Value2 as string;

                    // если у нас заполнена структура
                    if (rProperties.Count == eRequestStructureNames.Count)
                    {
                        var newRequest = CreateRequest(rProperties, type);
                        if (newRequest != null)
                            result.Add(newRequest);

                        rProperties.Clear();
                    }

                    // Если мы записываем по структуре
                    if (eRequestStructureNames.Count == excelStructure.Count)
                    {
                        if (excelStructure.ContainsKey(cell.Column))
                            rProperties[excelStructure[cell.Column]] = cell.Value2;
                    }
                    else if (!string.IsNullOrEmpty(text) && eRequestStructureNames.ContainsKey(text)) // настройка структуры
                        excelStructure[cell.Column] = eRequestStructureNames[text];
                }

                books.Close(0);
                app.Quit();
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка загрузки недокомплекта из: {0}", path), ex);
            }
            finally
            {
                ReleaseComObject(ws);
                ReleaseComObject(books);
                ReleaseComObject(app);

                GC.Collect();
            }

            return result;
        }

        /// <summary>Получаем недокомплект</summary>
        static OMTSRequest CreateRequest(Dictionary<RequestExcelType, object> rProperties, OMTSRequestType type)
        {
            int count = 0, index = 0;
            string name = "", article = "";

            DateTime date = DateTime.MinValue;

            foreach (var p in rProperties)
            {
                switch (p.Key)
                {
                    #region Name
                    case RequestExcelType.Name:
                        {
                            name = p.Value as string;
                            break;
                        }
                    #endregion
                    #region Article
                    case RequestExcelType.Article:
                        {
                            article = p.Value as string;
                            break;
                        }
                    #endregion
                    #region Count
                    case RequestExcelType.Count:
                        {
                            if (p.Value != null)
                                int.TryParse(p.Value.ToString(), out count);

                            break;
                        }
                    #endregion
                    #region DateCompletePlan
                    case RequestExcelType.DateCompletePlan:
                        {
                            if (p.Value != null)
                                DateTime.TryParse(p.Value.ToString(), out date);

                            break;
                        }
                    #endregion
                }
            }

            if (!string.IsNullOrEmpty(name) && count > 0)
                return new OMTSRequest(type, name, (uint)count) { Article = article, DateComplete_Plan = (date == DateTime.MinValue ? (DateTime?)null : date) };
            else
                return null;
        }

        /*Сохранение*/
        /// <summary>Сохранение недокомплета в Excel</summary>
        public static void SaveRequestFormat(Project p, List<OMTSRequest> requests)
        {
            var app = new Excel.ApplicationClass();
            var books = app.Workbooks.Add(Type.Missing);
            var ws = (Excel.Worksheet)books.ActiveSheet;

            try
            {
                ws.Name = DateTime.Now.ToString("dd.MM.yyyy");
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                ws.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;

                var cIndex = GetChar(9);

                var range = (Excel.Range)ws.get_Range("A2", cIndex + "2");
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.Font.Bold = true;
                range.Font.Size = 18;
                range.Merge();
                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                range.Value2 = "Карта недокомплекта";

                #region Заголовок
                //дата время печати
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Дата и время печати документа", DateTime.Now.ToString("dd MMMM yyyy года HH:mm"), "A4", cIndex + "4", false, true, 4, 3);
                range = (Excel.Range)ws.get_Range("A4", cIndex + "4");
                range.RowHeight = 30;

                var isHasUOption = ProjectConfiguration.HasU(p.Options);

                //Номер проекта
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Номер проекта", p.ID, "A6", cIndex + "6", false, isHasUOption, 6, 3, true);
                //Контрагент
                CreateRequestHeader(ws, "Контрагент", p.Customer, "A7", cIndex + "7", false, false, 7, 3);
                //Описание проекта
                CreateRequestHeader(ws, "Описание проекта", p.CustomerName, "A8", cIndex + "8", false, false, 8, 3);
                //Изделие
                CreateRequestHeader(ws, "Изделие", p.Product, "A9", cIndex + "9", false, false, 9, 3);
                //Опции
                CreateRequestHeader(ws, "Опции", p.Options, "A10", cIndex + "10", false, isHasUOption, 10, 3, true);
                //Дата отгрузки (план)
                CreateRequestHeader(ws, "Дата отгрузки (план)", p.TimeEndPlaned.ToString("dd.MM.yyyy"), "A11", cIndex + "11", false, false, 11, 3);

                //№ ЛЗК
                CreateRequestHeader(ws, "№ ЛЗК", "", "A12", cIndex + "12", false, false, 12, 3);
                range = (Excel.Range)ws.Cells[12, 3];
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 1d;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

                // примечание
                range = (Excel.Range)ws.get_Range(GetChar(4) + "4", GetChar(9) + "4");
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.Font.Bold = true;
                range.Font.Size = 18;
                range.Merge();
                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                range.Value2 = "Примечание";

                range = (Excel.Range)ws.get_Range(GetChar(4) + "4", GetChar(9) + "12");
                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                #endregion

                #region Таблица заголовок

                // номер             
                CreateRequestHeader(ws, "№пп", "", "A14", "A15", true);
                range = (Excel.Range)ws.get_Range("A14", "A15");
                range.ColumnWidth = 10;

                // Артикул
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Артикул", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;

                // Необходимо приобрести
                cIndex = GetChar(2);
                CreateRequestHeader(ws, "Необходимо приобрести", "", cIndex + "14", cIndex + "15", true);
                // Кол-во
                cIndex = GetChar(3);
                CreateRequestHeader(ws, "Кол-во", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 10;
                // Дата поставки 
                cIndex = GetChar(4);
                CreateRequestHeader(ws, "Дата поставки", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;
                // Кол-во выданного товара
                cIndex = GetChar(5);
                CreateRequestHeader(ws, "Кол-во выданного товара", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;
                // Давальческое
                cIndex = GetChar(6);
                CreateRequestHeader(ws, "Давальческое сырье", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;
                // Дата выдачи со склада
                cIndex = GetChar(7);
                CreateRequestHeader(ws, "Дата выдачи со склада", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;
                // ФИО, Подпись
                CreateRequestHeader(ws, "ФИО, Подпись", "", GetChar(8) + "14", GetChar(9) + "14", true);
                //Выдал
                cIndex = GetChar(8);
                CreateRequestHeader(ws, "Выдал", "", cIndex + "15", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "14");
                range.ColumnWidth = 15;
                //Выдал
                cIndex = GetChar(9);
                CreateRequestHeader(ws, "Получил", "", cIndex + "15", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "15", cIndex + "15");
                range.ColumnWidth = 15;
                #endregion

                int row = 0;
                // Таблица записи
                for (; row < requests.Count; row++)
                    CreateRequestRow(ws, 16 + row, requests[row]);

                app.Visible = true;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка импорта недокомплекта в Excel"), ex);
            }
            finally
            {
                ReleaseComObject(ws);
                ReleaseComObject(books);
                ReleaseComObject(app);

                GC.Collect();
            }
        }

        /// <summary>Создание заголовка для недокомплекта</summary>
        static void CreateRequestHeader(Excel.Worksheet ws, string sName, string sValue, string c1, string c2, bool isUseHeaderBorder = false, bool isUseValueBorder = false, int? rowValue = null, int? colValue = null, bool isDoubleBorder = false)
        {
            var range = (Excel.Range)ws.get_Range(c1, c2);
            range.Merge();
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            if (isUseHeaderBorder)
            {

                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            }
            range.Font.Bold = true;
            range.WrapText = true;
            range.Value2 = sName;
            range.Rows.AutoFit();

            if (rowValue.HasValue && colValue.HasValue)
            {
                range = (Excel.Range)ws.Cells[rowValue.Value, colValue.Value];
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                if (isUseValueBorder)
                {
                    if (isDoubleBorder)
                        range.BorderAround(Excel.XlLineStyle.xlDouble, Excel.XlBorderWeight.xlThin);
                    else
                        range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                }
                range.WrapText = true;
                range.Value2 = sValue;
            }

            range.ColumnWidth = 60;
        }
        /// <summary>Строка описания запроса</summary>
        static void CreateRequestRow(Excel.Worksheet ws, int row, OMTSRequest r)
        {
            // номер
            var range = (Excel.Range)ws.Cells[row, 1];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = (row + 1 - 16).ToString();
            // Артикул
            range = (Excel.Range)ws.Cells[row, 2];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = r.Article;
            // Необходимо приобрести
            range = (Excel.Range)ws.Cells[row, 3];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = r.Name;
            // Кол-во
            range = (Excel.Range)ws.Cells[row, 4];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = r.TotalCount;
            // Дата поставки 
            range = (Excel.Range)ws.Cells[row, 5];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            if (r.DateComplete_Plan.HasValue)
                range.Value2 = r.DateComplete_Plan.Value.ToString("dd.MM:yyyy");
            // Кол-во выданного товара
            range = (Excel.Range)ws.Cells[row, 6];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = r.ExistCount;
            // Давальческое
            range = (Excel.Range)ws.Cells[row, 7];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.WrapText = true;
            range.Value2 = r.IsCustomerMaterials ? "Да" : "";
            range.Rows.AutoFit();

            // Дата выдачи со склада
            range = (Excel.Range)ws.Cells[row, 8];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            if (r.DateComplete.HasValue)
                range.Value2 = r.DateComplete.Value.ToString("dd.MM:yyyy");

            // Выдал 
            range = (Excel.Range)ws.Cells[row, 9];
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            //Подпись
            range = (Excel.Range)ws.Cells[row, 10];
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
        }
        #endregion

        #region Коментарии
        public static void SaveComment(Project p)
        {
            var app = new Excel.ApplicationClass();
            var books = app.Workbooks.Add(Type.Missing);
            var ws = (Excel.Worksheet)books.ActiveSheet;

            try
            {
                ws.Name = DateTime.Now.ToString("dd.MM.yyyy");
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                ws.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;

                var cIndex = GetChar(2);

                var range = (Excel.Range)ws.get_Range("A2", cIndex + "2");
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.Font.Bold = true;
                range.Font.Size = 18;
                range.Merge();
                range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                range.Value2 = "Комментарии";

                #region Заголовок
                //дата время печати
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Дата и время печати документа", DateTime.Now.ToString("dd MMMM yyyy года HH:mm"), "A4", cIndex + "4", false, true, 4, 3);
                range = (Excel.Range)ws.get_Range("A4", cIndex + "4");
                range.RowHeight = 30;

                //Номер проекта
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Номер проекта", p.ID, "A6", cIndex + "6", false, false, 6, 3);
                //Контрагент
                CreateRequestHeader(ws, "Контрагент", p.Customer, "A7", cIndex + "7", false, false, 7, 3);
                //Описание проекта
                CreateRequestHeader(ws, "Описание проекта", p.CustomerName, "A8", cIndex + "8", false, false, 8, 3);
                //Изделие
                CreateRequestHeader(ws, "Изделие", p.Product, "A9", cIndex + "9", false, false, 9, 3);
                //Опции
                CreateRequestHeader(ws, "Опции", p.Options, "A10", cIndex + "10", false, false, 10, 3);
                //Дата отгрузки (план)
                CreateRequestHeader(ws, "Дата отгрузки (план)", p.TimeEndPlaned.ToString("dd.MM.yyyy"), "A11", cIndex + "11", false, false, 11, 3);

                //№ ЛЗК
                CreateRequestHeader(ws, "№ ЛЗК", "", "A12", cIndex + "12", false, false, 12, 3);
                range = (Excel.Range)ws.Cells[12, 3];
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 1d;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                #endregion

                #region Таблица заголовок

                // Дата             
                CreateRequestHeader(ws, "Дата", "", "A14", "A15", true);
                range = (Excel.Range)ws.get_Range("A14", "A15");
                range.ColumnWidth = 10;

                // Автор
                cIndex = GetChar(1);
                CreateRequestHeader(ws, "Автор", "", cIndex + "14", cIndex + "15", true);
                range = (Excel.Range)ws.get_Range(cIndex + "14", cIndex + "15");
                range.ColumnWidth = 15;

                // Сообщение
                cIndex = GetChar(2);
                CreateRequestHeader(ws, "Сообщение", "", cIndex + "14", cIndex + "15", true);
                #endregion

                int row = 0;
                // Таблица записи
                for (; row < p.Messages.Count; row++)
                    CreateMessageRow(ws, 16 + row, p.Messages[row]);

                app.Visible = true;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка импорта комментариев в в Excel"), ex);
            }
            finally
            {
                ReleaseComObject(ws);
                ReleaseComObject(books);
                ReleaseComObject(app);

                GC.Collect();
            }
        }

        /// <summary>Строка описания комментария</summary>
        static void CreateMessageRow(Excel.Worksheet ws, int row, AgrProjectComment comment)
        {
            // Дата
            var range = (Excel.Range)ws.Cells[row, 1];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = comment.Date.ToString("dd.MM:yyyy");
            // Автор
            range = (Excel.Range)ws.Cells[row, 2];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = comment.User;
            // Сообщение
            range = (Excel.Range)ws.Cells[row, 3];
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
            range.Rows.AutoFit();
            range.WrapText = true;
            range.Value2 = comment.Message;
        }
        #endregion

        #region Обработчики событий
        // поток импорта
        static void workerToExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is ProjectViewMode)
                ToExcelCore((ProjectViewMode)e.Argument);
        }
        // прогресс изменился
        static void workerToExcel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DoProgressChanged(e);
        }
        // поток сохранения в Excel закончился
        static void workerToExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DoCompleted(e);
        }

        // загрузка в Excel по отдельному проекту
        static void workerProjectToExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Project)
                ToExcelCore((Project)e.Argument);
        }
        static void workerProjectToExcel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DoProgressChanged(e);
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>Получение символа по Index</summary>
        static string GetChar(int idx)
        {
            int start = (int)'A',
                end = (int)'Z';

            var v = "";
            if (idx / (end - start + 1) >= 1)
                v = GetChar((int)(idx / (end - start + 1)) - 1);

            return v + ((char)(start + (idx) % (end - start + 1)));
        }

        /// <summary>Очитска ресурсов COM object</summary>
        static void ReleaseComObject(object obj)
        {
            if (obj == null)
                return;

            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
        }
        #endregion

        #region Вспомогательные классы
        enum RequestExcelType
        {
            Index,
            Article,
            Name,
            Count,
            DateCompletePlan,
        }
        #endregion

        #region События

        #region ProgressChanged
        public static event ProgressChangedEventHandler ProgressChanged;
        static void DoProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(null, e);
        }
        #endregion

        #region Completed
        public static event RunWorkerCompletedEventHandler Completed;
        static void DoCompleted(RunWorkerCompletedEventArgs e)
        {
            if (Completed != null)
                Completed(null, e);
        }
        #endregion

        #endregion
    }
}
