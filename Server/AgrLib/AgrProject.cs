using System;
using System.Collections.Generic;

namespace ArgDb
{
    [Serializable]
    /// <summary>Описание проекта</summary>
    public class AgrProject
    {
        #region Поля
        readonly List<AgrRequest> _requests = new List<AgrRequest>();
        readonly List<AgrProjectComment> _messages = new List<AgrProjectComment>();
        #endregion

        #region Свойства
        /// <summary>ID по БД</summary>
        public int ProjectId { get; set; }

        /// <summary>id по системе (не уникально)</summary>
        public string ID { get; set; }

        /// <summary>Контрагент</summary>
        public string Customer { get; set; }
        /// <summary>Заказчик</summary>
        public string CustomerName { get; set; }

        /// <summary>Проект в стопе</summary>
        public bool IsStop { get; set; }

        /// <summary>Изделие</summary>
        public string Product { get; set; }
        /// <summary>Опции</summary>
        public string Options { get; set; }

        /// <summary>Дата изменения последнего</summary>
        public DateTime ChangedDate { get; set; }

        /// <summary>Менеджеры сами устанавливают планируемое время.</summary>
        public bool IsManagerSetPlanDate { get; set; }

        #region ITO
        /// <summary>Гидравлические схемы (план)</summary>
        public DateTime? Time_ITO_G_Plan { get; set; }
        /// <summary>Гидравлические схемы </summary>
        public DateTime? Time_ITO_G_Actual { get; set; }
        /// <summary>ИТО не нужна гидравлика</summary>
        public bool Is_ITO_G_NotNeed { get; set; }

        /// <summary>Электрические схемы (план)</summary>
        public DateTime? Time_ITO_E_Plan { get; set; }
        /// <summary>Электрические схемы</summary>
        public DateTime? Time_ITO_E_Actual { get; set; }
        /// <summary>ИТО не нужна электрика</summary>
        public bool Is_ITO_E_NotNeed { get; set; }

        /// <summary>Рама (план)</summary>
        public DateTime? Time_ITO_R_Plan { get; set; }
        /// <summary>Рама</summary>
        public DateTime? Time_ITO_R_Actual { get; set; }
        /// <summary>ИТО не нужна рама</summary>
        public bool Is_ITO_R_NotNeed { get; set; }

        /// <summary>ИТО Рама тип</summary>
        public string ITO_R_Mode { get; set; }
        #endregion

        #region Warehouse
        /// <summary>СКЛАД Гидравлика (план)</summary>
        public DateTime? Time_WH_G_Plan { get; set; }
        /// <summary>СКЛАД Гидравлика</summary>
        public DateTime? Time_WH_G_Actual { get; set; }
        /// <summary>СКЛАД не нужна гидравлика</summary>
        public bool Is_WH_G_NotNeed { get; set; }

        /// <summary>СКЛАД Электрика (план)</summary>
        public DateTime? Time_WH_E_Plan { get; set; }
        /// <summary>СКЛАД Электрика</summary>
        public DateTime? Time_WH_E_Actual { get; set; }
        /// <summary>СКЛАД не нужна Электрика</summary>
        public bool Is_WH_E_NotNeed { get; set; }

        /// <summary>СКЛАД Рама (план)</summary>
        public DateTime? Time_WH_R_Plan { get; set; }
        /// <summary>СКЛАД Рама</summary>
        public DateTime? Time_WH_R_Actual { get; set; }
        /// <summary>СКЛАД не нужна Рама</summary>
        public bool Is_WH_R_NotNeed { get; set; }

        /// <summary>СКЛАД дата создания недокомплекта электрика</summary>
        public DateTime? Time_WH_E_Requests_Create { get; set; }
        /// <summary>СКЛАД дата создания недокомплекта гидравлика</summary>
        public DateTime? Time_WH_G_Requests_Create { get; set; }
        #endregion

        #region OMTS
        /// <summary>ОМТС Гидравлика (план)</summary>
        public DateTime? Time_OMTS_G_Plan { get; set; }
        /// <summary>ОМТС Гидравлика</summary>
        public DateTime? Time_OMTS_G_Actual { get; set; }

        /// <summary>ОМТС Электрика (план)</summary>
        public DateTime? Time_OMTS_E_Plan { get; set; }
        /// <summary>ОМТС Электрика</summary>
        public DateTime? Time_OMTS_E_Actual { get; set; }

