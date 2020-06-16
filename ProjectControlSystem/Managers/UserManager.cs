using ArgDb;
using System;

namespace ProjectControlSystem.Managers
{
    public static class UserManager
    {
        #region Поля
        const string sGuest = "Гость";
        #endregion

        #region Свойства
        //public static int ID { get; private set; }
        public static string Name { get; private set; }
        public static string Group { get; private set; }

        public static UserRight Rights { get; private set; }

        public static bool IsGuest { get { return Name == sGuest; } }
        #endregion

        static UserManager()
        {
            Logout();
            //Name = "Иванов И. П.";
            //Group = "Коммерческий отдел";
            //Rights = UserRight.EditCommercial | UserRight.AddNewProject;
        }

        public static bool Login(AgrUser user, string pswd)
        {
            if (!ServiceManager.Login(user.Name, pswd))
                return false;

            //ID = user.ID;
            Name = user.Name;
            Group = user.Group;
            Rights = user.Rights;

            DoUserChanged();

            return true;
        }
        public static void Logout()
        {
            //ID = -1;
            Name = sGuest;

#if Develop 
            Rights = UserRight.All;
            Group = "Develop mode";
#else
            Rights = UserRight.None;
            Group = "только чтение";
#endif

            DoUserChanged();
        }

        #region События

        #region UserChanged

        public static event EventHandler UserChanged;
        static void DoUserChanged()
        {
            if (UserChanged != null)
                UserChanged(null, null);
        }

        #endregion

        #endregion
    }
}
