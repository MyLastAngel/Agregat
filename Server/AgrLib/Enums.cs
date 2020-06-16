using System;
using System.ComponentModel;

namespace ArgDb
{
    [Serializable]
    [Flags]
    public enum UserRight
    {
        [Description("Не определен")]
        None = 0,
        [Description("Добавлять проект")]
        AddNewProject = 1,
        [Description("Комерсант")]
        EditCommercial = 2,
        [Description("ИТО")]
        EditITO = 4,
        [Description("Склад")]
        EditWarehouse = 8,
        [Description("МТС")]
        EditOMTS = 16,
        [Description("Производство")]
        EditManufacture = 32,
        [Description("ОТК")]
        EditOTK = 64,
        [Description("Полный доступ")]
        All = AddNewProject | EditCommercial | EditITO | EditManufacture | EditOMTS | EditOTK | EditWarehouse,
    }

    [Serializable]
    public enum OMTSRequestType
    {
        [Description("Неизвестно")]
        Unknown = 0,
        [Description("Гидравлика")]
        Hydraulics = 1,
        [Description("Электрика")]
        Electrician = 2
    }
}