        #endregion

        #region Commercial
        /// <summary>Время начало проекта</summary>
        public DateTime TimeBegin { get; set; }
        /// <summary>Запланированное время окончание проекта</summary>
        public DateTime TimeEndPlaned { get; set; }

        /// <summary>Измененное время окончание проекта</summary>
        public DateTime? Com_New_Time { get; set; }

        /// <summary>Фактическое время завершения проекта</summary>
        public DateTime? TimeEndActual { get; set; }

        /// <summary>Тип упаковки</summary>
        public string COM_Package_Type { get; set; }
        #endregion

        #region Manufacture
        /// <summary>Производство участок</summary>
        public string MF_Level { get; set; }
        /// <summary>Производство рама</summary>
        public string MF_Rama { get; set; }
        /// <summary>Производство прогресс</summary>
        public string MF_Complete_Percentage { get; set; }

        /// <summary>Производство коллектор</summary>
        public string MF_Collector { get; set; }

        /// <summary>Производство Номер поста</summary>
        public string MF_Post { get; set; }
        /// <summary>Производство Расключение агрегата</summary>
        public string MF_Agregat { get; set; }
        /// <summary>Производство Место изготовления ШУ</summary>
        public string MF_SH_Place { get; set; }
        /// <summary>Производство ШУ</summary>
        public string MF_SH { get; set; }

        /// <summary>Производство Планируемая дата завершения</summary>
        public DateTime? MF_Time_Plan { get; set; }

        /// <summary>Производство Тест факт</summary>
        public DateTime? MF_Time_Test_Actual { get; set; }

        /// <summary>Производство Время завершения</summary>
        public DateTime? MF_Time { get; set; }
        #endregion

        #region OTK
        /// <summary>ОТК дата испытаний (план)</summary>
        public DateTime? Time_OTK_Plan { get; set; }

        /// <summary>ОТК дата испытаний по гидравлике</summary>
        public DateTime? Time_OTK_G_Actual { get; set; }
        /// <summary>ОТК не нужна гидравлика</summary>
        public bool Is_OTK_G_NotNeed { get; set; }

        /// <summary>ОТК дата испытаний эл Шкафа</summary>
        public DateTime? Time_OTK_E_Actual { get; set; }
        /// <summary>ОТК не нужна электрика</summary>
        public bool Is_OTK_E_NotNeed { get; set; }
        #endregion

        /// <summary>Запросы недокомплекта</summary>
        public List<AgrRequest> Requests { get { return _requests; } }
        /// <summary>Коментарии</summary>
        public List<AgrProjectComment> Messages { get { return _messages; } }

        #endregion

        public AgrProject(string id, string customer, string product, string options, DateTime sDate, DateTime eDate)
        {
            ID = id;
            Customer = customer;
            Product = product;
            Options = options;
            TimeBegin = sDate;
            TimeEndPlaned = eDate;
        }
        public AgrProject(int pId)
        {
            ProjectId = pId;
        }

