using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ArgDb;
using ArgLib.Logger;

namespace ProjectControlSystem.Managers
{
    public static class MFPlannerManager
    {
        #region Поля
        const string unit = "MFPlannerManager";
        static readonly List<MFWorker> workers = new List<MFWorker>();
        static readonly List<MFPlannerAction> actions = new List<MFPlannerAction>();

        // Последнее обновление
        static DateTime dateLastRequest = DateTime.Now;
        // флаг указывающий что нужно перечитать сотрудников
        static bool isNeedReloadWorker = false,
                    isNeedReloadActions = false;

        // что запрашивали
        static ReloadActionArgs rActionCache = null;

        static readonly BackgroundWorker workerReloadWorkers = new BackgroundWorker(),
                                         workerReloadActions = new BackgroundWorker();

        // 
        static readonly ObservableCollection<EventMessage> eventMessages = new ObservableCollection<EventMessage>();
        #endregion

        #region Свойства
        public static ObservableCollection<EventMessage> EventMessages { get { return eventMessages; } }
        #endregion

        static MFPlannerManager()
        {
            workerReloadWorkers.DoWork += workerReloadWorkers_DoWork;
            workerReloadWorkers.RunWorkerCompleted += workerReloadWorkers_RunWorkerCompleted;

            workerReloadActions.DoWork += workerReloadActions_DoWork;
            workerReloadActions.RunWorkerCompleted += workerReloadActions_RunWorkerCompleted;
        }

        /* Работники */
        /// <summary>Возвращает список сотрудников</summary>
        public static List<MFWorker> GetWorkers(bool isIncludeEndWorkWorkers, bool isRead = false)
        {
            if (isRead)
            {
                lock (workers)
                    workers.Clear();

                var rWorker = ServiceManager.MFPlannerGetWorkers();
                if (rWorker != null)
                {
                    // Сначало не уволенные
                    lock (workers)
                        workers.AddRange(rWorker.OrderBy(x => !x.EndWorkTime.HasValue).ThenBy(x => x.Post));
                }

                DoWorkersChanged();
            }

            if (isIncludeEndWorkWorkers)
                return workers;
            else
                return workers.Where(x => !x.EndWorkTime.HasValue).ToList();
        }
        /// <summary>Загружаем сотрудников</summary>
        public static void ReloadWorkersAsync()
        {
            if (workerReloadWorkers.IsBusy)
            {
                isNeedReloadWorker = true;
                return;
            }

            isNeedReloadWorker = false;
            workerReloadWorkers.RunWorkerAsync();
        }

        /// <summary>Создать новый проект</summary>
        public static void CreateNewWorker(MFWorker w)
        {
            CreateNewWorker(w.Name, w.SecondName, w.Post, w.EndWorkTime);
        }
        /// <summary>Создать новый проект</summary>
        public static void CreateNewWorker(string name, string secondName, int post, DateTime? endWorkTime)
        {
            int workerId;

            lock (workers)
            {
                MFWorker newWorker = ServiceManager.MFPlannerCreateWorker(name, secondName, post, endWorkTime);
                workers.Add(newWorker);

                //if (!string.IsNullOrEmpty(win.NewComments))
                //    newProject.AddMessage(new AgrProjectComment(DateTime.Now, UserManager.Name, win.NewComments));

                workerId = newWorker.Id;
            }

            EventMessages.Insert(0, new EventMessage(DateTime.Now, workerId, "Добавлен новый работник"));

            DoWorkersChanged();
        }

        /// <summary>Меняет описание работника</summary>
        public static bool MFPlannerChangeWorker(MFWorker worker)
        {
            if (ServiceManager.MFPlannerChangeWorker(worker))
            {
                ReloadWorkersAsync();
                return true;
            }

            return false;
        }

        /* Действия */
        /// <summary>Возвращает список действий для сотрудника</summary>
        public static List<MFPlannerAction> GetActions()
        {
            return actions;
        }
        /// <summary>Асинхронное обновление спсика действий</summary>
        public static void ReloadActionsAsync(DateTime? dateStart, DateTime? dateEnd)
        {
            var a = new ReloadActionArgs { DateEnd = dateEnd, DateStart = dateStart };

            if (workerReloadActions.IsBusy)
            {
                rActionCache = a;
                isNeedReloadActions = true;
                return;
            }

            isNeedReloadActions = false;
            workerReloadActions.RunWorkerAsync(a);
        }

        /// <summary>Меняет описание работника</summary>
        public static bool MFPlannerChangeAction(MFPlannerAction action)
        {
            if (ServiceManager.MFPlannerChangeAction(action))
                return true;

            return false;
        }

        public static void CreateNewAction(MFWorkerActionType type, int workerId, int projectId, DateTime bTime, int workTime, string comment = "")
        {
            int actionId = -1;
            MFPlannerAction newAction = null;

            lock (actions)
                newAction = ServiceManager.MFPlannerCreateAction(type, workerId, projectId, bTime, workTime, comment);

            if (newAction != null)
            {
                actionId = newAction.Id;
                lock (actions)
                {
                    if (!actions.Any(x => x.Id == newAction.Id))
                        actions.Add(newAction);
                }

                EventMessages.Insert(0, new EventMessage(DateTime.Now, actionId, "Добавлено новое действие"));
                DoActionsChanged();
            }
        }
        /// <summary>Удаляем действие</summary>
        public static bool RemoveAction(MFPlannerAction action)
        {
            if (action != null && ServiceManager.MFPlannerRemoveAction(action.Id))
                return true;

            return false;
        }

