using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectControlSystem
{
    public enum ProjectViewMode
    {
        Unknown = 0,
        /// <summary>Главное окно</summary>
        Main = 1,
        /// <summary>Коммерческий отдел</summary>
        Commercial = 2,
        /// <summary>Отдел Инженерно Технического Обеспечения</summary>
        ITO = 4,
        /// <summary>Склад</summary>
        Warehouse = 8,
        /// <summary>Отдел Материально Технического Снабжения</summary>
        OMTS = 16,
        /// <summary>Производство</summary>
        Manufacture = 32,
        /// <summary>Отдел Технического Контроля</summary>
        OTK = 64,
    }

    /// <summary>Состояние</summary>
    public enum ProjectState
    {
        Ok,
        Warning,
        Error,
        Stop
    }

    //public enum UserRight
    //{
    //    None = 0,
    //    AddNewProject = 1,
    //    EditCommercial = 2,
    //    EditITO = 4,
    //    EditWarehouse = 8,
    //    EditOMTS = 16,
    //    EditManufacture = 32,
    //    EditOTK = 64,
    //    All = AddNewProject | EditCommercial | EditITO | EditManufacture | EditOMTS | EditOTK | EditWarehouse,
    //}

    public enum ProjectPropertyType
    {
        /// <summary>ID Проекта</summary>
        ID,
        /// <summary>Контрагент</summary>
        Customer,
        /// <summary>Заказчик</summary>
        CustomerName,
        /// <summary>Изделие</summary>
        Product,
        /// <summary>Опции</summary>
        Options,

        /// <summary>В стопе</summary>
        IsStop,

        /// <summary>Менеджеры сами устанавливают планируемые даты</summary>
        IsManagerSetPlanDate,

        /// <summary>Завершить проект</summary>
        CompleteProject,

        /// <summary>Коммерческий отдел время переноса</summary>
        Com_New_Time,
        /// <summary>Коммерческий отдел тип упаковки</summary>
        COM_Package_Type,

        /// <summary>ИТО Гидравлика Время план</summary>
        ITO_G_Time_Plan,
        /// <summary>ИТО Гидравлика Время Факт</summary>
        ITO_Hydraulics_Time_Actual,
        /// <summary>ИТО Электрика Время план</summary>
        ITO_E_Time_Plan,
        /// <summary>ИТО Электрика Время Факт</summary>
        ITO_Electrician_Time_Actual,
        /// <summary>ИТО Рама Время план</summary>
        ITO_R_Time_Plan,
        /// <summary>ИТО Рама Время Факт</summary>
        ITO_Rama_Time_Actual,
        /// <summary>ИТО Гидравлика не требуется</summary>
        Is_ITO_G_NotNeed,
        /// <summary>ИТО Электрика не требуется</summary>
        Is_ITO_E_NotNeed,
        /// <summary>ИТО Рама не требуется</summary>
        Is_ITO_R_NotNeed,

        /// <summary>ИТО Рама тип</summary>
        ITO_R_Mode,

        /// <summary>Склад Гидравлика Время план</summary>
        WH_G_Time_Plan,
        /// <summary>Склад Гидравлика Время Факт</summary>
        WH_Hydraulics_Time_Actual,
        /// <summary>Склад Электрика Время план</summary>
        WH_E_Time_Plan,
        /// <summary>Склад Электрика Время Факт</summary>
        WH_Electrician_Time_Actual,
        /// <summary>Склад Рама Время план</summary>
        WH_R_Time_Plan,
        /// <summary>Склад Рама Время Факт</summary>
        WH_Rama_Time_Actual,
        /// <summary>Склад Гидравлика не требуется</summary>
        Is_WH_G_NotNeed,
        /// <summary>Склад Электрика не требуется</summary>
        Is_WH_E_NotNeed,
        /// <summary>Склад Рама не требуется</summary>
        Is_WH_R_NotNeed,

        /// <summary>Склад время создания недокоплекта гидравлика</summary>
        Time_WH_G_Requests_Create,
        /// <summary>Склад время создания недокоплекта Электрика</summary>
        Time_WH_E_Requests_Create,

        /// <summary>ОМТС Гидравлика Время план</summary>
        OMTS_G_Time_Plan,
        /// <summary>ОМТС Гидравлика Время Факт</summary>
        OMTS_Hydraulics_Time_Actual,
        /// <summary>ОМТС Электрика Время план</summary>
        OMTS_E_Time_Plan,
        /// <summary>ОМТС Электрика Время Факт</summary>
        OMTS_Electrician_Time_Actual,

        /// <summary>ОТК Время план</summary>
        OTK_Time_Plan,
        /// <summary>ОТК Гидравлика Время Факт</summary>
        OTK_Hydraulics_Time_Actual,
        /// <summary>ОТК Электрика Время Факт</summary>
        OTK_Electrician_Time_Actual,

        /// <summary>ОТК Гидравлика не требуется</summary>
        Is_OTK_G_NotNeed,
        /// <summary>ОТК Электрика не требуется</summary>
        Is_OTK_E_NotNeed,

        /// <summary>Производство Время планируемое</summary>
        MF_Time_Planed,
        /// <summary>Производство уровень</summary>
        MF_Level,
        /// <summary>Производство пост</summary>
        MF_Post,
        /// <summary>Производство прогресс</summary>
        MF_Complete_Percentage,
        /// <summary>Производство рама</summary>
        MF_Rama,
        /// <summary>Производство Коллектор</summary>
        MF_Collector,
        /// <summary>Производство Агрегат</summary>
        MF_Agregat,
        /// <summary>Производство ШУ</summary>
        MF_SH,
        /// <summary> Производство ШУ место</summary>
        MF_SH_Place,
        /// <summary>Производство тест факт</summary>
        MF_Time_Test_Actual,

        /// <summary>ОМТС Запросы Электрика</summary>
        Requests_E,
        /// <summary>ОМТС Запросы Гидравлика</summary>
        Requests_G,
        /// <summary>Коментарии</summary>
        Messagess,
    }

}
