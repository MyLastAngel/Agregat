using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProjectControlSystem
{
    public enum PlannerMode
    {
        PlanProject_Confirm,
        CommentEdit,

        Remove,
        Break,
        ChangeDays,

        ChangeProgressNone,
        ChangeProgressStart,
        ChangeProgressHalf,
        ChangeProgressFinish,
    }
}
