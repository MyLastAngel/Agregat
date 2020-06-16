using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ArgDb
{
    [ServiceContract]
    public interface IRLTTaskManagerService
    {
        /// <summary>Возвращаем пользователей</summary>
        [OperationContract]
        LUsers GetUsers();

        /// <summary>Проверка пользователя</summary>
        [OperationContract]
        bool Login(string name, string password);

        /// <summary>Возвращаем клиентов</summary>
        [OperationContract]
        LClients GetClients();

        /// <summary>Возвращаем товары</summary>
        [OperationContract]
        LProducts GetProducts();

        [OperationContract]
        bool RemoveProject(int projectId);

        /// <summary>Загружаем проекты</summary>
        [OperationContract]
        LProjects GetProjects(DateTime fromTime, int previousId);
        /// <summary>Загружаем проект</summary>
        [OperationContract]
        AgrProject GetProject(int id);

        /// <summary>Получить измененные проекты</summary>
        [OperationContract]
        LProjects GetChangies(DateTime fromDate);

        /// <summary>Создает новый проект и возвращает ID</summary>
        [OperationContract]
        int CreateProject(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate,
            string comments, string userName, bool isManagerSetPlanDate, string packageType);
        /// <summary>Проверка на совпадение</summary>
        [OperationContract]
        bool CheckIsExist(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate);
        /// <summary>Изменить состояние проекта</summary>
        [OperationContract]
        bool ChangeProject(AgrProject project);

        /// <summary>Добавляем коментарий к проекту</summary>
        [OperationContract]
        bool AddCommentToProject(int projectId, DateTime time, string message, string userName);
        /// <summary>Удаляем коментарий к проекту</summary>
        [OperationContract]
        bool RemoveCommentFromProject(int projectId, DateTime time, string message, string userName);
        /// <summary>Удаляем все коментарий к проекту</summary>
        [OperationContract]
        bool ClearCommentsFromProject(int projectId);

        [OperationContract]
        string ServerVersion();

        #region MFPlanner
        /* РАБОТНИКИ */
        /// <summary>Возвращает список работников</summary>
        [OperationContract]
        MFWorker[] MFPlannerGetWorkers(DateTime updateTime);
        /// <summary>Создает работника</summary>
        [OperationContract]
        int MFPlannerCreateWorker(string name, string second, int post, DateTime? endWorkTime);
        /// <summary>Меняет описание работника</summary>
        [OperationContract]
        bool MFPlannerChangeWorker(MFWorker worker);

        /* ДЕЙСТВИЯ */
        /// <summary>Возвращает список действий для работника за определенный период</summary>
        [OperationContract]
        MFPlannerAction[] MFPlannerGetActions(int workerId, DateTime? sDate, DateTime? eDate);

        /// <summary>Создаем новое действие для пользователя</summary>
        [OperationContract]
        int MFPlannerCreateAction(MFWorkerActionType type, int workerId, int projectId, DateTime bTime, int days);
        /// <summary>Меняем действие для пользователя</summary>
        [OperationContract]
        bool MFPlannerChangeAction(MFPlannerAction action);

        /// <summary>Удаляем действие</summary>
        [OperationContract]
        bool MFPlannerRemoveAction(int actionId);
        #endregion


    }
}
