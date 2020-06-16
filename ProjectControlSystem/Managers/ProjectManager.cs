using ArgDb;
using ArgLib.Logger;
using Microsoft.Win32;
using ProjectControlSystem.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace ProjectControlSystem.Managers
{
    public static class ProjectManager
    {
        #region Поля
        const string unit = "ProjectManager";

        static readonly ObservableCollection<Project> projects = new ObservableCollection<Project>(),
                                                      filtredProjects = new ObservableCollection<Project>();

        static readonly ObservableCollection<EventMessage> eventMessages = new ObservableCollection<EventMessage>();

        // флаг указывающий что нужно перечитать проекты
        static bool isNeedReloadProject = false;

        // Последнее обновление
        static DateTime dateLastRequest = DateTime.Now;

        // буфер по предыдущему состоянию
        static ProjectUndoBuffer prevState = null;

        static readonly BackgroundWorker workerReloadProjects = new BackgroundWorker();
        #endregion

        #region Свойства
        static ProjectApplication App { get { return Application.Current as ProjectApplication; } }

        public static ObservableCollection<Project> FiltredProjects { get { return filtredProjects; } }

        public static ObservableCollection<EventMessage> EventMessages { get { return eventMessages; } }

        public static IFilterChecker FilterChecker { get; set; }
        #endregion

        static ProjectManager()
        {
            workerReloadProjects.DoWork += workerReloadProjects_DoWork;
            workerReloadProjects.RunWorkerCompleted += workerReloadProjects_RunWorkerCompleted;
        }

        /// <summary>Проверка обновлений</summary>
        public static void TryCheckChangies()
        {
            try
            {
                bool isError;
                bool isReload;
                var date = DateTime.Now;

                LogManager.LogInfo(unit, string.Format("Загрузить обновления от: {0}", dateLastRequest));

                var changedProjects = ServiceManager.GetChangies(dateLastRequest, out isReload, out isError);
                if (isError)
                    return;

                // Запрос на перечитывание
                if (isReload)
                {

                    ReloadProjectsAsync();
                    return;
                }

                dateLastRequest = date;

                if (changedProjects == null)
                    return;

                var isNeedUpdate = false;

                foreach (var p in changedProjects)
                {
                    Project pExist = null;

                    lock (projects)
                        pExist = projects.SingleOrDefault(x => x.ProjectID == p.ProjectID);

                    if (pExist != null) // Если есть такой проект добавляем изменения
                    {
                        var changies = Project.GetChangies(pExist, p);
                        foreach (var c in changies)
                        {
                            isNeedUpdate = true;
                            ChangeProjectPropety(pExist.ProjectID, c.Key, c.Value, false);
                        }
                    }
                    else
                    {
                        isNeedUpdate = true;
                        projects.Add(p);

                        var msg = string.Format("Добавлен новый проект: '{0}' от {1} заказчик: {2} {3}/{4} ", p.ID, p.TimeBegin, p.Customer, p.Product, p.Options);
                        EventMessages.Insert(0, new EventMessage(DateTime.Now, p.ProjectID, msg));

                        // Ставим что новый проект
                        TrayIconManager.SetBaloonTip("Новый проект", 5);
                        TrayIconManager.SetMessageDefault();
                    }
                }

                if (isNeedUpdate)
                    UpdateProjects();
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка обновления", ex);
            }
        }

        /// <summary>Удаляем проект</summary>
        public static bool TryRemoveProject(int projectId)
        {
            LogManager.LogInfo(unit, string.Format("Удаляем проект с ID: {0}", projectId));

            if (!ServiceManager.RemoveProject(projectId))
                return false;

            lock (projects)
            {
                var project = projects.SingleOrDefault(x => x.ProjectID == projectId);
                if (project != null)
                    prevState = new ProjectUndoBuffer { IsDelete = true, Project = project.Clone() };
            }

            return true;
        }

        /// <summary>Загружаем проекты</summary>
        public static void ReloadProjectsAsync()
        {
            if (workerReloadProjects.IsBusy)
            {
                isNeedReloadProject = true;
                return;
            }

            isNeedReloadProject = false;
            workerReloadProjects.RunWorkerAsync();
        }

        /// <summary>Создать новый проект</summary>
        public static void CreateNewProject(Window owner = null)
        {
            var win = new NewProjectWindow() { Owner = owner, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            if (owner != null)
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (win.ShowDialog() != true)
                return;

            var projectId = -1;

            lock (projects)
            {
                var newProject = ServiceManager.CreateProject(win.NewId, win.NewCustomer, win.NewCustomerName, win.NewProduct, win.NewOptions, win.NewStartTime,
                    win.NewEndTime, win.NewComments, win.NewIsManagerSetPlanDate, win.NewPackageType);
                projects.Add(newProject);

                if (!string.IsNullOrEmpty(win.NewComments))
                    newProject.AddMessage(new AgrProjectComment(DateTime.Now, UserManager.Name, win.NewComments));

                projectId = newProject.ProjectID;
            }

            UpdateProjects();

            EventMessages.Insert(0, new EventMessage(DateTime.Now, projectId, "Создан новый проект"));
        }

        /// <summary>Создать новый проект</summary>
        public static void CreateNewProjectsFromFile(Window owner = null)
        {
            var dlg = new OpenFileDialog { Filter = "Файлы проектов из 1С (.csv)|*.csv", Multiselect = false, Title = "Выбор файла проекта" };
            if (dlg.ShowDialog(owner) != true)
                return;

            if (string.IsNullOrEmpty(dlg.FileName) || !File.Exists(dlg.FileName))
                return;

            var win = new NewProjectsFromFileWindow(dlg.FileName) { Owner = owner };
            if (win.ShowDialog() != true || win.Projects.Count == 0)
                return;

            lock (projects)
            {
                foreach (var p in win.Projects)
                {
                    var newProject = ServiceManager.CreateProject(p.Id, p.Customer, p.CustomerName, p.Product, p.Options, p.StartTime, p.EndTime, p.Comments, p.IsManagerSetPlanDate, p.PackageType);
                    projects.Add(newProject);

                    if (!string.IsNullOrEmpty(p.Comments))
                        newProject.AddMessage(new AgrProjectComment(DateTime.Now, UserManager.Name, p.Comments));

                    EventMessages.Insert(0, new EventMessage(DateTime.Now, newProject.ProjectID, "Создан новый проект"));
                }
            }

            UpdateProjects();
        }

        /// <summary>Изменение настроек проекта</summary>
        public static void ChangeProjectPropety(int projectId, ProjectPropertyType type, object value)
        {
            ChangeProjectPropety(projectId, type, value, true);
        }
        /// <summary>Изменение настроек проекта</summary>
        static void ChangeProjectPropety(int projectId, ProjectPropertyType type, object value, bool isSendToServer = false)
        {
            lock (projects)
            {
                var project = projects.SingleOrDefault(x => x.ProjectID == projectId);
                if (project == null)
                    return;

                // копия состояния
                Project projectPreviesState = null;
                if (isSendToServer)
                    projectPreviesState = project.Clone();

                project.SetValue(type, value);

                if (isSendToServer)
                {
                    // сохраняем изменения в буфер                  
                    prevState = new ProjectUndoBuffer { Project = projectPreviesState };

                    // Сохраняем на сервер
                    if (!ServiceManager.ChangeProject(project.GetAgrProject()))
                    {
                        MessageBox.Show("Не удалось сохранить изменения на сервер", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var sValue = "";
                if (value != null && !(value is IEnumerable))
                    sValue = value.ToString();

                var msg = string.Format("Изменения в проекте: '{0}' тип изменения: '{1}'  {2}", project.ID, Project.GetDescription(type), (string.IsNullOrEmpty(sValue) ? "" : "значение: '" + sValue + "'"));
                EventMessages.Insert(0, new EventMessage(DateTime.Now, project.ProjectID, msg));

                // если не на этом компе изменилось то ставим
                if (!isSendToServer)
                {
                    // Ставим что новый проект
                    TrayIconManager.SetBaloonTip(string.Format("Изменения в проекте: '{0}'", project.ID), 5);
                    TrayIconManager.SetMessageDefault();
                }
            }
        }

        /// <summary>Добавление Комментарий</summary>
        public static void AddComment(int projectId, string msg)
        {
            lock (projects)
            {
                var project = projects.SingleOrDefault(x => x.ProjectID == projectId);
                if (project == null || string.IsNullOrEmpty(msg))
                    return;

                var time = DateTime.Now;
                // Сохраняем на сервер
                if (!ServiceManager.AddCommentToProject(projectId, time, msg))
                {
                    MessageBox.Show("Не удалось сохранить изменения на сервер", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                project.AddMessage(new AgrProjectComment(time, UserManager.Name, msg));

                var msgEvent = string.Format("Сообщение от: {0} ({1}): ", UserManager.Name, UserManager.Group, msg);
                EventMessages.Insert(0, new EventMessage(DateTime.Now, project.ProjectID, msgEvent));
            }
        }

        /// <summary>Установка планируемой даты</summary>
        public static void SetPlanDate(ProjectPropertyType type, Project p, Window owner)
        {
            if (!p.IsManagerSetPlanDate)
                return;

            var time = DateTime.Now.Date.AddDays(-1);
            bool isNew = true;
            var title = string.Format("Изменение времени '{0}'", Project.GetDescription(type));

            switch (type)
            {
                #region ITO_G_Time_Plan
                case ProjectPropertyType.ITO_G_Time_Plan:
                    {
                        if (p.Time_ITO_G_Plan.HasValue)
                        {
                            time = p.Time_ITO_G_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
                #region ITO_E_Time_Plan
                case ProjectPropertyType.ITO_E_Time_Plan:
                    {
                        if (p.Time_ITO_E_Plan.HasValue)
                        {
                            time = p.Time_ITO_E_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
                #region ITO_R_Time_Plan
                case ProjectPropertyType.ITO_R_Time_Plan:
                    {
                        if (p.Time_ITO_R_Plan.HasValue)
                        {
                            time = p.Time_ITO_R_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion

                #region WH_G_Time_Plan
                case ProjectPropertyType.WH_G_Time_Plan:
                    {
                        if (p.Time_WH_G_Plan.HasValue)
                        {
                            time = p.Time_WH_G_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
                #region WH_E_Time_Plan
                case ProjectPropertyType.WH_E_Time_Plan:
                    {
                        if (p.Time_WH_E_Plan.HasValue)
                        {
                            time = p.Time_WH_E_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
                #region WH_R_Time_Plan
                case ProjectPropertyType.WH_R_Time_Plan:
                    {
                        if (p.Time_WH_R_Plan.HasValue)
                        {
                            time = p.Time_WH_R_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion

                #region OMTS_G_Time_Plan
                case ProjectPropertyType.OMTS_G_Time_Plan:
                    {
                        if (p.Time_OMTS_G_Plan.HasValue)
                        {
                            time = p.Time_OMTS_G_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
                #region OMTS_E_Time_Plan
                case ProjectPropertyType.OMTS_E_Time_Plan:
                    {
                        if (p.Time_OMTS_E_Plan.HasValue)
                        {
                            time = p.Time_OMTS_E_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion

                #region OTK_Time_Plan
                case ProjectPropertyType.OTK_Time_Plan:
                    {
                        if (p.Time_OTK_Plan.HasValue)
                        {
                            time = p.Time_OTK_Plan.Value;
                            isNew = false;
                        }

                        break;
                    }
                #endregion
            }

            var win = new ChangeDateWindow(time, false) { Owner = owner, Title = title };
            if (win.ShowDialog() != true || !win.NewDate.HasValue)
                return;

            ChangeProjectPropety(p.ProjectID, type, win.NewDate);

            // коментарий в зависимости от 
            if (isNew)
                AddComment(p.ProjectID, string.Format("Задали: '{0}' на: {1} - {2}", Project.GetDescription(type), win.NewDate.Value.ToString("dd/MM/yyyy"), win.NewComment));
            else
                AddComment(p.ProjectID, string.Format("Изменили: '{0}' c: {1} на: {2} - {3}", Project.GetDescription(type), time.ToString("dd/MM/yyyy"), win.NewDate.Value.ToString("dd/MM/yyyy"), win.NewComment));
        }

        /// <summary>Отмена последнего изменения</summary>
        public static bool Revert()
        {
            if (prevState == null || prevState.Project == null)
                return false;

            // просто изменения
            if (prevState.IsDelete == false)
            {
                // Сохраняем на сервер
                if (!ServiceManager.ChangeProject(prevState.Project.GetAgrProject()))
                {
                    MessageBox.Show("Не удалось сохранить изменения на сервер", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var msg = string.Format("Отмена изменений в проекте: '{0}'", prevState.Project.ID);
                EventMessages.Insert(0, new EventMessage(DateTime.Now, prevState.Project.ProjectID, msg));

                prevState = null;
            }
            else
            {
                EventMessages.Insert(0, new EventMessage(DateTime.Now, prevState.Project.ProjectID, "Востановление удаленного проекта не поддерживается"));
            }

            return true;
        }

        // Фильтры
        public static void UpdateProjects()
        {
            if (FilterChecker == null)
                return;

            filtredProjects.Clear();

            lock (projects)
            {
                foreach (var p in projects)
                {
                    if (FilterChecker.IsFilterPassed(p))
                        filtredProjects.Add(p);
                }
            }
        }
        public static void ResetFilter()
        {
            filtredProjects.Clear();

            lock (projects)
            {
                foreach (var p in projects)
                    filtredProjects.Add(p);
            }
        }

        #region Обрабочики событий

        // Загрузка проектов
        static void workerReloadProjects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // очищаем список 
            lock (projects)
            {
                foreach (var p in projects)
                    p.Dispose();

                projects.Clear();
            }

            // если надо перечитать то запрашиваем заново
            if (isNeedReloadProject)
            {
                isNeedReloadProject = false;
                workerReloadProjects.RunWorkerAsync();
            }

            // добавляем перечитанное
            var pList = e.Result as List<Project>;
            if (pList != null && pList.Count > 0)
            {
                lock (projects)
                {
                    foreach (var p in pList)
                        projects.Add(p);
                }
            }

            UpdateProjects();
        }
        static void workerReloadProjects_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ServiceManager.LoadProjects();
        }

        #endregion

        #region События

        #region ProjectChanged
        public static event EventHandler ProjectChanged;
        static void DoProjectChanged()
        {
            if (ProjectChanged != null)
                ProjectChanged(null, null);
        }
        #endregion

        //#region EventEmmit
        //public static event EventHandler<EventEmmitEventArgs> EventEmmit;
        //static void DoEventEmmit(DateTime t, string id, string m)
        //{
        //    if (EventEmmit != null)
        //        EventEmmit(null, new EventEmmitEventArgs(t, id, m));
        //}


        //#endregion

        #endregion

        #region Вспомогательные классы
        class ProjectUndoBuffer
        {
            #region Свойства
            public Project Project { get; set; }
            public bool IsDelete { get; set; }
            #endregion
        }
        #endregion
    }
}
