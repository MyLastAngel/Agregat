using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ArgDb
{
    /// <summary>Типы действий для планирования производства</summary>
    public enum MFWorkerActionType
    {
        [Description("Отпуск")]
        Holiday = 1,
        [Description("Больничный")]
        Hospital = 2,
        [Description("Проект")]
        Project = 4,
        [Description("Резервирование проекта")]
        ReseveProject = 8,
        [Description("Простой")]
        Avait = 16,
    }
}
