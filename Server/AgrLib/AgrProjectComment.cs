using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgDb
{
    [Serializable]
    /// <summary>Коментарии для проекта</summary>
    public class AgrProjectComment
    {
        #region Свойства
        public DateTime Date { get; set; }
        //public int ProjectId { get; set; }
        public string User { get; set; }
        //public string Group { get; set; }
        public string Message { get; set; }
        #endregion

        public AgrProjectComment(DateTime t, string user, string m)
        {
            Date = t;
            User = user;
            //ProjectId = projectId;
            Message = m;
        }

        public AgrProjectComment Clone()
        {
            return new AgrProjectComment(this.Date, this.User, this.Message)
            {
                //Group = this.Group,
            };
        }

        public override string ToString()
        {
            return string.Format("{0}  '{1}'", Message, User);
        }
    }
}