        #region Обрабочики событий
        // Загрузка работников
        static void workerReloadWorkers_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // очищаем список 
            lock (workers)
            {
                //foreach (MFWorker p in workers)
                //    p.Dispose();
                workers.Clear();
            }

            // если надо перечитать то запрашиваем заново
            if (isNeedReloadWorker)
            {
                isNeedReloadWorker = false;
                workerReloadWorkers.RunWorkerAsync();
            }

            // добавляем перечитанное
            var pList = e.Result as MFWorker[];
            if (pList != null && pList.Length > 0)
            {
                lock (workers)
                {
                    foreach (MFWorker p in pList)
                        workers.Add(p);
                }
            }

            DoWorkersChanged();
        }
        static void workerReloadWorkers_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ServiceManager.MFPlannerGetWorkers();
        }

        // загрузка действий
        static void workerReloadActions_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as List<MFPlannerAction>;
            if (result != null)
            {
                lock (actions)
                {
                    actions.Clear();
                    actions.AddRange(result);
                }
            }

            DoActionsChanged();

            // Если требуется дочитывание
            if (isNeedReloadActions && rActionCache != null)
            {
                ReloadActionsAsync(rActionCache.DateStart, rActionCache.DateEnd);

                isNeedReloadActions = false;
                rActionCache = null;
            }
        }
        static void workerReloadActions_DoWork(object sender, DoWorkEventArgs e)
        {
            var a = e.Argument as ReloadActionArgs;
            if (a != null)
            {
                var result = new List<MFPlannerAction>();

                var wIds = new int[0];
                lock (workers)
                    wIds = workers.Select(x => x.Id).ToArray();

                foreach (var id in wIds)
                {
                    var data = ServiceManager.MFPlannerGetActions(id, a.DateStart, a.DateEnd);
                    if (data != null)
                        result.AddRange(data);
                }

                e.Result = result;
            }
        }
        #endregion

        #region События

        #region WorkersChanged
        public static event EventHandler WorkersChanged;
        static void DoWorkersChanged()
        {
            if (WorkersChanged != null)
                WorkersChanged(null, null);
        }
        #endregion

        #region ActionsChanged
        public static event EventHandler ActionsChanged;
        static void DoActionsChanged()
        {
            if (ActionsChanged != null)
                ActionsChanged(null, null);
        }
        #endregion

        #endregion

        #region Вспомогательные классы
        class ReloadActionArgs
        {
            #region Свойства
            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
            #endregion
        }
        #endregion

        ///// <summary>Проверка обновлений</summary>
        //public static void TryCheckChangies()
        //{
        //    try
        //    {
        //        bool isError;
        //        bool isReload;
        //        var date = DateTime.Now;

        //        LogManager.LogInfo(unit, string.Format("Загрузить обновления от: {0}", dateLastRequest));

        //        var loadWorkers = ServiceManager.GetWorkerChangies(dateLastRequest, out isReload, out isError);
        //        if (isError)
        //            return;

        //        // Запрос на перечитывание
        //        if (isReload)
        //        {
        //            ReloadWorkersAsync();
        //            return;
        //        }
        //        dateLastRequest = date;
        //        if (loadWorkers == null)
        //            return;

        //        //foreach (var w in loadWorkers)
        //        //{
        //        //    MFWorker wExist = null;
        //        //    lock (workers)
        //        //        wExist = workers.SingleOrDefault(x => x.Id == w.Id);

        //        //    if (wExist != null) // Если есть такой проект добавляем изменения
        //        //    {
        //        //        var changies = Project.GetChangies(pExist, p);
        //        //        foreach (var c in changies)
        //        //        {
        //        //            isNeedUpdate = true;
        //        //            ChangeProjectPropety(pExist.ProjectID, c.Key, c.Value, false);
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        isNeedUpdate = true;
        //        //        projects.Add(p);

        //        //        var msg = string.Format("Добавлен новый проект: '{0}' от {1} заказчик: {2} {3}/{4} ", p.ID, p.TimeBegin, p.Customer, p.Product, p.Options);
        //        //        EventMessages.Insert(0, new EventMessage(DateTime.Now, p.ProjectID, msg));

        //        //        // Ставим что новый проект
        //        //        TrayIconManager.SetBaloonTip("Новый проект", 5);
        //        //        TrayIconManager.SetMessageDefault();
        //        //    }
        //        //}

        //        workers.Clear();
        //        foreach (MFWorker p in loadWorkers)
        //            workers.Add(p);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.LogError(unit, "Ошибка обновления", ex);
        //    }
        //}
    }

    public class EditEventArgs : EventArgs
    {
        public enum EditAction
        {
            Add,
            Remove
        }
        public EditAction Action { set; get; }
        public int IdIdx { set; get; }
        public EditEventArgs(EditAction action, int id)
        {
            Action = action;
            IdIdx = id;
        }
    }
}
