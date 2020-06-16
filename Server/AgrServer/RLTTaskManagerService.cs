using ArgDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using ArgLib.Logger;

namespace AgrServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RLTTaskManagerService : IRLTTaskManagerService
    {
        #region Поля
        const string unit = "RLTTaskManagerService";
        #endregion

        #region Session

        #endregion Session

        public LUsers GetUsers()
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Запрос на список пользователей");
                Db.LoadUsers();

              return Db.Users.Users();
            }
        }

        public bool Login(string name, string password)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Проверка пароля");
                return Db.Login(name, password);
            }
        }

        public LClients GetClients()
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Запрос заказчиков");
                return Db.Clients;
            }
        }

        public LProducts GetProducts()
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Запрос Изделий");
                return Db.Products;
            }
        }

        private string IpClient()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpoint == null ? "???" : endpoint.Address;
        }

        /// <summary>Загружаем проекты</summary>
        public LProjects GetChangies(DateTime fromDate)
        {
            lock (Db.Ds)
            {
                //LogManager.LogTrace(IpClient(), "Запрос измененных проектов");
                return Db.ChangeProjects(fromDate);
            }
        }

        /// <summary>Загружаем проекты</summary>
        public LProjects GetProjects(DateTime fromTime, int previousId)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Запрос проектов");
                return Db.WorkProjects(fromTime, previousId);
            }
        }
        /// <summary>Загружаем проект</summary>
        public AgrProject GetProject(int id)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Запрос проекта " + id);
                AgrDataSet.ProjectsRow drProjects = Db.Ds.Projects.FindById(id);
                if (drProjects == null)
                    return null;
                AgrDataSet.ProjectRow drProject = drProjects.GetProjectRows()[0];
                AgrDataSet.ClientProjectRow drClientProject = drProjects.GetClientProjectRows()[0];
                AgrDataSet.ClientRow drClient = drClientProject.ClientRow;
                AgrDataSet.ProjectProductRow drProjectProduct = drProjects.GetProjectProductRows()[0];
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
                                   ? (DateTime?)null
                                   : drProjects.TimeEndActual,

                    IsManagerSetPlanDate = drProject.IsManagerSetPlanDate,

                    IsStop = drProject.IsStop,
                    COM_Package_Type = drProject.COM_Package_Type,

                    #region ITO

                    #region G
                    Time_ITO_G_Plan =
                                   drProject.Time_ITO_G_Plan == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_G_Plan,

                    Time_ITO_G_Actual =
                                   drProject.Time_ITO_G_Actual == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_G_Actual,

                    Is_ITO_G_NotNeed = drProject.Is_ITO_G_NotNeed,
                    #endregion G

                    #region E
                    Time_ITO_E_Plan =
                                   drProject.Time_ITO_E_Plan == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_E_Plan,

                    Time_ITO_E_Actual =
                                   drProject.Time_ITO_E_Actual == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_E_Actual,

                    Is_ITO_E_NotNeed = drProject.Is_ITO_E_NotNeed,
                    #endregion E

                    #region R
                    Time_ITO_R_Plan =
                                   drProject.Time_ITO_R_Plan == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_R_Plan,

                    Time_ITO_R_Actual =
                                   drProject.Time_ITO_R_Actual == DateTime.MinValue
                                       ? (DateTime?)null
                                       : drProject.Time_ITO_R_Actual,

                    ITO_R_Mode = drProject.ITO_R_Mode,

                    Is_ITO_R_NotNeed = drProject.Is_ITO_R_NotNeed,
                    #endregion R

                    #endregion ITO

                    #region WH

                    #region G
                    Time_WH_G_Plan =
                               drProject.Time_WH_G_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_G_Plan,

                    Time_WH_G_Actual =
                               drProject.Time_WH_G_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_G_Actual,

                    Is_WH_G_NotNeed = drProject.Is_WH_G_NotNeed,

                    Time_WH_G_Requests_Create =
                               drProject.Time_WH_G_Requests_Create == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_G_Requests_Create,

                    #endregion G

                    #region E
                    Time_WH_E_Plan =
                               drProject.Time_WH_E_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_E_Plan,

                    Time_WH_E_Actual =
                               drProject.Time_WH_E_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_E_Actual,

                    Is_WH_E_NotNeed = drProject.Is_WH_E_NotNeed,

                    Time_WH_E_Requests_Create =
                               drProject.Time_WH_E_Requests_Create == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_E_Requests_Create,
                    #endregion E

                    #region R
                    Time_WH_R_Plan =
                               drProject.Time_WH_R_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_R_Plan,

                    Time_WH_R_Actual =
                               drProject.Time_WH_R_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_WH_R_Actual,

                    Is_WH_R_NotNeed = drProject.Is_WH_R_NotNeed,
                    #endregion R

                    #endregion WH

                    #region OMTS

                    #region G
                    Time_OMTS_G_Plan =
                               drProject.Time_OMTS_G_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OMTS_G_Plan,

                    Time_OMTS_G_Actual =
                               drProject.Time_OMTS_G_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OMTS_G_Actual,
                    #endregion G

                    #region E
                    Time_OMTS_E_Plan =
                               drProject.Time_OMTS_E_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OMTS_E_Plan,

                    Time_OMTS_E_Actual =
                               drProject.Time_OMTS_E_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OMTS_E_Actual,
                    #endregion E

                    #endregion OMTS

                    TimeBegin = drProject.TimeBegin,
                    TimeEndPlaned = drProject.TimeEndPlaned,
                    Com_New_Time =
                               drProject.Com_New_Time == DateTime.MinValue
                                   ? (DateTime?)null
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
                                   ? (DateTime?)null
                                   : drProject.MF_Time_Plan,
                    MF_Time_Test_Actual =
                               drProject.MF_Time_Test_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.MF_Time_Test_Actual,
                    MF_Time = drProject.MF_Time == DateTime.MinValue ? (DateTime?)null : drProject.MF_Time,

                    #region OTK
                    Time_OTK_Plan =
                               drProject.Time_OTK_Plan == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OTK_Plan,

                    Time_OTK_G_Actual =
                               drProject.Time_OTK_G_Actual == DateTime.MinValue
                                   ? (DateTime?)null
                                   : drProject.Time_OTK_G_Actual,

                    Time_OTK_E_Actual =
                               drProject.Time_OTK_E_Actual == DateTime.MinValue
                                   ? (DateTime?)null
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
                    project.Requests.Add(new AgrRequest((OMTSRequestType)drRequest.Type, drRequest.Name)
                    {
                        TotalCount = drRequest.TotalCount,
                        ExistCount = drRequest.ExistCount,
                        Article = drRequest.Article,
                        DateComplete_Plan = drRequest.DateComplete_Plan == DateTime.MinValue
                                                    ? (DateTime?)null
                                                    : drRequest.DateComplete_Plan,
                        DateComplete = drRequest.DateComplete == DateTime.MinValue
                                               ? (DateTime?)null
                                               : drRequest.DateComplete,
                        IsCustomerMaterials = drRequest.IsCustomerMaterials
                    });
                return project;
            }
        }
        public bool RemoveProject(int id)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на удаление " + id);
                return Db.RemoveProject(id);
            }
        }

        /// <summary>Создает новый проект и возвращает ID</summary>
        public int CreateProject(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate, string comments, string userName, bool isManagerSetPlanDate, string packageType)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на создание проекта " + id);
                return Db.CreateProject(id, customer, customerName, product, options, sDate, eDate, comments, userName, isManagerSetPlanDate, packageType);
            }
        }
        /// <summary>Проверка на совпадение </summary>
        public bool CheckIsExist(string id, string customer, string customerName,
            string product, string options, DateTime sDate, DateTime eDate)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Проверка на совпадение проекта " + id);
                return Db.CheckIsExist(id, customer, customerName, product, options, sDate, eDate);
            }
        }
        /// <summary>Изменить состояние проекта</summary>
        public bool ChangeProject(AgrProject project)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на изменение проекта " + project.ID +
                    "- ProjectId: " + project.ProjectId +
                    "; ID: " + project.ID +
                    "; Customer: " + project.Customer +
                    "; CustomerName: " + project.CustomerName +
                    "; IsStop: " + project.IsStop +
                    "; Product: " + project.Product +
                    "; Options: " + project.Options +
                    "; ChangedDate: " + project.ChangedDate +
                    "; IsManagerSetPlanDate: " + project.IsManagerSetPlanDate +
                    "; Time_ITO_G_Plan: " + (project.Time_ITO_G_Plan?.ToString() ?? "") +
                    "; Time_ITO_G_Actual: " + (project.Time_ITO_G_Actual?.ToString() ?? "") +
                    "; Is_ITO_G_NotNeed: " + project.Is_ITO_G_NotNeed +
                    "; Time_ITO_E_Plan: " + (project.Time_ITO_E_Plan?.ToString() ?? "") +
                    "; Time_ITO_E_Actual: " + (project.Time_ITO_E_Actual?.ToString() ?? "") +
                    "; Is_ITO_E_NotNeed: " + project.Is_ITO_E_NotNeed +
                    "; Time_ITO_R_Plan: " + (project.Time_ITO_R_Plan?.ToString() ?? "") +
                    "; Time_ITO_R_Actual: " + (project.Time_ITO_R_Actual?.ToString() ?? "") +
                    "; Is_ITO_R_NotNeed: " + project.Is_ITO_R_NotNeed +
                    "; ITO_R_Mode: " + project.ITO_R_Mode +
                    "; Time_WH_G_Plan: " + (project.Time_WH_G_Plan?.ToString() ?? "") +
                    "; Time_WH_G_Actual: " + (project.Time_WH_G_Actual?.ToString() ?? "") +
                    "; Is_WH_G_NotNeed: " + project.Is_WH_G_NotNeed +
                    "; Time_WH_E_Plan: " + (project.Time_WH_E_Plan?.ToString() ?? "") +
                    "; Time_WH_E_Actual: " + (project.Time_WH_E_Actual?.ToString() ?? "") +
                    "; Is_WH_E_NotNeed: " + project.Is_WH_E_NotNeed +
                    "; Time_WH_R_Plan: " + (project.Time_WH_R_Plan?.ToString() ?? "") +
                    "; Time_WH_R_Actual: " + (project.Time_WH_R_Actual?.ToString() ?? "") +
                    "; Is_WH_R_NotNeed: " + project.Is_WH_R_NotNeed +
                    "; Time_WH_E_Requests_Create: " + (project.Time_WH_E_Requests_Create?.ToString() ?? "") +
                    "; Time_WH_G_Requests_Create: " + (project.Time_WH_G_Requests_Create?.ToString() ?? "") +
                    "; Time_OMTS_G_Plan: " + (project.Time_OMTS_G_Plan?.ToString() ?? "") +
                    "; Time_OMTS_G_Actual: " + (project.Time_OMTS_G_Actual?.ToString() ?? "") +
                    "; Time_OMTS_E_Plan: " + (project.Time_OMTS_E_Plan?.ToString() ?? "") +
                    "; Time_OMTS_E_Actual: " + (project.Time_OMTS_E_Actual?.ToString() ?? "") +
                    "; TimeBegin: " + project.TimeBegin +
                    "; TimeEndPlaned: " + project.TimeEndPlaned +
                    "; Com_New_Time: " + (project.Com_New_Time?.ToString() ?? "") +
                    "; TimeEndActual: " + (project.TimeEndActual?.ToString() ?? "") +
                    "; COM_Package_Type: " + project.COM_Package_Type +
                    "; MF_Level: " + project.MF_Level +
                    "; MF_Rama: " + project.MF_Rama +
                    "; MF_Complete_Percentage: " + project.MF_Complete_Percentage +
                    "; MF_Collector: " + project.MF_Collector +
                    "; MF_Post: " + project.MF_Post +
                    "; MF_Agregat: " + project.MF_Agregat +
                    "; MF_SH_Place: " + project.MF_SH_Place +
                    "; MF_SH: " + project.MF_SH +
                    "; MF_Time_Plan: " + (project.MF_Time_Plan?.ToString() ?? "") +
                    "; MF_Time_Test_Actual: " + (project.MF_Time_Test_Actual?.ToString() ?? "") +
                    "; MF_Time: " + (project.MF_Time?.ToString() ?? "") +
                    "; Time_OTK_Plan: " + (project.Time_OTK_Plan?.ToString() ?? "") +
                    "; Time_OTK_G_Actual: " + (project.Time_OMTS_E_Actual?.ToString() ?? "") +
                    "; Is_OTK_G_NotNeed: " + project.Is_OTK_G_NotNeed +
                    "; Time_OTK_E_Actual: " + (project.Time_OTK_E_Actual?.ToString() ?? "") +
                    "; Is_OTK_E_NotNeed: " + project.Is_OTK_E_NotNeed);
                return Db.SetProject(project);
            }
        }

        /// <summary>Добавляем коментарий к проекту</summary>
        public bool AddCommentToProject(int projectId, DateTime time, string message, string userName)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на добавление коментария проекта " + projectId);
                return Db.SetComment(projectId, time, message, userName);
            }
        }
        /// <summary>Удаляем коментарий к проекту</summary>
        public bool RemoveCommentFromProject(int projectId, DateTime time, string message, string userName)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на удаление коментария проекта " + projectId);
                return Db.RemoveComment(projectId, time, message, userName);
            }
        }
        /// <summary>Удаляем все коментарий к проекту</summary>
        public bool ClearCommentsFromProject(int projectId)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на очистку коментариев проекта " + projectId);
                return Db.ClearComment(projectId);
            }
        }

        public string ServerVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public bool AddRequestToProject(int projectId, DateTime time, string message, string userName)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "AddRequestToProject " + projectId);
                return Db.SetRequest(projectId, time, message, userName);
            }
        }

        #region MFPlanner
        #region Работники
        /// <summary>Возвращает список работников</summary>
        //public MFWorker[] MFPlannerGetWorkers()
        //{
        //    throw new NotImplementedException("MFPlannerGetWorkers");
        //}
        /// <summary>Возвращает список работников</summary>
        public MFWorker[] MFPlannerGetWorkers(DateTime updateTime)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "список работников");
                Db.LoadPlanners();
                return Db.Ds.Worker.Select(s => new MFWorker()
                {
                    Id = s.Id,
                    Post = s.Post,
                    Name = s.Name,
                    SecondName = s.SecondName,
                    EndWorkTime = s.EndWorkTime == DateTime.MinValue ? null : (DateTime?)s.EndWorkTime,
                }).ToArray();
            }
        }

        /// <summary>Создает работника</summary>
        public int MFPlannerCreateWorker(string name, string secondName, int post, DateTime? endWorkTime)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на создание работника " + name);
                AgrDataSet.PostRow drPost = Db.Ds.Post.FindById(post) ?? Db.Ds.Post.AddPostRow(post);
                AgrDataSet.WorkerRow drWorker = Db.Ds.Worker.AddWorkerRow(drPost, name, secondName,
                    endWorkTime == null ? new DateTime() : (DateTime)endWorkTime);
                if (Db.SavePlanners())
                    return drWorker.Id;
                drWorker.Delete();
                return -1;
            }
        }

        /// <summary>Меняет описание работника</summary>
        public bool MFPlannerChangeWorker(MFWorker worker)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Запрос на изиенение работника " + worker.Id);
                Db.LoadPlanners();
                var dbWorker = Db.Ds.Worker.Where(s => s.Id == worker.Id).SingleOrDefault();
                if (dbWorker != null)
                {
                    dbWorker.Post = worker.Post;
                    dbWorker.Name = worker.Name;
                    dbWorker.SecondName = worker.SecondName;
                    dbWorker.EndWorkTime = !worker.EndWorkTime.HasValue ? DateTime.MinValue : worker.EndWorkTime.Value;
                    if (Db.SavePlanners())
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region Действия
        /// <summary>Возвращает список действий для работника за определенный период</summary>
        public MFPlannerAction[] MFPlannerGetActions(int workerId, DateTime? sDate, DateTime? eDate)
        {
            lock (Db.Ds)
            {
                LogManager.LogDebug(IpClient(), "Список действий для работника " + workerId);
                Db.LoadPlanners();
                var action = Db.Ds.Action;
                return (from a in Db.Ds.Action
                        where a.WorkerId == workerId && (sDate.HasValue ? sDate.Value <= a.TimeBegin : true) && (eDate.HasValue ? eDate.Value >= a.TimeBegin : true)
                        select new MFPlannerAction((MFWorkerActionType)a.Type, a.Id, a.WorkerId, a.ProjectsRow != null ? a.ProjectsRow.Id : -1)
                        {
                            TimeBegin = a.TimeBegin,
                            Comment = a.Comment,
                            Days = a.Days
                        }).ToArray();
            }
        }

        /// <summary>Создаем новое действие для пользователя</summary>
        public int MFPlannerCreateAction(MFWorkerActionType type, int workerId, int projectId, DateTime bTime, int days)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Создаем новое действие для пользователя " + workerId + " для проекта " + projectId);
                AgrDataSet.WorkerRow drWorker = Db.Ds.Worker.FindById(workerId);
                if (drWorker == null)
                    return -1;
                AgrDataSet.ProjectsRow drProject;
                if (projectId == -1)
                    drProject = null;
                else
                {
                    drProject = Db.Ds.Projects.FindById(projectId);
                    if (drProject == null)
                        return -1;
                }
                AgrDataSet.ActionRow drAction = Db.Ds.Action.AddActionRow(drProject, drWorker, bTime, days, "", (int)type);
                if (Db.SavePlanners())
                    return drAction.Id;

                drAction.Delete();
                return -1;
            }
        }
        /// <summary>Меняем действие для пользователя</summary>
        public bool MFPlannerChangeAction(MFPlannerAction action)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Меняем действие для пользователя " + action.Id);
                AgrDataSet.ActionRow drAction = Db.Ds.Action.FindById(action.Id);
                if (drAction == null)
                    return false;
                drAction.Comment = action.Comment;
                drAction.Days = action.Days;
                drAction.Type = (int)action.Type;
                drAction.TimeBegin = action.TimeBegin;
                //drAction.TargetId = action.TargetId;
                return Db.SavePlanners();
            }
        }

        /// <summary>Удаляем действие</summary>
        public bool MFPlannerRemoveAction(int actionId)
        {
            lock (Db.Ds)
            {
                LogManager.LogInfo(IpClient(), "Удаляем действие " + actionId);

                var drAction = Db.Ds.Action.SingleOrDefault(a => a.Id == actionId);
                if (drAction == null)
                    return false;

                Db.Ds.Action.RemoveActionRow(drAction);
                if (Db.SavePlanners())
                    return true;
            }

            return false;
        }
        #endregion

        #endregion
    }

    //class LoggingErrorHandlerAttribute : Attribute, IErrorHandler
    //{
    //    #region поля
    //    const string unit = "LoggingErrorHandler";
    //    #endregion

    //    public void ProvideFault(Exception ex, MessageVersion version, ref Message fault)
    //    {
    //        LogManager.LogError(unit, string.Format("Version: {0} message: {1}", version, fault), ex);

    //    }
    //    public bool HandleError(Exception ex)
    //    {
    //        LogManager.LogError(unit, "Ошибка", ex);
    //        return false; // здесь можно ставить бряки, логировать и т.п.
    //    }
    //}
}
