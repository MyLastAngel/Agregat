using ArgDb;
using ProjectControlSystem.Controls;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProjectControlSystem
{
    /// <summary>Описание запроса на поставку</summary>
    public class OMTSRequest : INotifyPropertyChanged, IDisposable
    {
        #region Поля
        bool isCstomerMaterials = false;

        OMTSRequestType type = OMTSRequestType.Unknown;
        uint count = 0, existCount = 0;

        DateTime? dateComplete = null, dateComplete_Plan = null;

        string eName = "", article = "";
        #endregion

        #region Свойства
        public string CurrentUser { get { return UserManager.Name; } }

        public OMTSRequestType Type
        {
            get { return type; }
            private set
            {
                if (type == value)
                    return;

                type = value;
                DoPropertyChanged("Type");
            }
        }

        /// <summary>Всего нужно</summary>
        public uint TotalCount
        {
            get { return count; }
            set
            {
                if (count == value)
                    return;

                count = value;

                DoPropertyChanged("TotalCount");
                DoPropertyChanged("DebtCount");
            }
        }
        /// <summary>Долг</summary>
        public uint DebtCount { get { return count - existCount; } }
        /// <summary>В наличие</summary>
        public uint ExistCount
        {
            get { return existCount; }
            set
            {
                if (existCount == value)
                    return;

                // Количество в наличие не может быть больше чем долг
                if (count - value < 0)
                    value = count;

                existCount = value;

                if (DebtCount == 0)
                    DateComplete = DateTime.Now;

                DoPropertyChanged("ExistCount");
                DoPropertyChanged("DebtCount");
            }
        }

        public string Name
        {
            get { return eName; }
            set
            {
                if (eName == value)
                    return;

                eName = value;
                DoPropertyChanged("Element");
            }
        }

        /// <summary>Давальческое сырье</summary>
        public bool IsCustomerMaterials
        {
            get { return isCstomerMaterials; }
            set
            {
                if (isCstomerMaterials == value)
                    return;

                isCstomerMaterials = value;
                DoPropertyChanged("IsCustomerMaterials");
            }
        }

        /// <summary>Артикул товара</summary>
        public string Article
        {
            get { return article; }
            set
            {
                if (article == value)
                    return;

                if (!string.IsNullOrEmpty(value))
                    value = value.Trim();

                article = value;
                DoPropertyChanged("Article");
            }
        }

        public DateTime? DateComplete_Plan
        {
            get { return dateComplete_Plan; }
            set
            {
                if (dateComplete_Plan == value)
                    return;

                dateComplete_Plan = value;
                DoPropertyChanged("DateComplete_Plan");
            }
        }
        public DateTime? DateComplete
        {
            get { return dateComplete; }
            set
            {
                if (dateComplete == value)
                    return;

                dateComplete = value;
                DoPropertyChanged("DateComplete");
            }
        }
        #endregion

        public OMTSRequest(OMTSRequestType t, string name, uint count)
        {
            type = t;
            Name = name;
            TotalCount = count;
        }

        public OMTSRequest Clone()
        {
            return new OMTSRequest(Type, Name, TotalCount)
            {
                existCount = this.existCount,
                DateComplete_Plan = this.DateComplete_Plan,
                DateComplete = this.DateComplete,
                Article = this.Article,
                IsCustomerMaterials = this.IsCustomerMaterials,
            };
        }
        public AgrRequest GetRequest()
        {
            return new AgrRequest(Type, Name, TotalCount)
            {
                ExistCount = existCount,
                DateComplete_Plan = DateComplete_Plan,
                DateComplete = DateComplete,
                Article = Article,
                IsCustomerMaterials = IsCustomerMaterials
            };
        }
        public static OMTSRequest Convert(AgrRequest source)
        {
            return new OMTSRequest(source.Type, source.Name, source.TotalCount)
           {
               existCount = source.ExistCount,
               DateComplete_Plan = source.DateComplete_Plan,
               DateComplete = source.DateComplete,
               Article = source.Article,
               IsCustomerMaterials = source.IsCustomerMaterials
           };
        }

        public bool IsSame(OMTSRequest source)
        {
            return Type == source.Type
                && TotalCount == source.TotalCount
                && ExistCount == source.ExistCount
                && Name == source.Name
                && Article == source.Article
                && DateComplete_Plan == source.DateComplete_Plan
                && DateComplete == source.DateComplete
                && IsCustomerMaterials == source.IsCustomerMaterials;
        }

        #region События.
        public event PropertyChangedEventHandler PropertyChanged;
        private void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public void Dispose()
        {
            PropertyChanged = null;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}({2}) {3}", Article, Name, count, type);
        }
    }

    /// <summary>Описание события</summary>
    public class EventMessage
    {
        #region Свойства
        public DateTime Date { get; set; }
        public int ProjectID { get; set; }
        public string Message { get; set; }
        #endregion

        public EventMessage(DateTime t, int id, string m)
        {
            Date = t;
            ProjectID = id;
            Message = m;
        }
    }

    public class SelectedChangedEventArgs : EventArgs
    {
        #region Свойства
        public int ID { get; private set; }
        #endregion

        public SelectedChangedEventArgs(int id)
        {
            ID = id;
        }
    }

    /// <summary>Новый проект</summary>
    public class NewProject : INotifyPropertyChanged
    {
        #region Поля
        string customer = "",
               customerName = "",
               product = "",
               id = "",
               options = "",
               comments = "",
               packageType = "";

        bool isManagerSetPlanDate = false,
             isConfirm = false;

        DateTime startTime = DateTime.Now,
                 endTime = DateTime.Now.AddDays(14);
        #endregion

        #region Свойства
        public bool IsConfirm
        {
            get { return isConfirm; }
            set
            {
                if (isConfirm == value)
                    return;

                isConfirm = value;
                DoPropertyChanged("IsConfirm");
            }
        }

        public string Customer
        {
            get { return customer; }
            set
            {
                if (customer == value)
                    return;

                customer = value;
                DoPropertyChanged("Customer");
            }
        }
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                if (customerName == value)
                    return;

                customerName = value;
                DoPropertyChanged("CustomerName");
            }
        }
        public string Product
        {
            get { return product; }
            set
            {
                if (product == value)
                    return;

                product = value;
                DoPropertyChanged("Product");
            }
        }
        public string Id
        {
            get { return id; }
            set
            {
                if (id == value)
                    return;

                id = value;
                DoPropertyChanged("Id");
            }
        }
        public string Options
        {
            get { return options; }
            set
            {
                if (options == value)
                    return;

                options = value;
                DoPropertyChanged("Options");
            }
        }
        public string Comments
        {
            get { return comments; }
            set
            {
                if (comments == value)
                    return;

                comments = value;
                DoPropertyChanged("Comments");
            }
        }
        public string PackageType
        {
            get { return packageType; }
            set
            {
                if (packageType == value)
                    return;

                packageType = value;
                DoPropertyChanged("PackageType");
            }
        }

        public bool IsManagerSetPlanDate
        {
            get { return isManagerSetPlanDate; }
            set
            {
                if (isManagerSetPlanDate == value)
                    return;

                isManagerSetPlanDate = value;
                DoPropertyChanged("IsManagerSetPlanDate");
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime == value)
                    return;

                startTime = value;
                DoPropertyChanged("StartTime");
            }
        }
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime == value)
                    return;

                endTime = value;
                DoPropertyChanged("EndTime");
            }
        }
        #endregion

        public NewProject()
        { }

        internal void Set(ProjectControl cProject)
        {
            Customer = cProject.NewCustomer;
            CustomerName = cProject.NewCustomerName;
            Product = cProject.NewProduct;
            Id = cProject.NewId;
            Options = cProject.NewOptions;
            Comments = cProject.NewComments;
            PackageType = cProject.NewPackageType;
            IsManagerSetPlanDate = cProject.NewIsManagerSetPlanDate;
            StartTime = cProject.NewStartTime;
            EndTime = cProject.NewEndTime;
        }

        #region События
        public event PropertyChangedEventHandler PropertyChanged;
        void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}
