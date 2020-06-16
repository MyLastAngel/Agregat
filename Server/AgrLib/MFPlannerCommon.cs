using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgDb
{
    [Serializable]
    /// <summary>Представляет описание действия для планирования производства</summary>
    public class MFPlannerAction
    {
        #region Свойства
        /// <summary>Id действия</summary>
        public int Id { get; set; }
        /// <summary>Id целевого проекта</summary>
        public int TargetId { get; set; }
        /// <summary>Id работника</summary>
        public int WorkerId { get; set; }

        /// <summary>Дата начала</summary>
        public DateTime TimeBegin { get; set; }

        [field: NonSerialized]
        /// <summary>Дата конца</summary>
        public DateTime TimeEnd { get { return TimeBegin.AddDays(Days); } }

        /// <summary>Количество дней</summary>
        public int Days { get; set; }

        /// <summary>Комментарий</summary>
        public string Comment { get; set; }

        /// <summary>Тип действия</summary>
        public MFWorkerActionType Type { get; set; }
        #endregion

        public MFPlannerAction(MFWorkerActionType type, int id, int workerId, int projectId)
        {
            Id = id;
            TargetId = projectId;
            WorkerId = workerId;
            Type = type;
        }
    }

    [Serializable]
    /// <summary>Представляет работника</summary>
    public class MFWorker
    {
        #region Свойства
        public int Id { get; set; }
        public int Post { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }

        /// <summary>Дата увольнения</summary>
        public DateTime? EndWorkTime { get; set; }
        #endregion

        public MFWorker()
        {
        }

        public MFWorker(int id, string name, string secondName, int post)
        {
            Id = id;
            Name = name;
            SecondName = secondName;
            Post = post;
        }

        public MFWorker Clone()
        {
            return new MFWorker
            {
                Id = this.Id,
                Post = this.Post,
                Name = this.Name,
                SecondName = this.SecondName,

                EndWorkTime = this.EndWorkTime
            };
        }
    }
}
