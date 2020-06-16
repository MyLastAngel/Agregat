using ArgDb;
using ArgDb.Managers;
using ArgLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;

namespace ProjectControlSystem.Managers
{
    internal static class ServiceManager
    {
        #region Поля
        const string unit = "ServiceManager";
        static IRLTTaskManagerService service = null;
        #endregion

        #region Свойства
        public static bool IsConnected { get { return service != null; } }
        #endregion

        static ServiceManager()
        {
        }

        public static bool CreateClient()
        {
            if (service != null)
                return true;

            try
            {
                var host = HostManager.GetClientHost(ProjectConfiguration.Address);

                var binding = new NetTcpBinding
                {
                    Name = "my_binding",
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    MaxBufferSize = UInt16.MaxValue * 16
                };
                binding.Security.Mode = SecurityMode.None;
                binding.TransferMode = TransferMode.Buffered;

                //var b = new BasicHttpBinding(BasicHttpSecurityMode.None);

                var channelFactory = new ChannelFactory<IRLTTaskManagerService>(binding);
                var eP = new EndpointAddress(host);
                service = channelFactory.CreateChannel(eP);

                channelFactory.Closed += channelFactory_Closed;

                LogManager.LogInfo(unit, string.Format("Связь с каналом '{0}' установлена", ProjectConfiguration.Address));
                DoConnected();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка связи с каналом: {0} ", ProjectConfiguration.Address), ex);

                CloseClient();

                return false;
            }
        }
        public static void CloseClient()
        {
            service = null;

            DoDisconnected();

            LogManager.LogInfo(unit, string.Format("Связь с каналом '{0}' потеряна", ProjectConfiguration.Address));
        }

        /// <summary>Возвращает версию сервера</summary>
        public static string GetServerVersion()
        {
            if (!CreateClient())
                return null;

            try
            {
                return service.ServerVersion();
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка версии сервера"), ex);

                CloseClient();
            }

            return null;
        }

        /// <summary>Список заказчиков для производства</summary>
        public static LClients GetClients()
        {
            if (!CreateClient())
                return null;

            try
            {
                var clients = service.GetClients();
                LogManager.LogInfo(unit, string.Format("Загружено заказчиков: {0}", clients.Count));

                return clients;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка загрузки заказчиков"), ex);

                CloseClient();
            }

            return null;
        }

        /// <summary>Получение списка пользователей</summary>
        public static LUsers GetUsers()
        {
            if (!CreateClient())
                return null;

            try
            {
                var users = service.GetUsers();
                LogManager.LogInfo(unit, string.Format("Загружено пользователей: {0}", users.Count));

                return users;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка загрузки пользователей:"), ex);

                CloseClient();
            }

            return null;
        }

        /// <summary>Регистрация пользователя</summary>
        public static bool Login(string name, string password)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.Login(name, password);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка Login для '{0}'", name), ex);

                CloseClient();

