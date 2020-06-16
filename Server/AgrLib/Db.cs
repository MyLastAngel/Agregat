using System;
using System.Collections.Generic;
using System.Linq;
using ArgLib.Logger;

namespace ArgDb
{
    public class Db
    {
        const string unit = "Db";
        public static readonly AgrDataSet Ds = new AgrDataSet();

        public static AgrStorage Storage = new AgrStorage();

        #region User
        private static bool _userLoaded;
        public static bool LoadUsers()
        {
            if (!_userLoaded)
            {
                _userLoaded = true;
                if (Storage.LoadUsers(Ds))
                {
                }
            }
            return true;
        }

        public static bool Login(string name, string password)
        {
            AgrDataSet.UserRow drUser = Ds.User.FindByName(name);
            if (drUser == null)
                return false;
            return drUser.Login == password;
        }

        private static readonly CUsers UserList = new CUsers();
        public static CUsers Users
        {
            get
            {
                return UserList;
            }
        }
        //public static bool AddGroup(string name)
        //{
        //    try
        //    {
        //        AgrDataSet.GroupRow drGroup = Ds.Group.AddGroupRow(name);
        //        return Storage.SaveUsers(Ds);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public static bool EditGroup(string oldName, string newName)
        //{
        //    try
        //    {
        //        AgrDataSet.GroupRow drGroup = Ds.Group.FindByName(oldName);
        //        drGroup.Name = newName;
        //        return Storage.SaveUsers(Ds);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public static bool AddUser(string user, string login, string group, uint right)
        {
            try
            {
                AgrDataSet.UserRow drUser = Ds.User.AddUserRow(user, login, group, right);
                return Storage.SaveUsers(Ds, false);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool EditUser(string oldName, string newName, string login, string group, uint right)
        {
            try
            {
                AgrDataSet.UserRow drUser = Ds.User.FindByName(oldName);
                drUser.Name = newName;
                drUser.Login = login;
                drUser.Group = group;
                drUser.Right = right;
                return Storage.SaveUsers(Ds, false);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion User

        #region Projects
        #region ProjectList
        private static bool _projectListLoaded;
        //public static bool LoadProjects()
        //{
        //    if (!_projectListLoaded)
        //    {
        //        _projectListLoaded = true;
        //        if (Storage.LoadProjectList(Ds))
        //        {
        //        }
        //    }
        //    return true;
        //}
        public static bool LoadProjects()
        {
            LoadClients(true);
            LoadProducts(true);
            if (!_projectListLoaded)
            {
                _projectListLoaded = true;
                if (Storage.LoadProjectList(Ds))
                {
                }
            }
            return true;
        }
        //public static bool LoadDate(int projectId)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion ProjectList

        #region Project
        private static readonly List<int> ProjectLoaded = new List<int>();
        public static bool LoadProject(int id)
        {
            LoadClients(true);
            LoadProducts(true);
            int idx = ProjectLoaded.BinarySearch(id);
            if (idx < 0)
            {
                if (!Storage.LoadProject(Ds, id))
                    return false;
                Storage.LoadComments(Ds, id);
                Storage.LoadRequest(Ds, id);
                ProjectLoaded.Insert(~idx, id);
            }
            return true;
        }

        //private static readonly CProjects ProjectList = new CProjects();
        public static LProjects ChangeProjects(DateTime? fromDate)
        {
            if (fromDate != null)
            {
                if (fromDate < _removeTime)
                    return new LProjects { IsReload = true };
            }
            return ClProjects.Projects(fromDate, ClProjects.StateTime.Changed, -1);
        }

        public static LProjects WorkProjects(DateTime? fromDate, int previousId)
        {
            return ClProjects.Projects(fromDate, ClProjects.StateTime.Worked, previousId);
        }

        //public static AgrProject Project(int id)
        //{
        //    AgrProject project = ProjectList.Projects.FirstOrDefault(s => s.ProjectId == id);
        //    if (project != null)
        //        project.Load();
        //    return project;
        //}

        public static AgrDataSet.ProjectRow AddProject(string name, string customerName,
            AgrDataSet.ClientRow drClient, AgrDataSet.ProductRow drProduct,
            string options, DateTime sDate, DateTime eDate, string comments, string user,
            bool isManagerSetPlanDate, string packageType)
        {
            try
            {
                object newId = Ds.Projects.Compute("MAX (Id)", "");
                int id = (newId.ToString() == "") ? 1 : ((int)newId + 1);
                AgrDataSet.ProjectsRow drProjects = Ds.Projects.AddProjectsRow(id, DateTime.Now, DateTime.MinValue);
                AgrDataSet.ClientProjectRow drClientProject = Ds.ClientProject.AddClientProjectRow(drClient, drProjects);
                AgrDataSet.ProjectProductRow drProjectProduct = Ds.ProjectProduct.AddProjectProductRow(drProjects, drProduct);

                //string[] opts = options.Split(',');
                //foreach (var opt in opts.Select(s => s.Trim()).Where(s => s.Length > 0))
                //{
                //    AgrDataSet.OptionRow drOption = Ds.Option.FindByName(opt);
                //    if (drOption == null)
                //        drOption = Ds.Option.AddOptionRow(opt, "");
                //    AgrDataSet.ProjectOptionRow drProjectOption = Ds.ProjectOption.
                //        AddProjectOptionRow(drProjects.Id, drProduct.Id, drOption);
                //}

                DateTime date = DateTime.MinValue;
                AgrDataSet.ProjectRow drProject = Ds.Project.AddProjectRow(drProjects,
                    name, customerName, isManagerSetPlanDate,
                    date, date, date, date, date, date, "", false, false, false,
                    date, date, date, date, date, date, false, false, false, date, date,
                    date, date, date, date,
                    sDate, eDate, date,
                    "", "", "", "", "", "", false, date, date, date, date, "", "",
                    date, date, date, false, false,
                    options, packageType, false);

                // Сораняем проект
                if (!Storage.SaveProject(Ds, id))
                {
                    drProjects.Delete();
                    return null;
                }

                // Сохраняем только существующий комментарий
                if (!string.IsNullOrEmpty(comments))
                {
                    var drUser = Ds.User.FindByName(user);

                    Ds.Comment.AddCommentRow(drProjects, DateTime.Now, comments, drUser);
                    Storage.SaveComment(Ds, id);
                }

                int idx = ProjectLoaded.BinarySearch(id);
                if (idx < 0)
                    ProjectLoaded.Insert(~idx, id);
                return drProject;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DateTime _removeTime = DateTime.Now;
        public static bool RemoveProject(int id)
        {
            AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(id);
            if (drProjects != null)
            {
                if (Storage.RemoveProject(id))
                {
                    _removeTime = DateTime.Now;
                    drProjects.Delete();
                    return true;
                }
                return false;
            }
            return true;
        }
        public static int CreateProject(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate,
            string comments, string userName, bool isManagerSetPlanDate, string packageType)
        {
            //var drsProject = (AgrDataSet.ProjectRow[])Ds.Project.Select("Name= '" + id + "'");
            //if (drsProject.Length != 0)
            //    return drsProject[0].Id;
            var drsProduct = (AgrDataSet.ProductRow[])Ds.Product.Select("Name= '" + product + "'");
            AgrDataSet.ProductRow drProduct = drsProduct.Length == 0 ? AddProduct(product) : drsProduct[0];
            var drsClient = (AgrDataSet.ClientRow[])Ds.Client.Select("Name= '" + customer + "'");
            AgrDataSet.ClientRow drClient = drsClient.Length == 0 ? AddClient(customer) : drsClient[0];
            AgrDataSet.ProjectRow drProject = AddProject(id, customerName, drClient, drProduct, options, sDate, eDate,
                comments, userName, isManagerSetPlanDate, packageType);
            return drProject == null ? -1 : drProject.Id;
        }

        public static bool CheckIsExist(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate)
        {
            var drsProject = (AgrDataSet.ProjectRow[])Ds.Project.Select(
                "Name = '" + id +
                "' AND CustomerName = '" + customerName +
                "' AND Options = '" + options +
                "' AND TimeBegin = '" + sDate +
                "' AND TimeEndPlaned = '" + eDate + "'");
            foreach (AgrDataSet.ProjectRow drProject in drsProject)
            {
                var drsProduct = (AgrDataSet.ProductRow[])Ds.Product.Select("Name= '" + product + "'");
                if (drsProduct.Length < 0)
                    continue;
                AgrDataSet.ProductRow drProduct = drsProduct[0];
                if (Ds.ProjectProduct.FindByProjectIdProductId(drProject.Id, drProduct.Id) == null)
                    continue;

                var drsClient = (AgrDataSet.ClientRow[])Ds.Client.Select("Name= '" + customer + "'");
                if (drsClient.Length < 0)
                    continue;
                AgrDataSet.ClientRow drClient = drsClient[0];

                if (Ds.ClientProject.FindByClientIdProjectId(drClient.Id, drProject.Id) == null)
                    continue;
                return true;
            }
            return false;
        }

        public static bool SetProject(AgrProject project)
        {
            try
            {
                AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(project.ProjectId);
                if (drProjects == null)
                {
                    LogManager.LogError(unit, "SetProject. Ошибка. Проект не найден: " + project.ProjectId);
                    return false;
                }
                drProjects.Date = DateTime.Now;

                drProjects.TimeEndActual = project.TimeEndActual == null
                    ? DateTime.MinValue
                    : (DateTime)project.TimeEndActual;

                AgrDataSet.ProjectRow[] drsProject = drProjects.GetProjectRows();
                AgrDataSet.ProjectRow drProject = drsProject[0];

                drProject.Name = project.ID;
                drProject.CustomerName = project.CustomerName;
                drProject.Options = project.Options;
                drProject.IsStop = project.IsStop;
                drProject.COM_Package_Type = project.COM_Package_Type;
                drProject.MF_Complete_Percentage = project.MF_Complete_Percentage;

                AgrDataSet.ClientProjectRow drClientProject = drProjects.GetClientProjectRows()[0];
                AgrDataSet.ClientRow drClient = drClientProject.ClientRow;
                if (drClient.Name != project.Customer)
                {
                    var drsClient = (AgrDataSet.ClientRow[])Ds.Client.Select("Name = '" + project.Customer + "'");
                    if (drsClient.Length > 0)
                        drClientProject.ClientId = drsClient[0].Id;
                    else
                    {
                        drClientProject.Delete();
                        object newId = Ds.Client.Compute("MAX (Id)", "");
                        int id = (newId.ToString() == "") ? 1 : ((int)newId + 1);

                        drClient = Ds.Client.AddClientRow(id, project.Customer);
                        Ds.ClientProject.AddClientProjectRow(drClient, drProjects);
                    }
                }

                //drClient.Description = project.CustomerName;
                AgrDataSet.ProjectProductRow drProjectProduct = drProjects.GetProjectProductRows()[0];
                AgrDataSet.ProductRow drProduct = drProjectProduct.ProductRow;
                if (drProduct.Name != project.Product)
                {
                    var drsProduct = (AgrDataSet.ProductRow[])Ds.Product.Select("Name = '" + project.Product + "'");
                    if (drsProduct.Length > 0)
                        drProjectProduct.ProductId = drsProduct[0].Id;
                    else
                    {
                        drProjectProduct.Delete();
                        object newId = Ds.Product.Compute("MAX (Id)", "");
                        int id = (newId.ToString() == "") ? 1 : ((int)newId + 1);

                        drProduct = Ds.Product.AddProductRow(id, project.Product, "");
                        Ds.ProjectProduct.AddProjectProductRow(drProjects, drProduct);
                    }
                }
                //drProduct.Name = project.Product;

                drProject.IsManagerSetPlanDate = project.IsManagerSetPlanDate;

                #region ITO

                #region G
                drProject.Time_ITO_G_Plan = project.Time_ITO_G_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_G_Plan;

                drProject.Time_ITO_G_Actual = project.Time_ITO_G_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_G_Actual;

                drProject.Is_ITO_G_NotNeed = project.Is_ITO_G_NotNeed;
                #endregion G

                #region E
                drProject.Time_ITO_E_Plan = project.Time_ITO_E_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_E_Plan;

                drProject.Time_ITO_E_Actual = project.Time_ITO_E_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_E_Actual;

                drProject.Is_ITO_E_NotNeed = project.Is_ITO_E_NotNeed;
                #endregion E

                #region R
                drProject.Time_ITO_R_Plan = project.Time_ITO_R_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_R_Plan;

                drProject.Time_ITO_R_Actual = project.Time_ITO_R_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_ITO_R_Actual;

                drProject.ITO_R_Mode = project.ITO_R_Mode;

                drProject.Is_ITO_R_NotNeed = project.Is_ITO_R_NotNeed;
                #endregion R

                #endregion ITO

                #region WH

                #region G
                drProject.Time_WH_G_Plan = project.Time_WH_G_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_G_Plan;

                drProject.Time_WH_G_Actual = project.Time_WH_G_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_G_Actual;

                drProject.Is_WH_G_NotNeed = project.Is_WH_G_NotNeed;
                #endregion G

                #region E
                drProject.Time_WH_E_Plan = project.Time_WH_E_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_E_Plan;

                drProject.Time_WH_E_Actual = project.Time_WH_E_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_E_Actual;

                drProject.Is_WH_E_NotNeed = project.Is_WH_E_NotNeed;
                #endregion E

                #region R
                drProject.Time_WH_R_Plan = project.Time_WH_R_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_R_Plan;

                drProject.Time_WH_R_Actual = project.Time_WH_R_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_R_Actual;

                drProject.Is_WH_R_NotNeed = project.Is_WH_R_NotNeed;
                #endregion R

                drProject.Time_WH_E_Requests_Create = project.Time_WH_E_Requests_Create == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_E_Requests_Create;
                drProject.Time_WH_G_Requests_Create = project.Time_WH_G_Requests_Create == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_WH_G_Requests_Create;
                #endregion WH

                #region OMTS
                drProject.Time_OMTS_E_Plan = project.Time_OMTS_E_Plan == null
                ? DateTime.MinValue
                : (DateTime)project.Time_OMTS_E_Plan;

                drProject.Time_OMTS_E_Actual = project.Time_OMTS_E_Actual == null
                ? DateTime.MinValue
                : (DateTime)project.Time_OMTS_E_Actual;

                drProject.Time_OMTS_G_Plan = project.Time_OMTS_G_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_OMTS_G_Plan;
                drProject.Time_OMTS_G_Actual = project.Time_OMTS_G_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_OMTS_G_Actual;
                #endregion OMTS

                drProject.TimeBegin = project.TimeBegin;
                drProject.TimeEndPlaned = project.TimeEndPlaned;
                drProject.Com_New_Time = project.Com_New_Time == null
                    ? DateTime.MinValue
                    : (DateTime)project.Com_New_Time;

                #region MF
                drProject.MF_Level = project.MF_Level;
                drProject.MF_Rama = project.MF_Rama;
                drProject.MF_Post = project.MF_Post;
                drProject.MF_Agregat = project.MF_Agregat;
                drProject.MF_SH_Place = project.MF_SH_Place;
                drProject.MF_SH = project.MF_SH;
                //drProject.MF_Test = project.MF_Test;
                drProject.MF_Time_Plan = project.MF_Time_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.MF_Time_Plan;

                drProject.MF_Time_Test_Actual = project.MF_Time_Test_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.MF_Time_Test_Actual;
                drProject.MF_Time = project.MF_Time == null
                    ? DateTime.MinValue
                    : (DateTime)project.MF_Time;
                drProject.MF_Collector = project.MF_Collector;
                drProject.MF_Complete_Percentage = project.MF_Complete_Percentage;
                #endregion MF

                #region OTK
                drProject.Time_OTK_Plan = project.Time_OTK_Plan == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_OTK_Plan;

                drProject.Time_OTK_G_Actual = project.Time_OTK_G_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_OTK_G_Actual;
                drProject.Time_OTK_E_Actual = project.Time_OTK_E_Actual == null
                    ? DateTime.MinValue
                    : (DateTime)project.Time_OTK_E_Actual;
                drProject.Is_OTK_G_NotNeed = project.Is_OTK_G_NotNeed;
                drProject.Is_OTK_E_NotNeed = project.Is_OTK_E_NotNeed;

                #endregion OTK

                // недокомплект
                var drsRequest = (AgrDataSet.RequestRow[])Ds.Request.Select("ProjectId = " + drProject.Id);
                foreach (AgrDataSet.RequestRow drRequest in drsRequest)
                    drRequest.Delete();

                foreach (AgrRequest request in project.Requests)
                {
                    Ds.Request.AddRequestRow(drProjects,
                                             request.Article, request.Name, (int)request.Type,
                                             request.DateComplete_Plan == null ? DateTime.MinValue : (DateTime)request.DateComplete_Plan,
                                             request.DateComplete == null ? DateTime.MinValue : (DateTime)request.DateComplete,
                                             request.TotalCount, request.ExistCount, request.IsCustomerMaterials);
                }
                Storage.SaveClientList(Ds);
                Storage.SaveProductList(Ds);
                return Storage.SaveProject(Ds, project.ProjectId) && Storage.SaveRequest(Ds, project.ProjectId);
            }
            catch (Exception exc)
            {
                LogManager.LogError(unit, "SetProject. Ошибка. " + exc.Message);
                return false;
            }
        }

        public static bool SetComment(int projectId, DateTime time, string message, string userName)
        {
            AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(projectId);
            if (drProjects == null)
            {
                LogManager.LogError(unit, "SetComment. Ошибка. Проект не найден: " + projectId);
                return false;
            }
            AgrDataSet.UserRow drUser = Ds.User.FindByName(userName);
            if (drUser == null)
            {
                LogManager.LogError(unit, "SetComment. Ошибка. Пользователь не найден: " + userName);
                return false;
            }
            drProjects.Date = DateTime.Now;
            AgrDataSet.ProjectRow[] drsProject = drProjects.GetProjectRows();
            if (drsProject.Length > 0)
            {
                bool res = Storage.AddComment(projectId, time, message, userName);
                if (res)
                {
                    Ds.Comment.AddCommentRow(drProjects, time, message, drUser);
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveComment(int projectId, DateTime time, string message, string userName)
        {
            AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(projectId);
            if (drProjects == null)
            {
                LogManager.LogError(unit, "RemoveComment. Ошибка. Проект не найден: " + projectId);
                return false;
            }
            AgrDataSet.UserRow drUser = Ds.User.FindByName(userName);
            if (drUser == null)
            {
                LogManager.LogError(unit, "RemoveComment. Ошибка. Пользователь не найден: " + userName);
                return false;
            }
            drProjects.Date = DateTime.Now;
            bool res = Storage.RemoveComment(projectId, time, message, userName);
            if (res)
            {
                var drsComment = (AgrDataSet.CommentRow[])Ds.Comment.Select("ProjectId = " + projectId
                    + " AND Time = '" + time
                    + "' AND User = '" + userName + "'");
                foreach (AgrDataSet.CommentRow drComment in drsComment)
                    drComment.Delete();
                return true;
            }
            return false;
        }

        public static bool ClearComment(int projectId)
        {
            AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(projectId);
            if (drProjects == null)
            {
                LogManager.LogError(unit, "ClearComment. Ошибка. Проект не найден: " + projectId);
                return false;
            }
            drProjects.Date = DateTime.Now;
            bool res = Storage.ClearComment(projectId);
            if (res)
            {
                var drsComment = (AgrDataSet.CommentRow[])Ds.Comment.Select("ProjectId = " + projectId);
                foreach (AgrDataSet.CommentRow drComment in drsComment)
                    drComment.Delete();
                return true;
            }
            return false;
        }
        public static bool SetRequest(int projectId, DateTime time, string message, string userName)
        {
            AgrDataSet.ProjectsRow drProjects = Ds.Projects.FindById(projectId);
            if (drProjects == null)
            {
                LogManager.LogError(unit, "SetRequest. Ошибка. Проект не найден: " + projectId);
                return false;
            }
            AgrDataSet.UserRow drUser = Ds.User.FindByName(userName);
            if (drUser == null)
            {
                LogManager.LogError(unit, "SetRequest. Ошибка. Пользователь не найден: " + userName);
                return false;
            }
            drProjects.Date = DateTime.Now;
            return Storage.AddRequest(projectId, time, message, userName);
        }
        #endregion Project

        #region Client
        private static bool _clientLoaded;
        public static bool LoadClients(bool projectState)
        {
            if (!projectState)
            {
                LoadProjects();
                LoadProducts(true);
            }
            if (!_clientLoaded)
            {
                _clientLoaded = true;
                if (Storage.LoadClientList(Ds, false))
                {
                }
            }
            //if (!projectState)
            //    foreach (int projectId in Projects)
            //        LoadProject(projectId);
            return true;
        }
        private static readonly ClClients ClientList = new ClClients();
        public static LClients Clients
        {
            get
            {
                return ClientList.Clients;
            }
        }
        public static AgrDataSet.ClientRow AddClient(string name)//, string cName)
        {
            try
            {
                ClientList.Clear();
                object newId = Ds.Client.Compute("MAX (Id)", "");
                int id = (newId.ToString() == "") ? 1 : ((int)newId + 1);
                AgrDataSet.ClientRow drClient = Ds.Client.AddClientRow(id, name);
                return Storage.SaveClientList(Ds) ? drClient : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool EditClient(int id, string name, string description)
        {
            ClientList.Clear();
            AgrDataSet.ClientRow drClient = Ds.Client.FindById(id);
            drClient.Name = name;
            //drClient.Description = description;
            Storage.SaveClientList(Ds);
            return true;
        }
        #endregion Client

        #region Product
        private static bool _productLoaded;
        public static bool LoadProducts(bool projectState)
        {
            //LoadOptions();
            if (!projectState)
            {
                LoadProjects();
                LoadClients(true);
            }
            if (!_productLoaded)
            {
                _productLoaded = true;
                if (Storage.LoadProductList(Ds, false))
                {
                }
            }
            //if (!projectState)
            //    foreach (int projectId in Projects)
            //        LoadProject(projectId);
            return true;
        }

        private static readonly ClProducts ProductList = new ClProducts();
        public static LProducts Products
        {
            get
            {
                return ProductList.Products;
            }
        }
        public static AgrDataSet.ProductRow AddProduct(string name)
        {
            try
            {
                object newId = Ds.Product.Compute("MAX (Id)", "");
                int id = (newId.ToString() == "") ? 1 : ((int)newId + 1);
                AgrDataSet.ProductRow drProduct = Ds.Product.AddProductRow(id, name, "");
                return Storage.SaveProductList(Ds) ? drProduct : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool EditProduct(int id, string name, string description)
        {
            AgrDataSet.ProductRow drProduct = Ds.Product.FindById(id);
            drProduct.Name = name;
            drProduct.Description = description;
            Storage.SaveProductList(Ds);
            return true;
        }
        #endregion Product

        //#region Option
        //private static bool _optionLoaded;
        //public static bool LoadOptions()
        //{
        //    if (!_optionLoaded)
        //    {
        //        _optionLoaded = true;
        //        if (Storage.LoadOptionsList(Ds))
        //        {
        //        }
        //    }
        //    return true;
        //}
        //private static readonly ClOptions OptionList = new ClOptions();
        //public static LOptions Options
        //{
        //    get
        //    {
        //        return OptionList.Options;
        //    }
        //}
        //#endregion Option
        #endregion Projects

        #region Planner
        private static bool _plannerLoaded;
        public static bool LoadPlanners()
        {
            if (!_plannerLoaded)
            {
                _plannerLoaded = true;
                return Storage.LoadPlanners(Ds);
            }
            return true;
        }
        public static bool SavePlanners()
        {
            return Storage.SavePlanners(Ds);
        }
        #endregion Planner
    }

    #region Projects

    #region Client
    [Serializable]
    public class Client
    {
        #region Свойства
        public int Id { set; get; }
        public string Name { set; get; }
        public string Decription { set; get; }

        public List<int> Projects { set; get; }
        #endregion

        public Client(int id, string name, IEnumerable<int> projects = null)
        {
            Id = id;
            Name = name;
            //Decription = decription;

            if (projects != null)
                Projects = new List<int>(projects);
        }


        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class LClients : List<Client>
    {
    }

    public class ClClients
    {
        private LClients _clients;
        public LClients Clients
        {
            get
            {
                if (_clients == null)
                {
                    _clients = new LClients();
                    if (Db.LoadClients(false))
                    {
                        foreach (AgrDataSet.ClientRow drClient in Db.Ds.Client.Select())
                        {
                            var client = new Client(drClient.Id, drClient.Name, drClient.GetClientProjectRows().Select(s => s.ProjectId));
                            _clients.Add(client);
                        }
                    }
                }
                return _clients;
            }
        }
        public void Clear()
        {
            _clients = null;
        }
    }
    #endregion Client

    //#region Option
    //[Serializable]
    //public class Option
    //{
    //    public Option(string name, string decription)
    //    {
    //        Name = name;
    //        Decription = decription;
    //    }

    //    public string Name { set; get; }
    //    public string Decription { set; get; }
    //}
    //[Serializable]
    //public class LOptions : List<Option>
    //{
    //}
    //public class ClOptions
    //{
    //    private LOptions _options;
    //    public LOptions Options
    //    {
    //        get
    //        {
    //            //if (_options == null)
    //            {
    //                _options = new LOptions();
    //                if (Db.LoadOptions())
    //                {
    //                    foreach (AgrDataSet.OptionRow drOption in Db.Ds.Option.Select())
    //                    {
    //                        var option = new Option(drOption.Name, drOption.Description);
    //                        _options.Add(option);
    //                    }
    //                }
    //            }
    //            return _options;
    //        }
    //    }
    //}
    //#endregion Option

    #region Product
    [Serializable]
    public class Product
    {
        #region Свойства
        public int Id { set; get; }
        public string Name { set; get; }
        public string Decription { set; get; }
        #endregion

        public Product(int id, string name, string decription)
        {
            Id = id;
            Name = name;
            Decription = decription;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class LProducts : List<Product>
    {
    }

    public class ClProducts
    {
        private LProducts _products;
        public LProducts Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new LProducts();
                    if (Db.LoadProducts(false))
                    {
                        foreach (AgrDataSet.ProductRow drProduct in Db.Ds.Product.Select())
                        {
                            var product = new Product(drProduct.Id, drProduct.Name, drProduct.Description);
                            _products.Add(product);
                        }
                    }
                }
                return _products;
            }
        }
        public void Clear()
        {
            _products = null;
        }
    }
    #endregion Product
    #endregion Projects
}
