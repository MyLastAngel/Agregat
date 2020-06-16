using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArgDb
{
    [Serializable]
    /// <summary>Описание пользователя</summary>
    public class AgrUser// : ISerializable
    {
        #region Свойства
        //public int ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        public UserRight Rights { get; set; }
        #endregion

        public AgrUser() {}
        public AgrUser(string n)
        {
            Name = n;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class LUsers : List<AgrUser>// , ISerializable
    {
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
           
        //}
    }

    [Serializable]
    public class CUsers
    {
        //private LUsers _users;
       // public LUsers Users()
        public LUsers Users()
        {
            //get
            //{
                //if (_users == null)
                //{
                    LUsers _users = new LUsers();
                    if (Db.LoadUsers())
                    {
                        foreach (AgrDataSet.UserRow drUser in Db.Ds.User.Select())
                        {
                            var user = new AgrUser(drUser.Name)
                                {
                                    Group = drUser.Group,
                                    Rights = (UserRight)drUser.Right
                                };
                            _users.Add(user);
                        }
                    }
                //}
                return _users;
            //}
        }
    }
}