        public bool Load()
        {
            lock (Db.Ds)
            //if (Db.LoadDate(ProjectId))
            {
                AgrDataSet.ProjectsRow drProjects = Db.Ds.Projects.FindById(ProjectId);
                ChangedDate = drProjects.Date;
                if (Db.LoadProject(ProjectId))
                {
                    AgrDataSet.ProjectRow drProject = Db.Ds.Project.FindById(ProjectId);
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class LProjects : List<AgrProject>
    {
        public bool IsReload { get; set; }
    }

    [Serializable]
    public class LWorkers : List<MFWorker>
    {
        public bool IsReload { get; set; }
    }

    [Serializable]
    public static class ClProjects
    {
        public static object LockObject = new object();
        public enum StateTime
        {
            None,
            Changed,
            Worked,
        }

        // после этого перестает падать надо блокировать доступ к базе во время чтения записи..
        // куча клиентов лезет одновременно
        //static readonly object lockObject = new object();
        //public static LProjects Projects(DateTime? fromDate, StateTime stateTime, int previousId)
        //{
        //    lock (lockObject)
        //    {
        //    }
        //}

        public static LProjects Projects(DateTime? fromDate, StateTime stateTime, int previousId)
        {
            lock (Db.Ds)
            {
                if (!Db.LoadProjects())
                    return null;
                AgrDataSet.ProjectsRow[] drsProjects;
                if (fromDate == null)
                {
                    if (stateTime == StateTime.Worked)
                    {
                        if (previousId >= 0)
                            drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                                                                         Select("TimeEndActual = '" + DateTime.MinValue
                                                                                + "' AND Id > " + previousId, "Id");
                        else
                            drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                                                                         Select("TimeEndActual = '" + DateTime.MinValue +
                                                                                "'");
                    }
                    else
                        drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.Select();
                }
                else
                {
                    switch (stateTime)
                    {
                        case StateTime.Changed:
                            drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                                                                         Select("Date > '" + fromDate + "'");
                            break;
                        case StateTime.Worked:
                            //drsProject = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                            //    Select("TimeEndActual = '" + DateTime.MinValue + "'");
                            if (previousId >= 0)
                                drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                                                                             Select(
                                                                                 "((TimeEndActual = '" +
                                                                                 DateTime.MinValue
                                                                                 + "') OR (TimeEndActual > '" + fromDate
                                                                                 + "')) AND Id > " + previousId, "Id");
                            else
                                drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.
                                                                             Select("(TimeEndActual = '" +
                                                                                    DateTime.MinValue
                                                                                    + "') OR (TimeEndActual > '" +
                                                                                    fromDate + "')");
                            break;
                        default:
                            drsProjects = (AgrDataSet.ProjectsRow[])Db.Ds.Projects.Select();
                            break;
                    }

                }
                var projects = new LProjects();
                int index1 = 0;
                for (int index = 0; index < drsProjects.Length; index++)
                {
                    if (index1 >= 10)
                        break;
                    AgrDataSet.ProjectsRow drProjects = drsProjects[index];
                    if ((previousId >= 0) && (previousId == drProjects.Id))
                        continue;
                    if (Db.LoadProject(drProjects.Id))
                    {
                        index1++;
                        AgrDataSet.ProjectRow drProject = drProjects.GetProjectRows()[0];
                        AgrDataSet.ClientProjectRow[] drsClientProject = drProjects.GetClientProjectRows();
                        if (drsClientProject.Length == 0)
                            continue;
                        AgrDataSet.ClientProjectRow drClientProject = drsClientProject[0];
                        AgrDataSet.ClientRow drClient = drClientProject.ClientRow;

                        AgrDataSet.ProjectProductRow[] drsProjectProduct = drProjects.GetProjectProductRows();
                        if (drsProjectProduct.Length == 0)
                            continue;
                        AgrDataSet.ProjectProductRow drProjectProduct = drsProjectProduct[0];
                        AgrDataSet.ProductRow drProduct = drProjectProduct.ProductRow;

                        var project = new AgrProject(drProject.Id)
                        {
                            ID = drProject.Name,
                            Customer = drClient.Name,
                            CustomerName = drProject.CustomerName,
                            Product = drProduct.Name,
                            Options = drProject.Options,
                            ChangedDate = drProjects.Date,
                            TimeEndActual =
                                drProjects.TimeEndActual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProjects.TimeEndActual,

                            IsManagerSetPlanDate = drProject.IsManagerSetPlanDate,

                            IsStop = drProject.IsStop,
                            COM_Package_Type = drProject.COM_Package_Type,

                            #region ITO

                            #region G
                            Time_ITO_G_Plan =
                                drProject.Time_ITO_G_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_G_Plan,

                            Time_ITO_G_Actual =
                                drProject.Time_ITO_G_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_G_Actual,

                            Is_ITO_G_NotNeed = drProject.Is_ITO_G_NotNeed,

                            #endregion G

                            #region E
                            Time_ITO_E_Plan =
                                drProject.Time_ITO_E_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_E_Plan,

                            Time_ITO_E_Actual =
                                drProject.Time_ITO_E_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_E_Actual,

                            Is_ITO_E_NotNeed = drProject.Is_ITO_E_NotNeed,

                            #endregion E

                            #region R
                            Time_ITO_R_Plan =
                                drProject.Time_ITO_R_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_R_Plan,

                            Time_ITO_R_Actual =
                                drProject.Time_ITO_R_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_ITO_R_Actual,

                            ITO_R_Mode = drProject.ITO_R_Mode,

                            Is_ITO_R_NotNeed = drProject.Is_ITO_R_NotNeed,

                            #endregion R

                            #endregion ITO

                            #region WH

                            #region G
                            Time_WH_G_Plan =
                                drProject.Time_WH_G_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_G_Plan,

                            Time_WH_G_Actual =
                                drProject.Time_WH_G_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_G_Actual,

                            Is_WH_G_NotNeed = drProject.Is_WH_G_NotNeed,

                            Time_WH_G_Requests_Create =
                                drProject.Time_WH_G_Requests_Create == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_G_Requests_Create,

                            #endregion G

                            #region E
                            Time_WH_E_Plan =
                                drProject.Time_WH_E_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_E_Plan,

                            Time_WH_E_Actual =
                                drProject.Time_WH_E_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_E_Actual,

                            Is_WH_E_NotNeed = drProject.Is_WH_E_NotNeed,

                            Time_WH_E_Requests_Create =
                                drProject.Time_WH_E_Requests_Create == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_E_Requests_Create,

                            #endregion E

                            #region R
                            Time_WH_R_Plan =
                                drProject.Time_WH_R_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_R_Plan,

                            Time_WH_R_Actual =
                                drProject.Time_WH_R_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_WH_R_Actual,

                            Is_WH_R_NotNeed = drProject.Is_WH_R_NotNeed,

                            #endregion R

                            #endregion WH

                            #region OMTS

                            #region G
                            Time_OMTS_G_Plan =
                                drProject.Time_OMTS_G_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OMTS_G_Plan,

                            Time_OMTS_G_Actual =
                                drProject.Time_OMTS_G_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OMTS_G_Actual,

                            #endregion G

                            #region E
                            Time_OMTS_E_Plan =
                                drProject.Time_OMTS_E_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OMTS_E_Plan,

                            Time_OMTS_E_Actual =
                                drProject.Time_OMTS_E_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OMTS_E_Actual,

                            #endregion E

                            #endregion OMTS

                            TimeBegin = drProject.TimeBegin,
                            TimeEndPlaned = drProject.TimeEndPlaned,
                            Com_New_Time =
                                drProject.Com_New_Time == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Com_New_Time,
                            MF_Collector = drProject.MF_Collector,
                            MF_Complete_Percentage = drProject.MF_Complete_Percentage,
                            MF_Level = drProject.MF_Level,
                            MF_Rama = drProject.MF_Rama,
                            MF_Post = drProject.MF_Post,
                            MF_Agregat = drProject.MF_Agregat,
                            MF_SH_Place = drProject.MF_SH_Place,
                            MF_SH = drProject.MF_SH,
                            MF_Time_Plan =
                                drProject.MF_Time_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.MF_Time_Plan,
                            MF_Time_Test_Actual =
                                drProject.MF_Time_Test_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.MF_Time_Test_Actual,
                            MF_Time = drProject.MF_Time == DateTime.MinValue ? (DateTime?) null : drProject.MF_Time,

                            #region OTK
                            Time_OTK_Plan =
                                drProject.Time_OTK_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OTK_Plan,

                            Time_OTK_G_Actual =
                                drProject.Time_OTK_G_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OTK_G_Actual,

                            Time_OTK_E_Actual =
                                drProject.Time_OTK_E_Actual == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drProject.Time_OTK_E_Actual,
                            Is_OTK_G_NotNeed = drProject.Is_OTK_G_NotNeed,
                            Is_OTK_E_NotNeed = drProject.Is_OTK_E_NotNeed,

                            #endregion OTK
                        };

                        // Сообщения
                        foreach (AgrDataSet.CommentRow drComment in drProjects.GetCommentRows())
                            project.Messages.Add(new AgrProjectComment(drComment.Time, drComment.UserName,
                                drComment.Value));

                        // Недокомплект
                        foreach (AgrDataSet.RequestRow drRequest in drProjects.GetRequestRows())
                            project.Requests.Add(new AgrRequest((OMTSRequestType) drRequest.Type, drRequest.Name)
                            {
                                TotalCount = drRequest.TotalCount,
                                ExistCount = drRequest.ExistCount,
                                Article = drRequest.Article,
                                DateComplete_Plan = drRequest.DateComplete_Plan == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drRequest.DateComplete_Plan,
                                DateComplete = drRequest.DateComplete == DateTime.MinValue
                                    ? (DateTime?) null
                                    : drRequest.DateComplete,
                                IsCustomerMaterials = drRequest.IsCustomerMaterials
                            });

                        projects.Add(project);
                    }
                }
                return projects;
            }
        }
    }
}