                return false;
            }
        }

        /// <summary>Создание нового проекта</summary>
        public static Project CreateProject(string id, string cutomer, string customerName, string product, string options, DateTime sDate, DateTime eDate, string comments, bool isManagerSetPlanDate, string packageType)
        {
            if (!CreateClient())
                return null;

            try
            {
                var index = service.CreateProject(id, cutomer, customerName, product, options, sDate, eDate, comments, UserManager.Name, isManagerSetPlanDate, packageType);
                var newProject = new Project(id, cutomer, customerName, product, options, sDate, eDate)
                {
                    ProjectID = index,
                    IsManagerSetPlanDate = isManagerSetPlanDate
                };

                LogManager.LogInfo(unit, string.Format("Проект '{0}' для '{1}' '{2}' ({3}) успешно создан", id, cutomer, product, options));

                return newProject;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка создания проекта '{0}' для '{1}' '{2}' ({3})", id, cutomer, product, options), ex);

                CloseClient();
            }

            return null;
        }
        /// <summary>Проверка на существование проекта</summary>
        public static bool CheckIsExist(string id, string cutomer, string customerName, string product, string options, DateTime sDate, DateTime eDate, out bool exist)
        {
            if (!CreateClient())
            {
                exist = false;
                return false;
            }

            try
            {
                exist = service.CheckIsExist(id, cutomer, customerName, product, options, sDate, eDate);
                return true;
            }
            catch (Exception ex)
            {
                exist = false;
                LogManager.LogError(unit, string.Format("Ошибка проверка существования проекта '{0}' для '{1}' '{2}' ({3})", id, cutomer, product, options), ex);
                CloseClient();
                return false;
            }
        }

        /// <summary>Удаляем проект</summary>
        public static bool RemoveProject(int projectId)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.RemoveProject(projectId);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Удаления проекта '{0}' ", projectId), ex);

                CloseClient();
            }

            return false;
        }

        /// <summary>Загружаем проекты</summary>
        public static List<Project> LoadProjects(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var result = new List<Project>();

            if (!CreateClient())
                return result;

            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddDays(-ProjectConfiguration.LoadDays);

            try
            {
                var previousId = -1;
                while (true)
                {
                    var projects = service.GetProjects(fromDate.Value, previousId);
                    if (projects == null)
                        break;

                    result.AddRange(projects.Select(Project.GetProject));
                    if (projects.Count < 10)
                        break;
                    var previous = projects.Last();

                    // ограничение по времени
                    if (toDate.HasValue && previous.TimeBegin > toDate.Value)
                        break;

                    previousId = previous.ProjectId;
                    //fromDate = previous.TimeEndActual ?? DateTime.MinValue;

                    //Console.WriteLine(previous.ProjectId + " : " + previous.TimeEndActual);
                }

                LogManager.LogInfo(unit, string.Format("Загружено проектов с: {0} в количестве: {1}", fromDate.Value, result.Count));
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка загрузки проектов с: {0}", fromDate.Value), ex);

                CloseClient();
            }

            return result;
        }
        /// <summary>Загружаем проект по ID</summary>
        public static Project LoadProject(int id)
        {
            if (!CreateClient())
                return null;

            try
            {
                var p = service.GetProject(id);
                if (p != null)
                    return Project.GetProject(p);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка загрузки проекта id: {0}", id), ex);

                CloseClient();
            }

            return null;
        }

        /// <summary> Получение изменений</summary>
        public static List<Project> GetChangies(DateTime updateTime, out bool isReload, out bool isError)
        {
            var result = new List<Project>();
            isReload = isError = false;

            if (!CreateClient())
            {
                isReload = false;
                isError = true;
                return result;
            }

            try
            {
                var projects = service.GetChangies(updateTime);
                if (projects != null)
                {
                    isReload = projects.IsReload;
                    result.AddRange(projects.Select(Project.GetProject));
                    if (projects.Count > 0)
                        LogManager.LogInfo(unit,
                            string.Format("Загружено изменений с: {0} в количестве: {1}", updateTime,
                            projects.Count));
                }
            }
            catch (Exception ex)
            {
                isReload = false;
                isError = true;
                LogManager.LogError(unit, string.Format("Ошибка загрузки изменений {0}", updateTime), ex);

                CloseClient();
            }

            return result;
        }

        /// <summary>Добавляем Комментарий к проекту</summary>
        public static bool AddCommentToProject(int projectId, DateTime time, string message)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.AddCommentToProject(projectId, time, message, UserManager.Name);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка добавления сообщения в проект", ex);

                DoDisconnected();
                return false;
            }
        }
        /// <summary>Удаляем коментарий к проекту</summary>
        public static bool RemoveCommentFromProject(int projectId, DateTime time, string message, string userName)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.RemoveCommentFromProject(projectId, time, message, userName);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка удаления сообщения: '{0}' из проекта", message), ex);

                DoDisconnected();
                return false;
            }
        }
        /// <summary>Удаляем все коментарий к проекту</summary>
        public static bool ClearCommentsFromProject(int projectId)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.ClearCommentsFromProject(projectId);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка удаления всех сообщений из проекта", ex);

                DoDisconnected();
                return false;
            }
        }

        /// <summary>Сохранение изменения проекта на сервере</summary>
        public static bool ChangeProject(AgrProject project)
        {
            if (!CreateClient() || project == null)
                return false;

            try
            {
                return service.ChangeProject(project);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка изменения для '{0}'", project.ProjectId), ex);

                CloseClient();

                return false;
            }
        }

        #region MFPlanner

        #region Работники
        /// <summary>Возвращает список работников</summary>
        public static MFWorker[] MFPlannerGetWorkers()
        {
            if (!CreateClient())
                return null;
            try
            {
                return service.MFPlannerGetWorkers(new DateTime());
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка получения списка работников"), ex);
                CloseClient();
                return null;
            }
        }

        /// <summary> Получение изменений</summary>
        public static List<MFWorker> GetWorkerChangies(DateTime updateTime, out bool isReload, out bool isError)
        {
            if (CreateClient())
            {
                try
                {
                    MFWorker[] workers = service.MFPlannerGetWorkers(updateTime);
                    isError = false;
                    if (workers != null)
                    {
                        isReload = true;
                        LogManager.LogInfo(unit,
                            string.Format("Загружен новый список сотрудников с: {0} в количестве: {1}", updateTime,
                            workers.Length));
                        return workers.ToList();
                    }
                    isReload = false;
                }
                catch (Exception ex)
                {
                    isReload = false;
                    isError = true;
                    LogManager.LogError(unit, string.Format("Ошибка загрузки изменений {0}", updateTime), ex);

                    CloseClient();
                }
            }
            else
            {
                isReload = false;
                isError = true;
            }
            return null;
        }

        /// <summary>Создает работника</summary>
        public static MFWorker MFPlannerCreateWorker(string name, string secondName, int post, DateTime? endWorkTime)
        {
            if (!CreateClient())
                return null;

            try
            {
                int id = service.MFPlannerCreateWorker(name, secondName, post, endWorkTime);
                var newWorker = new MFWorker(id, name, secondName, post)
                {
                    EndWorkTime = endWorkTime
                };

                LogManager.LogInfo(unit, string.Format("Сотрудник №{0} '{1}' '{2}') успешно добавлен", newWorker.Id, newWorker.Name, newWorker.SecondName));

                return newWorker;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка добавления сотрудника '{0}' '{1}'", name, secondName), ex);

                CloseClient();
            }

            return null;
        }
        //public static bool MFPlannerCreateWorker(MFWorker worker)
        //{
        //    if (!CreateClient())
        //        return false;

        //    try
        //    {
        //        return service.MFPlannerCreateWorker();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.LogError(unit, string.Format("Ошибка создания работника: '{0} {1}'", worker.SecondName, worker.Name), ex);

        //        CloseClient();

        //        return false;
        //    }
        //}

        /// <summary>Меняет описание работника</summary>
        public static bool MFPlannerChangeWorker(MFWorker worker)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.MFPlannerChangeWorker(worker);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка изменения работника: [{0}] '{1} {2}'", worker.Id, worker.SecondName, worker.Name), ex);

                CloseClient();

                return false;
            }
        }
        #endregion

        #region Действия
        /// <summary>Возвращает список действий для работника за определенный период</summary>
        public static MFPlannerAction[] MFPlannerGetActions(int workerId, DateTime? sDate, DateTime? eDate)
        {
            if (!CreateClient())
                return null;

            try
            {
                return service.MFPlannerGetActions(workerId, sDate, eDate);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка получения действий работника: [{0}]  с '{1}' по '{2}'", workerId, sDate, eDate), ex);

                CloseClient();

                return null;
            }
        }

        /// <summary>Создаем новое действие для пользователя</summary>
        public static MFPlannerAction MFPlannerCreateAction(MFWorkerActionType type, int workerId, int projectId, DateTime bTime, int days, string comment)
        {
            if (!CreateClient())
                return null;

            try
            {

                int id = service.MFPlannerCreateAction(type, workerId, projectId, bTime, days);
                var newAction = new MFPlannerAction(type, id, workerId, projectId)
                {
                    TimeBegin = bTime,
                    Days = days,
                    Comment = comment
                };
                return newAction;
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка создания действия"), ex);

                CloseClient();

                return null;
            }
        }
        /// <summary>Меняем действие для пользователя</summary>
        public static bool MFPlannerChangeAction(MFPlannerAction action)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.MFPlannerChangeAction(action);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка изменения действия: [{0}]", action.Id), ex);

                CloseClient();

                return false;
            }
        }

        /// <summary>Удаляем действие</summary>
        public static bool MFPlannerRemoveAction(int actionId)
        {
            if (!CreateClient())
                return false;

            try
            {
                return service.MFPlannerRemoveAction(actionId);
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, string.Format("Ошибка удаления действия: [{0}]", actionId), ex);

                CloseClient();

                return false;
            }
        }

        #endregion

        #endregion

        #region Обработчики событий
        static void channelFactory_Closed(object sender, EventArgs e)
        {
            CloseClient();

            var channelFactory = sender as ChannelFactory;
            if (channelFactory != null)
                channelFactory.Closed -= channelFactory_Closed;
        }
        #endregion

        #region События

        #region Disconnected
        public static event EventHandler Disconnected;
        static void DoDisconnected()
        {
            if (Disconnected != null)
                Disconnected(null, null);
        }
        #endregion

        #region Connected
        public static event EventHandler Connected;
        static void DoConnected()
        {
            if (Connected != null)
                Connected(null, null);
        }
        #endregion

        #endregion
    }
}
