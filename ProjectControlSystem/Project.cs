using ArgDb;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProjectControlSystem
{
    public class Project : INotifyPropertyChanged, IDisposable
    {
        #region Поля
        public const string complete = "Готов", notNeed = "Не надо", inProgress = "В работе",
                inStart = "Начало", inHalf = "Половина", inFinish = "Финиш";

        string id = "", customer = "", customerName = "", product = "", options = "",
               mf_level = "", mf_rama = "", mf_post = "", mf_agregat = "", mf_sh_place = "", mf_sh = "", mf_collector = "",
               ito_R_Mode = "",
               com_package_type = "",
               mf_complete_percentage = "";

        DateTime timeBegin = DateTime.MinValue, timeEndPlaned = DateTime.MinValue;

        DateTime? time_ITO_G_Actual = null, time_ITO_R_Actual = null, time_ITO_E_Actual = null,
                  time_WH_G_Actual = null, time_WH_E_Actual = null, time_WH_R_Actual = null,
                  time_OMTS_G_Actual = null, time_OMTS_E_Actual = null,
                  timeEndActual = null, com_New_Time = null,
                  mf_time = null, mf_Time_Plan = null, mf_Time_Test_Actual = null,
                  time_OTK_G_Actual = null, time_OTK_E_Actual = null,
                  time_WH_G_Requests_Create = null, time_WH_E_Requests_Create = null,
                  time_ITO_G_Plan = null, time_ITO_E_Plan = null, time_ITO_R_Plan = null,
                  time_WH_G_Plan = null, time_WH_E_Plan = null, time_WH_R_Plan = null,
                  time_OMTS_G_Plan = null, time_OMTS_E_Plan = null,
                  time_OTK_Plan = null;

        bool is_ITO_G_NotNeed = false, is_ITO_R_NotNeed = false, is_ITO_E_NotNeed = false,
              is_OTK_G_NotNeed = false, is_OTK_E_NotNeed = false,
              is_WH_G_NotNeed = false, is_WH_E_NotNeed = false, is_WH_R_NotNeed = false;

        bool isManagerSetPlanDate = false, isStop = false;

        readonly ObservableCollection<OMTSRequest> requests = new ObservableCollection<OMTSRequest>();
        readonly ObservableCollection<AgrProjectComment> messages = new ObservableCollection<AgrProjectComment>();
        #endregion

        #region Свойства
        public int ProjectID { get; internal set; }

        public string ID
        {
            get { return id; }
            set
            {
                if (id == value)
                    return;

                id = value;
                DoPropertyChanged("ID");
            }
        }

        /// <summary>Проект в стопе</summary>
        public bool IsStop
        {
            get { return isStop; }
            set
            {
                if (isStop != value)
                    return;

                isStop = value;

                DoPropertyChanged("IsStop");
                DoPropertyChanged("State");
            }
        }

        /// <summary>Контрагент</summary>
        public string Customer
        {
            get { return customer; }
            set
            {
                if (customer == value)
                    return;

                customer = value;

                DoPropertyChanged("Customer");
                DoPropertyChanged("FullCustomerName");
            }
        }
        /// <summary>Заказчик</summary>
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                if (customerName == value)
                    return;

                customerName = value;

                DoPropertyChanged("CustomerName");
                DoPropertyChanged("FullCustomerName");
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
        public string Options
        {
            get { return options; }
            set
            {
                if (options == value)
                    return;

                options = value;

                DoPropertyChanged("Options");
                DoPropertyChanged("ID");
                DoPropertyChanged("FullCustomerName");
            }
        }

        /// <summary>Менеджеры сами устанавливают планируемое время.</summary>
        public bool IsManagerSetPlanDate
        {
#if Develop
            get { return false; }
#else
            get { return isManagerSetPlanDate; }
#endif
            set
            {
                if (isManagerSetPlanDate == value)
                    return;

                isManagerSetPlanDate = value;
                DoPropertyChanged("IsManagerSetPlanDate");

                DoPropertyChanged("Time_ITO_G_Plan");
                DoPropertyChanged("Time_ITO_E_Plan");
                DoPropertyChanged("Time_ITO_R_Plan");

                DoPropertyChanged("Time_WH_G_Plan");
                DoPropertyChanged("Time_WH_E_Plan");
                DoPropertyChanged("Time_WH_R_Plan");

                DoPropertyChanged("Time_OMTS_G_Plan");
                DoPropertyChanged("Time_OMTS_E_Plan");

                DoPropertyChanged("Time_OTK_Plan");
            }
        }

        #region ITO
        /// <summary>Гидравлические схемы план (12/9 для 15/10 дней до планируемой даты отгрузки)</summary>
        public DateTime? Time_ITO_G_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_ITO_G_Plan;
                else
                {
                    var days = TimeManager.WorkDaysCount(TimeBegin, TimeEndPlaned);
                    if (days >= 15)
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 12);
                    //if (days == 10)
                    else
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 9);
                }
            }
            set
            {
                if (!IsManagerSetPlanDate || time_ITO_G_Plan == value)
                    return;

                time_ITO_G_Plan = value;
                DoPropertyChanged("Time_ITO_G_Plan");
            }
        }
        /// <summary>Гидравлические схемы </summary>
        public DateTime? Time_ITO_G_Actual
        {
            get { return time_ITO_G_Actual; }
            set
            {
                if (time_ITO_G_Actual == value)
                    return;

                time_ITO_G_Actual = value;

                DoPropertyChanged("Time_ITO_G_Actual");
                DoPropertyChanged("Is_ITO_G_Ok");

                DoPropertyChanged("Time_WH_G_Plan");

                DoPropertyChanged("IsDebt_G_None");
                DoPropertyChanged("IsDebtOk");

                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }
        /// <summary>Не требуется гидравлика</summary>
        public bool Is_ITO_G_NotNeed
        {
            get { return is_ITO_G_NotNeed; }
            set
            {
                if (is_ITO_G_NotNeed == value)
                    return;

                is_ITO_G_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                {
                    Time_ITO_G_Actual = DateTime.Now.Date;
                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_ITO_G_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_ITO_G_NotNeed");
                DoPropertyChanged("Is_ITO_G_Ok");
                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }

        /// <summary>Электрические схемы план (10/7 для 15/10 дней до планируемой даты отгрузки)</summary>
        public DateTime? Time_ITO_E_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_ITO_E_Plan;
                else
                {
                    var days = TimeManager.WorkDaysCount(TimeBegin, TimeEndPlaned);
                    if (days >= 15)
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 10);
                    //if (days == 10)
                    else
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 7);
                }
            }
            set
            {
                if (!IsManagerSetPlanDate || time_ITO_E_Plan == value)
                    return;

                time_ITO_E_Plan = value;
                DoPropertyChanged("Time_ITO_E_Plan");
            }
        }
        /// <summary>Электрические схемы</summary>
        public DateTime? Time_ITO_E_Actual
        {
            get { return time_ITO_E_Actual; }
            set
            {
                if (time_ITO_E_Actual == value)
                    return;

                time_ITO_E_Actual = value;

                DoPropertyChanged("Time_ITO_E_Actual");
                DoPropertyChanged("Is_ITO_E_Ok");

                DoPropertyChanged("Time_WH_E_Plan");

                DoPropertyChanged("IsDebt_E_None");
                DoPropertyChanged("IsDebtOk");

                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }
        /// <summary>Не требуется электрика</summary>
        public bool Is_ITO_E_NotNeed
        {
            get { return is_ITO_E_NotNeed; }
            set
            {
                if (is_ITO_E_NotNeed == value)
                    return;

                is_ITO_E_NotNeed = value;

                // Устанавливаем даты для всего что связано с электрикой
                if (value == true)
                {
                    Time_ITO_E_Actual = DateTime.Now.Date;
                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_ITO_E_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_ITO_E_NotNeed");
                DoPropertyChanged("Is_ITO_E_Ok");
                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }

        /// <summary>Рама план (10/7 для 15/10 дней до планируемой даты отгрузки)</summary>
        public DateTime? Time_ITO_R_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_ITO_R_Plan;
                else
                {
                    var days = TimeManager.WorkDaysCount(TimeBegin, TimeEndPlaned);
                    if (days >= 15)
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 10);
                    //if (days == 10)
                    else
                        return TimeManager.RemoveWorkDays(TimeEndPlaned, 7);
                }
            }
            set
            {
                if (!IsManagerSetPlanDate || time_ITO_R_Plan == value)
                    return;

                time_ITO_R_Plan = value;
                DoPropertyChanged("Time_ITO_R_Plan");
            }
        }
        /// <summary>Рама</summary>
        public DateTime? Time_ITO_R_Actual
        {
            get { return time_ITO_R_Actual; }
            set
            {
                if (time_ITO_R_Actual == value)
                    return;

                time_ITO_R_Actual = value;

                DoPropertyChanged("Time_ITO_R_Actual");
                DoPropertyChanged("Is_ITO_R_Ok");

                DoPropertyChanged("Time_WH_R_Plan");
                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }
        /// <summary>ИТО Рама тип</summary>
        public string ITO_R_Mode
        {
            get { return ito_R_Mode; }
            set
            {
                if (ito_R_Mode == value)
                    return;

                ito_R_Mode = value;
                DoPropertyChanged("ITO_R_Mode");
            }
        }
        /// <summary>Не требуется рама</summary>
        public bool Is_ITO_R_NotNeed
        {
            get { return is_ITO_R_NotNeed; }
            set
            {
                if (is_ITO_R_NotNeed == value)
                    return;

                is_ITO_R_NotNeed = value;

                // Устанавливаем даты для всего что связано с рамой
                if (value == true)
                {
                    Time_ITO_R_Actual = DateTime.Now.Date;
                    ITO_R_Mode = notNeed;

                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_ITO_R_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_ITO_R_NotNeed");
                DoPropertyChanged("Is_ITO_R_Ok");
                DoPropertyChanged("Is_ITO_NotEnoughTime");
            }
        }

        /// <summary>ИТО гидравлика состояние</summary>
        public bool Is_ITO_G_Ok { get { return Is_ITO_G_NotNeed == true || time_ITO_G_Actual.HasValue; } }
        /// <summary>ИТО Электрика состояние</summary>
        public bool Is_ITO_E_Ok { get { return Is_ITO_E_NotNeed == true || time_ITO_E_Actual.HasValue; } }
        /// <summary>ИТО Рама состояние</summary>
        public bool Is_ITO_R_Ok { get { return Is_ITO_R_NotNeed == true || time_ITO_R_Actual.HasValue; } }

        /// <summary>ИТО не хвататет времени по плану</summary>
        public bool Is_ITO_NotEnoughTime
        {
            get
            {
                // проект закрыт нам на все пофиг
                if (TimeEndActual.HasValue)
                    return false;

                var now = DateTime.Now;

                // если гидравлика нужна И значения актуального нет И сейчас день позднее чем по плану
                if (Is_ITO_G_NotNeed != true && !Time_ITO_G_Actual.HasValue && now >= Time_ITO_G_Plan)
                    return true;
                if (Is_ITO_E_NotNeed != true && !Time_ITO_E_Actual.HasValue && now >= Time_ITO_E_Plan)
                    return true;
                if (Is_ITO_R_NotNeed != true && !Time_ITO_R_Actual.HasValue && now >= Time_ITO_R_Plan)
                    return true;

                return false;
            }
        }
        #endregion

        #region Warehouse
        /// <summary>СКЛАД Гидравлика план (+2 дня от готовности ИТО)</summary>
        public DateTime? Time_WH_G_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_WH_G_Plan;

                if (Time_ITO_G_Actual.HasValue)
                    return TimeManager.AddWorkDays(Time_ITO_G_Actual.Value, 2);
                else
                    return TimeManager.AddWorkDays(Time_ITO_G_Plan.Value, 2);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_WH_G_Plan == value)
                    return;

                time_WH_G_Plan = value;
                DoPropertyChanged("Time_WH_G_Plan");
            }
        }
        /// <summary>СКЛАД Гидравлика</summary>
        public DateTime? Time_WH_G_Actual
        {
            get { return time_WH_G_Actual; }
            set
            {
                if (time_WH_G_Actual == value)
                    return;

                // Если у нас нет задолженостей по складу нет задолженостей и по ОМТС
                if (!Time_OMTS_G_Actual.HasValue)
                    Time_OMTS_G_Actual = value;

                time_WH_G_Actual = value;

                DoPropertyChanged("Time_WH_G_Actual");
                DoPropertyChanged("Is_WH_G_Ok");

                DoPropertyChanged("Is_OTK_E_Time_Enable");
                DoPropertyChanged("Is_OTK_G_Time_Enable");

                DoPropertyChanged("IsDebt_G_None");
                DoPropertyChanged("IsDebtOk");

                DoPropertyChanged("IsCanCompleteProject");
            }
        }
        /// <summary>Склад Не требуется гидравлика</summary>
        public bool Is_WH_G_NotNeed
        {
            get { return is_WH_G_NotNeed; }
            set
            {
                if (is_WH_G_NotNeed == value)
                    return;

                is_WH_G_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                {
                    Time_WH_G_Actual = DateTime.Now.Date;
                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_WH_G_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_WH_G_NotNeed");
                DoPropertyChanged("Is_WH_G_Ok");

                DoPropertyChanged("Is_OMTS_G_Ok");
            }
        }

        /// <summary>СКЛАД Электрика план (+2 дня от готовности ИТО)</summary>
        public DateTime? Time_WH_E_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_WH_E_Plan;

                if (Time_ITO_E_Actual.HasValue)
                    return TimeManager.AddWorkDays(Time_ITO_E_Actual.Value, 2);
                else
                    return TimeManager.AddWorkDays(Time_ITO_E_Plan.Value, 2);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_WH_E_Plan == value)
                    return;

                time_WH_E_Plan = value;
                DoPropertyChanged("Time_WH_E_Plan");
            }
        }
        /// <summary>СКЛАД Электрика</summary>
        public DateTime? Time_WH_E_Actual
        {
            get { return time_WH_E_Actual; }
            set
            {
                if (time_WH_E_Actual == value)
                    return;

                // Если у нас нет задолженостей по складу нет задолженостей и по ОМТС
                if (!Time_OMTS_E_Actual.HasValue)
                    Time_OMTS_E_Actual = value;

                time_WH_E_Actual = value;

                DoPropertyChanged("Time_WH_E_Actual");
                DoPropertyChanged("Is_WH_E_Ok");

                DoPropertyChanged("IsCommercialDepartamentOk");

                DoPropertyChanged("Is_OTK_E_Time_Enable");
                DoPropertyChanged("Is_OTK_G_Time_Enable");

                DoPropertyChanged("IsDebt_E_None");
                DoPropertyChanged("IsDebtOk");

                DoPropertyChanged("IsCanCompleteProject");
            }
        }
        /// <summary>Склад Не требуется электрика</summary>
        public bool Is_WH_E_NotNeed
        {
            get { return is_WH_E_NotNeed; }
            set
            {
                if (is_WH_E_NotNeed == value)
                    return;

                is_WH_E_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                {
                    Time_WH_E_Actual = DateTime.Now.Date;

                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_WH_E_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_WH_E_NotNeed");
                DoPropertyChanged("Is_WH_E_Ok");

                DoPropertyChanged("Is_OMTS_E_Ok");
            }
        }

        /// <summary>СКЛАД Рама план (+1 дня от готовности ИТО)</summary>
        public DateTime? Time_WH_R_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_WH_R_Plan;

                if (Time_ITO_R_Actual.HasValue)
                    return TimeManager.AddWorkDays(Time_ITO_R_Actual.Value, 1);
                else
                    return TimeManager.AddWorkDays(Time_ITO_R_Plan.Value, 1);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_WH_R_Plan == value)
                    return;

                time_WH_R_Plan = value;
                DoPropertyChanged("Time_WH_R_Plan");
            }
        }
        /// <summary>СКЛАД Рама</summary>
        public DateTime? Time_WH_R_Actual
        {
            get { return time_WH_R_Actual; }
            set
            {
                if (time_WH_R_Actual == value)
                    return;

                time_WH_R_Actual = value;

                DoPropertyChanged("Time_WH_R_Actual");
                DoPropertyChanged("Is_WH_R_Ok");

                DoPropertyChanged("IsCommercialDepartamentOk");

                DoPropertyChanged("Is_OTK_E_Time_Enable");
                DoPropertyChanged("Is_OTK_G_Time_Enable");

                DoPropertyChanged("IsCanCompleteProject");
            }
        }
        /// <summary>Склад Не требуется Рама</summary>
        public bool Is_WH_R_NotNeed
        {
            get { return is_WH_R_NotNeed; }
            set
            {
                if (is_WH_R_NotNeed == value)
                    return;

                is_WH_R_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                {
                    Time_WH_R_Actual = DateTime.Now.Date;

                    //if (IsManagerSetPlanDate) // если план дата устанавливается в ручную то ставим сегодняшнее число тк не нужно
                    //    Time_WH_R_Plan = DateTime.Now.Date;
                }

                DoPropertyChanged("Is_WH_R_NotNeed");
                DoPropertyChanged("Is_WH_R_Ok");
            }
        }

        /// <summary>Склад дата создания Недокомплекта по гидравлике</summary>
        public DateTime? Time_WH_G_Requests_Create
        {
            get { return time_WH_G_Requests_Create; }
            set
            {
                if (time_WH_G_Requests_Create == value)
                    return;

                time_WH_G_Requests_Create = value;

                DoPropertyChanged("Time_WH_G_Requests_Create");
            }
        }
        /// <summary>Склад дата создания Недокомплекта по электрике</summary>
        public DateTime? Time_WH_E_Requests_Create
        {
            get { return time_WH_E_Requests_Create; }
            set
            {
                if (time_WH_E_Requests_Create == value)
                    return;

                time_WH_E_Requests_Create = value;

                DoPropertyChanged("Time_WH_E_Requests_Create");
            }
        }

        /// <summary>Склад может ли закрывать позицию гидравлики</summary>
        public bool Is_WH_G_CanComplete { get { return requests.Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Hydraulics) == 0; } }
        /// <summary>Склад может ли закрывать позицию електрики</summary>
        public bool Is_WH_E_CanComplete { get { return requests.Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Electrician) == 0; } }

        /// <summary>СКЛАД Гидравлика состояние</summary>
        public bool Is_WH_G_Ok { get { return Is_WH_G_NotNeed == true || Time_WH_G_Actual.HasValue; } }
        /// <summary>СКЛАД Электрика состояние</summary>
        public bool Is_WH_E_Ok { get { return Is_WH_E_NotNeed == true || Time_WH_E_Actual.HasValue; } }
        /// <summary>СКЛАД Рама состояние</summary>
        public bool Is_WH_R_Ok { get { return Is_WH_R_NotNeed == true || Time_WH_R_Actual.HasValue; } }
        #endregion

        #region OMTS
        /// <summary>ОМТС Гидравлика план (+2 дня от готовности ИТО)</summary>
        public DateTime? Time_OMTS_G_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_OMTS_G_Plan;

                if (Time_ITO_G_Actual.HasValue)
                    return TimeManager.AddWorkDays(Time_ITO_G_Actual.Value, 2);
                else
                    return TimeManager.AddWorkDays(Time_ITO_G_Plan.Value, 2);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_OMTS_G_Plan == value)
                    return;

                time_OMTS_G_Plan = value;
                DoPropertyChanged("Time_OMTS_G_Plan");
            }
        }
        /// <summary>ОМТС Гидравлика</summary>
        public DateTime? Time_OMTS_G_Actual
        {
            get { return time_OMTS_G_Actual; }
            set
            {
                if (time_OMTS_G_Actual == value)
                    return;

                time_OMTS_G_Actual = value;

                DoPropertyChanged("Time_OMTS_G_Actual");
                DoPropertyChanged("Is_OMTS_G_Ok");

                DoPropertyChanged("IsCommercialDepartamentOk");
                DoPropertyChanged("IsCanCompleteProject");
            }
        }

        /// <summary>ОМТС Электрика план (+2 дня от готовности ИТО)</summary>
        public DateTime? Time_OMTS_E_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_OMTS_E_Plan;

                if (Time_ITO_E_Actual.HasValue)
                    return TimeManager.AddWorkDays(Time_ITO_E_Actual.Value, 2);
                else
                    return TimeManager.AddWorkDays(Time_ITO_E_Plan.Value, 2);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_OMTS_E_Plan == value)
                    return;

                time_OMTS_E_Plan = value;
                DoPropertyChanged("Time_OMTS_E_Plan");
            }
        }
        /// <summary>ОМТС Электрика</summary>
        public DateTime? Time_OMTS_E_Actual
        {
            get
            {
                //if (IsDebt_E_None == false)
                //    return null;

                return time_OMTS_E_Actual;
            }
            set
            {
                if (time_OMTS_E_Actual == value)
                    return;

                time_OMTS_E_Actual = value;

                DoPropertyChanged("Time_OMTS_E_Actual");
                DoPropertyChanged("Is_OMTS_E_Ok");

                DoPropertyChanged("IsCommercialDepartamentOk");
                DoPropertyChanged("IsCanCompleteProject");
            }
        }

        /// <summary>ОМТС Гидравлика состояние</summary>
        public bool Is_OMTS_G_Ok { get { return Is_WH_G_NotNeed == true || Time_OMTS_G_Actual.HasValue; } }
        /// <summary>ОМТС Электрика состояние</summary>
        public bool Is_OMTS_E_Ok { get { return Is_WH_E_NotNeed == true || Time_OMTS_E_Actual.HasValue; } }
        #endregion

        #region Commercial
        /// <summary>Время начало проекта</summary>
        public DateTime TimeBegin
        {
            get { return timeBegin; }
            private set
            {
                if (timeBegin == value)
                    return;

                timeBegin = value;
                DoPropertyChanged("TimeBegin");
            }
        }
        /// <summary>Запланированное время окончание проекта</summary>
        public DateTime TimeEndPlaned
        {
            get
            {
                if (com_New_Time.HasValue)
                    return com_New_Time.Value;
                else
                    return timeEndPlaned;
            }
            //private set
            //{
            //    if (timeEndPlaned == value)
            //        return;

            //    timeEndPlaned = value;
            //    DoPropertyChanged("TimeEndPlaned");
            //}
        }
        /// <summary>Запланированное время окончание проекта при сoздании</summary>
        public DateTime TimeEndCreatePlaned
        {
            get { return timeEndPlaned; }
        }

        /// <summary>Измененное время окончание проекта</summary>
        public DateTime? Com_New_Time
        {
            get { return com_New_Time; }
            set
            {
                if (com_New_Time == value)
                    return;

                com_New_Time = value;

                DoPropertyChanged("TimeEndPlaned");
                DoPropertyChanged("Com_New_Time");

                DoPropertyChanged("Time_ITO_G_Plan");
                DoPropertyChanged("Time_ITO_E_Plan");
                DoPropertyChanged("Time_ITO_R_Plan");

                DoPropertyChanged("Is_ITO_NotEnoughTime");

                DoPropertyChanged("Time_OMTS_G_Plan");
                DoPropertyChanged("Time_OMTS_E_Plan");

                DoPropertyChanged("IsManufactureDepartamentOk");
                DoPropertyChanged("IsManufactureNotEnoughTime");

                DoPropertyChanged("State");
            }
        }

        /// <summary>Фактическое время завершения проекта</summary>
        public DateTime? TimeEndActual
        {
            get { return timeEndActual; }
            set
            {
                if (timeEndActual == value)
                    return;

                timeEndActual = value;

                DoPropertyChanged("TimeEndActual");

                DoPropertyChanged("IsCommercialDepartamentOk");
                DoPropertyChanged("IsCanCompleteProject");

                DoPropertyChanged("Is_ITO_NotEnoughTime");

                DoPropertyChanged("State");
            }
        }

        /// <summary>Возможно ли завершить проект</summary>
        public bool IsCanCompleteProject
        {
            get
            {
                // не учитывается 
                var result = Is_ITO_G_Ok && Is_ITO_E_Ok && Is_ITO_R_Ok
                              && Is_WH_G_Ok && Is_WH_E_Ok && Is_WH_R_Ok
                              && Is_OMTS_G_Ok && Is_OMTS_E_Ok
                              && IsManufactureDepartamentOk
                              && IsOTKDepartamentOk;

                return result;
            }
        }

        /// <summary>Время окончания проекта переносилось </summary>
        public bool IsEndTimeChanged { get { return com_New_Time.HasValue; } }

        /// <summary>Закрыт проект</summary>
        public bool IsCommercialDepartamentOk { get { return true; } }
        //public bool IsCommercialDepartamentOk { get { return TimeEndActual.HasValue; } }

        /// <summary>Тип упаковки</summary>
        public string COM_Package_Type
        {
            get { return com_package_type; }
            set
            {
                if (com_package_type == value)
                    return;

                com_package_type = value;

                DoPropertyChanged("COM_Package_Type");
            }
        }
        #endregion

        #region Manufacture
        /// <summary>Производство участок</summary>
        public string MF_Level
        {
            get { return mf_level; }
            set
            {
                if (mf_level == value)
                    return;

                mf_level = value;

                // Сбрасываем все значения 
                MF_Rama = MF_SH = MF_Agregat = MF_Post = "";

                DoPropertyChanged("MF_Level");
            }
        }
        /// <summary>Производство рама</summary>
        public string MF_Rama
        {
            get { return mf_rama; }
            set
            {
                if (mf_rama == value)
                    return;

                mf_rama = value;

                DoPropertyChanged("MF_Rama");

                DoPropertyChanged("IsManufactureDepartamentOk");
            }
        }

        /// <summary>Производство рама</summary>
        public string MF_Collector
        {
            get { return mf_collector; }
            set
            {
                if (mf_collector == value)
                    return;

                mf_collector = value;

                DoPropertyChanged("MF_Collector");
            }
        }

        /// <summary>Производство Номер поста</summary>
        public string MF_Post
        {
            get { return mf_post; }
            set
            {
                if (mf_post == value)
                    return;

                mf_post = value;
                DoPropertyChanged("MF_Post");
            }
        }

        /// <summary>Производство Прогресс</summary>
        public string MF_Complete_Percentage
        {
            get { return mf_complete_percentage; }
            set
            {
                if (mf_complete_percentage == value)
                    return;

                mf_complete_percentage = value;
                DoPropertyChanged("MF_Complete_Percentage");
            }
        }

        /// <summary>Производство Расключение агрегата</summary>
        public string MF_Agregat
        {
            get { return mf_agregat; }
            set
            {
                if (mf_agregat == value)
                    return;

                mf_agregat = value;

                DoPropertyChanged("MF_Agregat");

                DoPropertyChanged("IsManufactureDepartamentOk");
            }
        }

        /// <summary>Производство Место изготовления ШУ</summary>
        public string MF_SH_Place
        {
            get { return mf_sh_place; }
            set
            {
                if (mf_sh_place == value)
                    return;

                mf_sh_place = value;
                DoPropertyChanged("MF_SH_Place");
            }
        }
        /// <summary>Производство ШУ</summary>
        public string MF_SH
        {
            get { return mf_sh; }
            set
            {
                if (mf_sh == value)
                    return;

                mf_sh = value;

                DoPropertyChanged("MF_SH");

                DoPropertyChanged("IsManufactureDepartamentOk");
            }
        }

        /// <summary>Производство Планируемая дата завершения</summary>
        public DateTime? MF_Time_Plan
        {
            get { return mf_Time_Plan; }
            set
            {
                if (mf_Time_Plan == value)
                    return;

                mf_Time_Plan = value;

                DoPropertyChanged("MF_Time_Plan");

                DoPropertyChanged("IsManufactureDepartamentOk");
                DoPropertyChanged("IsManufactureNotEnoughTime");

                DoPropertyChanged("State");
            }
        }

        /// <summary>Производство Тест факт</summary>
        public DateTime? MF_Time_Test_Actual
        {
            get { return mf_Time_Test_Actual; }
            set
            {
                if (mf_Time_Test_Actual == value)
                    return;

                mf_Time_Test_Actual = value;

                DoPropertyChanged("MF_Time_Test_Actual");
                DoPropertyChanged("IsManufactureDepartamentOk");
            }
        }

        /// <summary>Производство Время завершения</summary>
        public DateTime? MF_Time
        {
            get { return mf_time; }
            set
            {
                if (mf_time == value)
                    return;

                mf_time = value;

                DoPropertyChanged("MF_Time");
            }
        }

        public bool IsManufactureDepartamentOk
        {
            get
            {
                //// Если производство план >= Планируемого времени конца
                //if (MF_Time_Plan.HasValue && MF_Time_Plan.Value.Date > TimeEndPlaned.Date)
                //    return false;

                //!string.IsNullOrEmpty(MF_Rama) && (MF_Rama == complete || MF_Rama == notNeed)

                // не требуется или или завершено - значит все ОК
                if (!string.IsNullOrEmpty(MF_Agregat) && (MF_Agregat == complete || MF_Agregat == notNeed) &&
                    !string.IsNullOrEmpty(MF_SH) && (MF_SH == complete || MF_SH == notNeed))
                    return true;
                else
                    return false;
                //return MF_Time_Test_Actual.HasValue;
            }
        }

        // Ориентировочная дата готовности производства больше чем дата отгрузки
        public bool IsManufactureNotEnoughTime { get { return MF_Time_Plan.HasValue && MF_Time_Plan.Value > TimeEndPlaned; } }
        #endregion

        #region OTK
        /// <summary>ОТК дата передачи (-1 дня от дня отгрузки)</summary>
        public DateTime? Time_OTK_Plan
        {
            get
            {
                if (IsManagerSetPlanDate)
                    return time_OTK_Plan;

                return TimeManager.RemoveWorkDays(TimeEndPlaned, 1);
            }
            set
            {
                if (!IsManagerSetPlanDate || time_OTK_Plan == value)
                    return;

                time_OTK_Plan = value;
                DoPropertyChanged("Time_OTK_Plan");
            }
        }
        /// <summary>ОТК дата испытаний по гидравлике</summary>
        public DateTime? Time_OTK_G_Actual
        {
            get { return time_OTK_G_Actual; }
            set
            {
                if (time_OTK_G_Actual == value)
                    return;

                time_OTK_G_Actual = value;

                DoPropertyChanged("Time_OTK_G_Actual");
                DoPropertyChanged("IsOTKDepartamentOk");

                DoPropertyChanged("IsCommercialDepartamentOk");
                DoPropertyChanged("IsCanCompleteProject");
            }
        }
        /// <summary>ОТК Не требуется гидравлика</summary>
        public bool Is_OTK_G_NotNeed
        {
            get { return is_OTK_G_NotNeed; }
            set
            {
                if (is_OTK_G_NotNeed == value)
                    return;

                is_OTK_G_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                    Time_OTK_G_Actual = DateTime.Now.Date;

                DoPropertyChanged("Is_OTK_G_NotNeed");
                DoPropertyChanged("IsOTKDepartamentOk");
            }
        }

        /// <summary>ОТК дата испытаний эл Шкафа</summary>
        public DateTime? Time_OTK_E_Actual
        {
            get { return time_OTK_E_Actual; }
            set
            {
                if (time_OTK_E_Actual == value)
                    return;

                time_OTK_E_Actual = value;

                DoPropertyChanged("Time_OTK_E_Actual");
                DoPropertyChanged("IsOTKDepartamentOk");

                DoPropertyChanged("IsCommercialDepartamentOk");
                DoPropertyChanged("IsCanCompleteProject");
            }
        }
        /// <summary>ОТК Не требуется гидравлика</summary>
        public bool Is_OTK_E_NotNeed
        {
            get { return is_OTK_E_NotNeed; }
            set
            {
                if (is_OTK_E_NotNeed == value)
                    return;

                is_OTK_E_NotNeed = value;

                // Устанавливаем даты для всего что связано с гидравликой
                if (value == true)
                    Time_OTK_E_Actual = DateTime.Now.Date;

                DoPropertyChanged("Is_OTK_E_NotNeed");
                DoPropertyChanged("IsOTKDepartamentOk");
            }
        }

        /// <summary> ОТК можно ли испытвать гидравлику</summary>
        public bool Is_OTK_G_Time_Enable { get { return Is_WH_G_Ok && IsManufactureDepartamentOk; } }
        /// <summary> ОТК можно ли испытвать электрику</summary>
        public bool Is_OTK_E_Time_Enable { get { return Is_WH_E_Ok && IsManufactureDepartamentOk; } }

        /// <summary>ОТК состояние</summary>
        public bool IsOTKDepartamentOk { get { return (Is_OTK_G_NotNeed == true || Time_OTK_G_Actual.HasValue) && (Is_OTK_E_NotNeed == true || Time_OTK_E_Actual.HasValue); } }
        #endregion

        #region Designer
        public string CurrentUser { get { return UserManager.Name; } }

        public static string[] levelItems = new string[] { "Гидравлика", "Электрика" },
                         stateItems = new string[] { "", inProgress, notNeed, complete },
                         stateWithTestItems = new string[] { "", "Согласование", inProgress, notNeed, complete },
                         customerItems = new string[] { "", "Агрегат", "СЭБ", "РКМ" },
                         confirmItems = new string[] { "", "Да", "Нет" },
                         packageTypeItems = new string[] { "", "Доски" },
                         ramaModes = new string[] { "", "Сборная", "Сварная", notNeed },
                         percentageMode = new string[] { "", inStart, inHalf, inFinish };

        public string[] DesignerLevels { get { return levelItems; } }
        public string[] DesignerStates { get { return stateItems; } }
        public string[] DesignerCustomers { get { return customerItems; } }
        public string[] DesignerStatesTest { get { return stateWithTestItems; } }
        public string[] DesignerConfirm { get { return confirmItems; } }
        public string[] DesignerPackagetTypes { get { return packageTypeItems; } }

        public string[] DesignerRamaModes { get { return ramaModes; } }
        public string[] DesignerPercentageMode { get { return percentageMode; } }

        /// <summary>Состояние проекта зависит от времени</summary>
        public ProjectState State
        {
            get
            {
                if (IsStop)
                    return ProjectState.Stop;

                // если проект завершили то все хорошо
                if (TimeEndActual.HasValue)
                {
                    if (IsDebtOk == false) // Если есть долги (проект завершен с долгами)
                        return ProjectState.Error;

                    return ProjectState.Ok;
                }

                //MF_Time_Plan

                // Если производство план >= Планируемого времени конца
                if (MF_Time_Plan.HasValue && MF_Time_Plan.Value.Date > TimeEndPlaned.Date)
                    return ProjectState.Error;

                // если у нас кл-во дней выполнение меньше чем любое из планируемых
                var days = TimeManager.WorkDaysCount(TimeBegin, TimeEndPlaned);
                if (days < 10)
                    return ProjectState.Warning;

                //var timePlaned = TimeManager.AddWorkDays(TimeBegin.Date, 15);

                //// если проект надо завершить раньше чем через 15 дней
                //if (TimeEndPlaned.Date < timePlaned.Date)
                //    return ProjectState.Warning;

                // Если проект просрачивают
                if (TimeEndPlaned.Date < DateTime.Now.Date)
                    return ProjectState.Error;

                // В других случаях возвращаем ок
                return ProjectState.Ok;
            }
        }
        #endregion

        /// <summary>Запросы недокомплекта</summary>
        public ObservableCollection<OMTSRequest> Requests { get { return requests; } }
        /// <summary>Коментарии</summary>
        public ObservableCollection<AgrProjectComment> Messages { get { return messages; } }

        /// <summary>Долги по комплектации</summary>
        public bool? IsDebtOk
        {
            get
            {
                if (IsDebt_G_None == null && IsDebt_E_None == null)
                    return null;

                // Если есть недокомплект но он весь закрыт
                if (IsDebt_G_None.HasValue && IsDebt_E_None.HasValue)
                    return IsDebt_G_None.Value && IsDebt_E_None.Value;

                // Если недокомплект только по гидравлике
                if (IsDebt_G_None.HasValue)
                    return IsDebt_G_None.Value;

                // Если недокомплект только по электрике
                if (IsDebt_E_None.HasValue)
                    return IsDebt_E_None.Value;

                return null;
            }
        }
        /// <summary>Задолженности по гидравлике</summary>
        public bool? IsDebt_G_None
        {
            get
            {
                if (!Time_ITO_G_Actual.HasValue)
                    return null;

                var rez = requests.Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Hydraulics) == 0;
                if (rez == true)
                {
                    if (Time_WH_G_Actual.HasValue)
                        return true;
                    else
                        return null;
                }
                else
                    return false;
            }
        }
        /// <summary>Задолженности по электрике</summary>
        public bool? IsDebt_E_None
        {
            get
            {
                if (!Time_ITO_E_Actual.HasValue)
                    return null;

                var rez = requests.Count(x => x.DebtCount != 0 && x.Type == OMTSRequestType.Electrician) == 0;
                if (rez == true)
                {
                    if (Time_WH_E_Actual.HasValue)
                        return true;
                    else
                        return null;
                }
                else
                    return false;
            }
        }

        #endregion

        public Project(string id, string customer, string customerName, string product, string options, DateTime sDate, DateTime eDate)
            : this()
        {
            ID = id;
            Customer = customer;
            CustomerName = customerName;
            Product = product;
            Options = options;
            TimeBegin = sDate;
            timeEndPlaned = eDate;
        }
        Project()
        {
            ProjectID = -1;

            UserManager.UserChanged += UserManager_UserChanged;
            TimeManager.DayChanged += TimeManager_DayChanged;
        }

        public void AddMessage(AgrProjectComment m)
        {
            messages.Insert(0, m);

            DoPropertyChanged("Messages");
        }

        /// <summary>Устанавливаем значения</summary>
        public void SetValue(ProjectPropertyType type, object value)
        {
            switch (type)
            {
                #region ID
                case ProjectPropertyType.ID:
                    {
                        if (value is string)
                            ID = (string)value;
                        break;
                    }
                #endregion
                #region Customer
                case ProjectPropertyType.Customer:
                    {
                        if (value is string)
                            Customer = (string)value;
                        break;
                    }
                #endregion
                #region CustomerName
                case ProjectPropertyType.CustomerName:
                    {
                        if (value is string)
                            CustomerName = (string)value;
                        break;
                    }
                #endregion
                #region Product
                case ProjectPropertyType.Product:
                    {
                        if (value is string)
                            Product = (string)value;
                        break;
                    }
                #endregion
                #region Options
                case ProjectPropertyType.Options:
                    {
                        if (value is string)
                            Options = (string)value;
                        break;
                    }
                #endregion
                #region IsStop
                case ProjectPropertyType.IsStop:
                    {
                        if (value is bool)
                            IsStop = (bool)value;
                        break;
                    }
                #endregion

                #region IsManagerSetPlanDate
                case ProjectPropertyType.IsManagerSetPlanDate:
                    {
                        if (value is bool)
                            IsManagerSetPlanDate = (bool)value;

                        break;
                    }
                #endregion

                // ИТО
                #region  ITO_G_Time_Plan
                case ProjectPropertyType.ITO_G_Time_Plan:
                    {
                        Time_ITO_G_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  ITO_E_Time_Plan
                case ProjectPropertyType.ITO_E_Time_Plan:
                    {
                        Time_ITO_E_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  ITO_R_Time_Plan
                case ProjectPropertyType.ITO_R_Time_Plan:
                    {
                        Time_ITO_R_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                // Склад
                #region  WH_G_Time_Plan
                case ProjectPropertyType.WH_G_Time_Plan:
                    {
                        Time_WH_G_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  WH_E_Time_Plan
                case ProjectPropertyType.WH_E_Time_Plan:
                    {
                        Time_WH_E_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  WH_R_Time_Plan
                case ProjectPropertyType.WH_R_Time_Plan:
                    {
                        Time_WH_R_Plan = value as DateTime?;
                        break;
                    }
                #endregion

                // ОМТС
                #region  OMTS_G_Time_Plan
                case ProjectPropertyType.OMTS_G_Time_Plan:
                    {
                        Time_OMTS_G_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  OMTS_E_Time_Plan
                case ProjectPropertyType.OMTS_E_Time_Plan:
                    {
                        Time_OMTS_E_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region  OTK_Time_Plan
                case ProjectPropertyType.OTK_Time_Plan:
                    {
                        Time_OTK_Plan = value as DateTime?;
                        break;
                    }
                #endregion


                #region CompleteProject
                case ProjectPropertyType.CompleteProject:
                    {
                        TimeEndActual = value as DateTime?;

                        break;
                    }
                #endregion

                #region Com_New_Time
                case ProjectPropertyType.Com_New_Time:
                    {
                        Com_New_Time = value as DateTime?;
                        break;
                    }
                #endregion
                #region COM_Package_Type
                case ProjectPropertyType.COM_Package_Type:
                    {
                        if (value is string)
                            COM_Package_Type = (string)value;

                        break;
                    }
                #endregion

                #region ITO_Hydraulics_Time_Actual
                case ProjectPropertyType.ITO_Hydraulics_Time_Actual:
                    {
                        Time_ITO_G_Actual = value as DateTime?;
                        break;
                    }
                #endregion
                #region ITO_Electrician_Time_Actual
                case ProjectPropertyType.ITO_Electrician_Time_Actual:
                    {

                        Time_ITO_E_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region ITO_Rama_Time_Actual
                case ProjectPropertyType.ITO_Rama_Time_Actual:
                    {
                        Time_ITO_R_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region ITO_R_Mode
                case ProjectPropertyType.ITO_R_Mode:
                    {
                        if (value is string)
                            ITO_R_Mode = (string)value;

                        break;
                    }
                #endregion
                #region Is_ITO_G_NotNeed
                case ProjectPropertyType.Is_ITO_G_NotNeed:
                    {
                        if (value is bool)
                            Is_ITO_G_NotNeed = (bool)value;

                        break;
                    }
                #endregion
                #region Is_ITO_E_NotNeed
                case ProjectPropertyType.Is_ITO_E_NotNeed:
                    {
                        if (value is bool)
                            Is_ITO_E_NotNeed = (bool)value;

                        break;
                    }
                #endregion
                #region Is_ITO_R_NotNeed
                case ProjectPropertyType.Is_ITO_R_NotNeed:
                    {
                        if (value is bool)
                            Is_ITO_R_NotNeed = (bool)value;

                        break;
                    }
                #endregion

                #region WH_Hydraulics_Time_Actual
                case ProjectPropertyType.WH_Hydraulics_Time_Actual:
                    {
                        Time_WH_G_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region WH_Electrician_Time_Actual
                case ProjectPropertyType.WH_Electrician_Time_Actual:
                    {
                        Time_WH_E_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region WH_Rama_Time_Actual
                case ProjectPropertyType.WH_Rama_Time_Actual:
                    {
                        Time_WH_R_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region Is_WH_G_NotNeed
                case ProjectPropertyType.Is_WH_G_NotNeed:
                    {
                        if (value is bool)
                            Is_WH_G_NotNeed = (bool)value;

                        break;
                    }
                #endregion
                #region Is_WH_E_NotNeed
                case ProjectPropertyType.Is_WH_E_NotNeed:
                    {
                        if (value is bool)
                            Is_WH_E_NotNeed = (bool)value;

                        break;
                    }
                #endregion
                #region Is_WH_R_NotNeed
                case ProjectPropertyType.Is_WH_R_NotNeed:
                    {
                        if (value is bool)
                            Is_WH_R_NotNeed = (bool)value;

                        break;
                    }
                #endregion

                #region OMTS_Hydraulics_Time_Actual
                case ProjectPropertyType.OMTS_Hydraulics_Time_Actual:
                    {
                        Time_OMTS_G_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region OMTS_Electrician_Time_Actual
                case ProjectPropertyType.OMTS_Electrician_Time_Actual:
                    {
                        Time_OMTS_E_Actual = value as DateTime?;
                        break;
                    }
                #endregion

                #region OTK_Hydraulics_Time_Actual
                case ProjectPropertyType.OTK_Hydraulics_Time_Actual:
                    {
                        Time_OTK_G_Actual = value as DateTime?;
                        break;
                    }
                #endregion
                #region OTK_Electrician_Time_Actual
                case ProjectPropertyType.OTK_Electrician_Time_Actual:
                    {
                        Time_OTK_E_Actual = value as DateTime?;

                        break;
                    }
                #endregion
                #region Is_OTK_G_NotNeed
                case ProjectPropertyType.Is_OTK_G_NotNeed:
                    {
                        if (value is bool)
                            Is_OTK_G_NotNeed = (bool)value;

                        break;
                    }
                #endregion
                #region Is_OTK_E_NotNeed
                case ProjectPropertyType.Is_OTK_E_NotNeed:
                    {
                        if (value is bool)
                            Is_OTK_E_NotNeed = (bool)value;

                        break;
                    }
                #endregion

                #region MF_Time_Planed
                case ProjectPropertyType.MF_Time_Planed:
                    {
                        MF_Time_Plan = value as DateTime?;
                        break;
                    }
                #endregion
                #region MF_Level
                case ProjectPropertyType.MF_Level:
                    {
                        if (value is string)
                            MF_Level = (string)value;

                        break;
                    }
                #endregion
                #region MF_Rama
                case ProjectPropertyType.MF_Rama:
                    {
                        if (value is string)
                            MF_Rama = (string)value;

                        break;
                    }
                #endregion
                #region MF_Collector
                case ProjectPropertyType.MF_Collector:
                    {
                        if (value is string)
                            MF_Collector = (string)value;

                        break;
                    }
                #endregion


                #region MF_Post
                case ProjectPropertyType.MF_Post:
                    {
                        MF_Post = value as string;
                        break;
                    }
                #endregion
                #region MF_Complete_Percentage
                case ProjectPropertyType.MF_Complete_Percentage:
                    {
                        if (value is string)
                            MF_Complete_Percentage = (string)value;

                        break;
                    }
                #endregion

                #region MF_Agregat
                case ProjectPropertyType.MF_Agregat:
                    {
                        if (value is string)
                            MF_Agregat = (string)value;

                        break;
                    }
                #endregion
                #region MF_SH
                case ProjectPropertyType.MF_SH:
                    {
                        if (value is string)
                            MF_SH = (string)value;

                        break;
                    }
                #endregion
                #region MF_SH_Place
                case ProjectPropertyType.MF_SH_Place:
                    {
                        if (value is string)
                            MF_SH_Place = (string)value;

                        break;
                    }
                #endregion

                #region MF_Time_Test_Actual
                case ProjectPropertyType.MF_Time_Test_Actual:
                    {
                        MF_Time_Test_Actual = value as DateTime?;
                        break;
                    }
                #endregion

                #region Requests_E
                case ProjectPropertyType.Requests_E:
                    {
                        if (!(value is IEnumerable<OMTSRequest>))
                            return;

                        var rList = Requests.Where(x => x.Type == OMTSRequestType.Electrician).ToArray();
                        foreach (var r in rList)
                            Requests.Remove(r);

                        foreach (var r in ((IEnumerable<OMTSRequest>)value))
                        {
                            if (!Time_WH_E_Requests_Create.HasValue)
                                Time_WH_E_Requests_Create = DateTime.Now.Date;

                            Requests.Add(r);
                        }

                        DoRequestChanged();

                        break;
                    }
                #endregion
                #region Requests_G
                case ProjectPropertyType.Requests_G:
                    {
                        if (!(value is IEnumerable<OMTSRequest>))
                            return;

                        var rList = Requests.Where(x => x.Type == OMTSRequestType.Hydraulics).ToArray();
                        foreach (var r in rList)
                            Requests.Remove(r);

                        foreach (var r in ((IEnumerable<OMTSRequest>)value))
                        {
                            if (!Time_WH_G_Requests_Create.HasValue)
                                Time_WH_G_Requests_Create = DateTime.Now.Date;

                            Requests.Add(r);
                        }

                        DoRequestChanged();

                        break;
                    }
                #endregion
                #region Messagess
                case ProjectPropertyType.Messagess:
                    {
                        if (!(value is IEnumerable<AgrProjectComment>))
                            return;

                        Messages.Clear();

                        foreach (var r in ((IEnumerable<AgrProjectComment>)value))
                            Messages.Add(r);

                        DoPropertyChanged("Messages");

                        break;
                    }
                    #endregion
            }
        }
        /// <summary>Копия проетка</summary>
        public Project Clone()
        {
            var aProject = GetAgrProject();
            return GetProject(aProject);
        }

        #region Обработчики событий
        // Пользователь изменился
        void UserManager_UserChanged(object sender, EventArgs e)
        {
            DoPropertyChanged("CurrentUser");
        }
        // Поменялся день
        void TimeManager_DayChanged(object sender, EventArgs e)
        {
            DoPropertyChanged("State");
            DoPropertyChanged("Is_ITO_NotEnoughTime");
        }
        #endregion

        #region События.

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        void DoRequestChanged()
        {
            DoPropertyChanged("IsDebt_G_None");
            DoPropertyChanged("IsDebt_E_None");

            DoPropertyChanged("IsDebtOk");

            DoPropertyChanged("Requests");

            DoPropertyChanged("Time_WH_G_Requests_Create");
            DoPropertyChanged("Time_WH_E_Requests_Create");
            DoPropertyChanged("Time_WH_R_Requests_Create");

            DoPropertyChanged("State");
        }

        #endregion

        #region Преобразование
        public AgrProject GetAgrProject()
        {
            var project = new AgrProject(ProjectID)
            {
                Com_New_Time = this.com_New_Time,
                COM_Package_Type = this.COM_Package_Type,
                Customer = this.customer,
                CustomerName = this.customerName,
                ID = this.ID,
                IsManagerSetPlanDate = this.IsManagerSetPlanDate,

                ITO_R_Mode = this.ito_R_Mode,
                MF_Agregat = this.mf_agregat,
                MF_Level = this.mf_level,
                MF_Post = this.mf_post,
                MF_Complete_Percentage = this.mf_complete_percentage,
                MF_Rama = this.mf_rama,
                MF_Collector = this.mf_collector,
                MF_SH = this.mf_sh,
                MF_SH_Place = this.mf_sh_place,
                MF_Time = this.mf_time,
                MF_Time_Plan = this.mf_Time_Plan,
                MF_Time_Test_Actual = this.mf_Time_Test_Actual,
                Options = this.options,
                Product = this.product,

                // ito time
                Time_ITO_G_Plan = this.time_ITO_G_Plan,
                Time_ITO_G_Actual = this.time_ITO_G_Actual,
                Time_ITO_E_Plan = this.time_ITO_E_Plan,
                Time_ITO_E_Actual = this.time_ITO_E_Actual,
                Time_ITO_R_Plan = this.time_ITO_R_Plan,
                Time_ITO_R_Actual = this.time_ITO_R_Actual,

                // wh time
                Time_WH_G_Plan = this.time_WH_G_Plan,
                Time_WH_G_Actual = this.time_WH_G_Actual,
                Time_WH_E_Plan = this.time_WH_E_Plan,
                Time_WH_E_Actual = this.time_WH_E_Actual,
                Time_WH_R_Plan = this.time_WH_R_Plan,
                Time_WH_R_Actual = this.time_WH_R_Actual,

                // omts time
                Time_OMTS_G_Plan = this.time_OMTS_G_Plan,
                Time_OMTS_G_Actual = this.time_OMTS_G_Actual,
                Time_OMTS_E_Plan = this.time_OMTS_E_Plan,
                Time_OMTS_E_Actual = this.time_OMTS_E_Actual,

                // otk time
                Time_OTK_Plan = this.time_OTK_Plan,
                Time_OTK_E_Actual = this.time_OTK_E_Actual,
                Time_OTK_G_Actual = this.time_OTK_G_Actual,

                TimeBegin = this.timeBegin,
                TimeEndActual = this.timeEndActual,
                TimeEndPlaned = this.timeEndPlaned,

                Is_ITO_G_NotNeed = this.Is_ITO_G_NotNeed,
                Is_ITO_E_NotNeed = this.Is_ITO_E_NotNeed,
                Is_ITO_R_NotNeed = this.Is_ITO_R_NotNeed,
                Is_OTK_G_NotNeed = this.Is_OTK_G_NotNeed,
                Is_OTK_E_NotNeed = this.Is_OTK_E_NotNeed,
                Is_WH_G_NotNeed = this.Is_WH_G_NotNeed,
                Is_WH_E_NotNeed = this.Is_WH_E_NotNeed,
                Is_WH_R_NotNeed = this.Is_WH_R_NotNeed,
                Time_WH_E_Requests_Create = this.Time_WH_E_Requests_Create,
                Time_WH_G_Requests_Create = this.Time_WH_G_Requests_Create
            };

            // Копируем сообщения
            foreach (var m in this.messages)
                project.Messages.Add(m.Clone());

            // копируем запросы
            foreach (var r in requests)
                project.Requests.Add(r.GetRequest());

            return project;
        }
        #endregion

        #region Статические методы
        public static Dictionary<ProjectPropertyType, object> GetChangies(Project source, Project changed)
        {
            var result = new Dictionary<ProjectPropertyType, object>();

            if (source.ID != changed.ID)
                result[ProjectPropertyType.ID] = changed.ID;
            if (source.Customer != changed.Customer)
                result[ProjectPropertyType.Customer] = changed.Customer;
            if (source.CustomerName != changed.CustomerName)
                result[ProjectPropertyType.CustomerName] = changed.CustomerName;
            if (source.Product != changed.Product)
                result[ProjectPropertyType.Product] = changed.Product;
            if (source.Options != changed.Options)
                result[ProjectPropertyType.Options] = changed.Options;
            if (source.COM_Package_Type != changed.COM_Package_Type)
                result[ProjectPropertyType.COM_Package_Type] = changed.COM_Package_Type;

            if (source.IsStop != changed.IsStop)
                result[ProjectPropertyType.IsStop] = changed.IsStop;

            if (source.IsManagerSetPlanDate != changed.IsManagerSetPlanDate)
                result[ProjectPropertyType.IsManagerSetPlanDate] = changed.IsManagerSetPlanDate;

            // устанавливаем планируемые даты
            if (changed.IsManagerSetPlanDate)
            {
                // ИТО
                if (source.time_ITO_G_Plan != changed.time_ITO_G_Plan)
                    result[ProjectPropertyType.ITO_G_Time_Plan] = changed.time_ITO_G_Plan;
                if (source.time_ITO_E_Plan != changed.time_ITO_E_Plan)
                    result[ProjectPropertyType.ITO_E_Time_Plan] = changed.time_ITO_E_Plan;
                if (source.time_ITO_R_Plan != changed.time_ITO_R_Plan)
                    result[ProjectPropertyType.ITO_R_Time_Plan] = changed.time_ITO_R_Plan;

                // Склад
                if (source.time_WH_G_Plan != changed.time_WH_G_Plan)
                    result[ProjectPropertyType.WH_G_Time_Plan] = changed.time_WH_G_Plan;
                if (source.time_WH_E_Plan != changed.time_WH_E_Plan)
                    result[ProjectPropertyType.WH_E_Time_Plan] = changed.time_WH_E_Plan;
                if (source.time_WH_R_Plan != changed.time_WH_R_Plan)
                    result[ProjectPropertyType.WH_R_Time_Plan] = changed.time_WH_R_Plan;

                // ОМТС
                if (source.time_OMTS_G_Plan != changed.time_OMTS_G_Plan)
                    result[ProjectPropertyType.OMTS_G_Time_Plan] = changed.Time_OMTS_G_Plan;
                if (source.time_OMTS_E_Plan != changed.time_OMTS_E_Plan)
                    result[ProjectPropertyType.OMTS_E_Time_Plan] = changed.time_OMTS_E_Plan;

                if (source.time_OTK_Plan != changed.time_OTK_Plan)
                    result[ProjectPropertyType.OTK_Time_Plan] = changed.time_OTK_Plan;
            }

            // Установка полей не требуется
            // ИТО
            if (source.Is_ITO_G_NotNeed != changed.Is_ITO_G_NotNeed)
                result[ProjectPropertyType.Is_ITO_G_NotNeed] = changed.Is_ITO_G_NotNeed;
            if (source.Is_ITO_E_NotNeed != changed.Is_ITO_E_NotNeed)
                result[ProjectPropertyType.Is_ITO_E_NotNeed] = changed.Is_ITO_E_NotNeed;
            if (source.Is_ITO_R_NotNeed != changed.Is_ITO_R_NotNeed)
                result[ProjectPropertyType.Is_ITO_R_NotNeed] = changed.Is_ITO_R_NotNeed;

            // ОТК
            if (source.Is_OTK_G_NotNeed != changed.Is_OTK_G_NotNeed)
                result[ProjectPropertyType.Is_OTK_G_NotNeed] = changed.Is_OTK_G_NotNeed;
            if (source.Is_OTK_E_NotNeed != changed.Is_OTK_E_NotNeed)
                result[ProjectPropertyType.Is_OTK_E_NotNeed] = changed.Is_OTK_E_NotNeed;

            // Склад
            if (source.Is_WH_G_NotNeed != changed.Is_WH_G_NotNeed)
                result[ProjectPropertyType.Is_WH_G_NotNeed] = changed.Is_WH_G_NotNeed;
            if (source.Is_WH_E_NotNeed != changed.Is_WH_E_NotNeed)
                result[ProjectPropertyType.Is_WH_E_NotNeed] = changed.Is_WH_E_NotNeed;
            if (source.Is_WH_R_NotNeed != changed.Is_WH_R_NotNeed)
                result[ProjectPropertyType.Is_WH_R_NotNeed] = changed.Is_WH_R_NotNeed;

            if (source.Time_WH_G_Requests_Create != changed.Time_WH_G_Requests_Create)
                result[ProjectPropertyType.Time_WH_G_Requests_Create] = changed.Time_WH_G_Requests_Create;
            if (source.Time_WH_E_Requests_Create != changed.Time_WH_E_Requests_Create)
                result[ProjectPropertyType.Time_WH_E_Requests_Create] = changed.Time_WH_E_Requests_Create;

            if (source.com_New_Time != changed.com_New_Time)
                result[ProjectPropertyType.Com_New_Time] = changed.com_New_Time;
            if (source.ito_R_Mode != changed.ito_R_Mode)
                result[ProjectPropertyType.ITO_R_Mode] = changed.ito_R_Mode;
            if (source.mf_agregat != changed.mf_agregat)
                result[ProjectPropertyType.MF_Agregat] = changed.mf_agregat;
            if (source.mf_level != changed.mf_level)
                result[ProjectPropertyType.MF_Level] = changed.mf_level;
            if (source.mf_post != changed.mf_post)
                result[ProjectPropertyType.MF_Post] = changed.mf_post;
            if (source.mf_complete_percentage != changed.mf_complete_percentage)
                result[ProjectPropertyType.MF_Complete_Percentage] = changed.mf_complete_percentage;

            if (source.mf_rama != changed.mf_rama)
                result[ProjectPropertyType.MF_Rama] = changed.mf_rama;
            if (source.mf_collector != changed.mf_collector)
                result[ProjectPropertyType.MF_Collector] = changed.mf_collector;
            if (source.mf_sh != changed.mf_sh)
                result[ProjectPropertyType.MF_SH] = changed.mf_sh;
            if (source.mf_sh_place != changed.mf_sh_place)
                result[ProjectPropertyType.MF_SH_Place] = changed.mf_sh_place;
            if (source.mf_time != changed.mf_time)
                result[ProjectPropertyType.MF_Time_Planed] = changed.mf_time;
            if (source.mf_Time_Plan != changed.mf_Time_Plan)
                result[ProjectPropertyType.MF_Time_Planed] = changed.mf_Time_Plan;
            if (source.mf_Time_Test_Actual != changed.mf_Time_Test_Actual)
                result[ProjectPropertyType.MF_Time_Test_Actual] = changed.mf_Time_Test_Actual;
            if (source.time_ITO_E_Actual != changed.time_ITO_E_Actual)
                result[ProjectPropertyType.ITO_Electrician_Time_Actual] = changed.time_ITO_E_Actual;
            if (source.time_ITO_G_Actual != changed.time_ITO_G_Actual)
                result[ProjectPropertyType.ITO_Hydraulics_Time_Actual] = changed.time_ITO_G_Actual;
            if (source.time_ITO_R_Actual != changed.time_ITO_R_Actual)
                result[ProjectPropertyType.ITO_Rama_Time_Actual] = changed.time_ITO_R_Actual;
            if (source.time_OMTS_E_Actual != changed.time_OMTS_E_Actual)
                result[ProjectPropertyType.OMTS_Electrician_Time_Actual] = changed.time_OMTS_E_Actual;
            if (source.time_OMTS_G_Actual != changed.time_OMTS_G_Actual)
                result[ProjectPropertyType.OMTS_Hydraulics_Time_Actual] = changed.time_OMTS_G_Actual;
            if (source.time_OTK_E_Actual != changed.time_OTK_E_Actual)
                result[ProjectPropertyType.OTK_Electrician_Time_Actual] = changed.time_OTK_E_Actual;
            if (source.time_OTK_G_Actual != changed.time_OTK_G_Actual)
                result[ProjectPropertyType.OTK_Hydraulics_Time_Actual] = changed.time_OTK_G_Actual;
            if (source.time_WH_E_Actual != changed.time_WH_E_Actual)
                result[ProjectPropertyType.WH_Electrician_Time_Actual] = changed.time_WH_E_Actual;
            if (source.time_WH_G_Actual != changed.time_WH_G_Actual)
                result[ProjectPropertyType.WH_Hydraulics_Time_Actual] = changed.time_WH_G_Actual;
            if (source.time_WH_R_Actual != changed.time_WH_R_Actual)
                result[ProjectPropertyType.WH_Rama_Time_Actual] = changed.time_WH_R_Actual;
            if (source.timeEndActual != changed.timeEndActual)
                result[ProjectPropertyType.CompleteProject] = changed.timeEndActual;

            // сообщения не удаляются
            if (source.messages.Count != changed.messages.Count)
                result[ProjectPropertyType.Messagess] = changed.messages;

            // Недокомплект электрика
            var isChanged = source.requests.Count(x => x.Type == OMTSRequestType.Electrician) != changed.requests.Count(x => x.Type == OMTSRequestType.Electrician);
            if (!isChanged) // проверям все
            {
                foreach (var r in changed.requests)
                {
                    if (r.Type != OMTSRequestType.Electrician)
                        continue;

                    // если нет похожего запроса
                    if (!source.requests.Any(x => x.IsSame(r)))
                    {
                        isChanged = true;
                        break;
                    }
                }
            }

            if (isChanged)
                result[ProjectPropertyType.Requests_E] = changed.requests.Where(x => x.Type == OMTSRequestType.Electrician);

            //Недокомплект  Гидравлика
            isChanged = source.requests.Count(x => x.Type == OMTSRequestType.Hydraulics) != changed.requests.Count(x => x.Type == OMTSRequestType.Hydraulics);
            if (!isChanged) // проверям все
            {
                foreach (var r in changed.requests)
                {
                    if (r.Type != OMTSRequestType.Hydraulics)
                        continue;

                    // если нет похожего запроса
                    if (!source.requests.Any(x => x.IsSame(r)))
                    {
                        isChanged = true;
                        break;
                    }
                }
            }

            if (isChanged)
                result[ProjectPropertyType.Requests_G] = changed.requests.Where(x => x.Type == OMTSRequestType.Hydraulics);


            return result;
        }

        public static Project GetProject(AgrProject source)
        {
            var project = new Project()
            {
                ProjectID = source.ProjectId,
                com_New_Time = source.Com_New_Time,
                COM_Package_Type = source.COM_Package_Type,
                customer = source.Customer,
                customerName = source.CustomerName,
                ID = source.ID,
                IsStop = source.IsStop,

                isManagerSetPlanDate = source.IsManagerSetPlanDate,

                ito_R_Mode = source.ITO_R_Mode,
                mf_agregat = source.MF_Agregat,
                mf_level = source.MF_Level,
                mf_post = source.MF_Post,
                mf_rama = source.MF_Rama,
                mf_collector = source.MF_Collector,
                mf_sh = source.MF_SH,
                mf_sh_place = source.MF_SH_Place,
                mf_time = source.MF_Time,
                mf_Time_Plan = source.MF_Time_Plan,
                mf_Time_Test_Actual = source.MF_Time_Test_Actual,
                options = source.Options,
                product = source.Product,

                time_ITO_G_Plan = source.Time_ITO_G_Plan,
                time_ITO_G_Actual = source.Time_ITO_G_Actual,
                time_ITO_E_Plan = source.Time_ITO_E_Plan,
                time_ITO_E_Actual = source.Time_ITO_E_Actual,
                time_ITO_R_Plan = source.Time_ITO_R_Plan,
                time_ITO_R_Actual = source.Time_ITO_R_Actual,

                time_WH_G_Plan = source.Time_WH_G_Plan,
                time_WH_G_Actual = source.Time_WH_G_Actual,
                time_WH_E_Plan = source.Time_WH_E_Plan,
                time_WH_E_Actual = source.Time_WH_E_Actual,
                time_WH_R_Plan = source.Time_WH_R_Plan,
                time_WH_R_Actual = source.Time_WH_R_Actual,

                time_OMTS_G_Plan = source.Time_OMTS_G_Plan,
                time_OMTS_G_Actual = source.Time_OMTS_G_Actual,
                time_OMTS_E_Plan = source.Time_OMTS_E_Plan,
                time_OMTS_E_Actual = source.Time_OMTS_E_Actual,

                time_OTK_Plan = source.Time_OTK_Plan,
                time_OTK_E_Actual = source.Time_OTK_E_Actual,
                time_OTK_G_Actual = source.Time_OTK_G_Actual,

                timeBegin = source.TimeBegin,
                timeEndActual = source.TimeEndActual,
                timeEndPlaned = source.TimeEndPlaned,
                Is_ITO_G_NotNeed = source.Is_ITO_G_NotNeed,
                Is_ITO_E_NotNeed = source.Is_ITO_E_NotNeed,
                Is_ITO_R_NotNeed = source.Is_ITO_R_NotNeed,
                Is_OTK_G_NotNeed = source.Is_OTK_G_NotNeed,
                Is_OTK_E_NotNeed = source.Is_OTK_E_NotNeed,
                Is_WH_G_NotNeed = source.Is_WH_G_NotNeed,
                Is_WH_E_NotNeed = source.Is_WH_E_NotNeed,
                Is_WH_R_NotNeed = source.Is_WH_R_NotNeed,
                Time_WH_E_Requests_Create = source.Time_WH_E_Requests_Create,
                Time_WH_G_Requests_Create = source.Time_WH_G_Requests_Create
            };

            // Копируем сообщения
            foreach (var m in source.Messages)
                project.Messages.Add(m.Clone());

            // копируем запросы
            foreach (var r in source.Requests)
                project.Requests.Add(OMTSRequest.Convert(r));

            return project;
        }

        public static string GetDescription(ProjectPropertyType type)
        {
            switch (type)
            {
                #region ID
                case ProjectPropertyType.ID:
                    return "ID Проекта (Администратор)";
                #endregion
                #region Customer
                case ProjectPropertyType.Customer:
                    return "Контрагент (Администратор)";
                #endregion
                #region CustomerName
                case ProjectPropertyType.CustomerName:
                    return "Проект (Администратор)";
                #endregion
                #region Product
                case ProjectPropertyType.Product:
                    return "Изделие (Администратор)";
                #endregion
                #region Options
                case ProjectPropertyType.Options:
                    return "Опции (Администратор)";
                #endregion
                #region IsStop
                case ProjectPropertyType.IsStop:
                    return "Проект в стопе";
                #endregion


                #region IsManagerSetPlanDate
                case ProjectPropertyType.IsManagerSetPlanDate:
                    return "Флаг разрешающий установку планируемого времени проекта";
                #endregion

                #region CompleteProject
                case ProjectPropertyType.CompleteProject:
                    return "Фактическое время завершения проекта";
                #endregion

                #region Com_New_Time
                case ProjectPropertyType.Com_New_Time:
                    return "Измененное время окончание проекта";
                #endregion
                #region COM_Package_Type
                case ProjectPropertyType.COM_Package_Type:
                    return "Тип Упаковки";
                #endregion

                #region ITO_G_Time_Plan
                case ProjectPropertyType.ITO_G_Time_Plan:
                    return "ИТО Гидравлические схемы (план)";
                #endregion
                #region ITO_Hydraulics_Time_Actual
                case ProjectPropertyType.ITO_Hydraulics_Time_Actual:
                    return "ИТО Гидравлические схемы (факт)";
                #endregion
                #region ITO_E_Time_Plan
                case ProjectPropertyType.ITO_E_Time_Plan:
                    return "ИТО Электрические схемы (план)";
                #endregion
                #region ITO_Electrician_Time_Actual
                case ProjectPropertyType.ITO_Electrician_Time_Actual:
                    return "ИТО Электрические схемы (факт)";
                #endregion
                #region ITO_R_Time_Plan
                case ProjectPropertyType.ITO_R_Time_Plan:
                    return "ИТО Рама схемы (план)";
                #endregion
                #region ITO_Rama_Time_Actual
                case ProjectPropertyType.ITO_Rama_Time_Actual:
                    return "ИТО Рама схема (факт)";
                #endregion
                #region ITO_R_Mode
                case ProjectPropertyType.ITO_R_Mode:
                    return "ИТО Тип Рамы";
                #endregion
                #region Is_ITO_G_NotNeed
                case ProjectPropertyType.Is_ITO_G_NotNeed:
                    return "ИТО Гидравлика не требуется";
                #endregion
                #region Is_ITO_E_NotNeed
                case ProjectPropertyType.Is_ITO_E_NotNeed:
                    return "ИТО Электрика не требуется";
                #endregion
                #region Is_ITO_R_NotNeed
                case ProjectPropertyType.Is_ITO_R_NotNeed:
                    return "ИТО Рама не требуется";
                #endregion

                #region WH_G_Time_Plan
                case ProjectPropertyType.WH_G_Time_Plan:
                    return "Склад Гидравлика (план)";
                #endregion
                #region WH_Hydraulics_Time_Actual
                case ProjectPropertyType.WH_Hydraulics_Time_Actual:
                    return "Склад Гидравлика (факт)";
                #endregion
                #region WH_E_Time_Plan
                case ProjectPropertyType.WH_E_Time_Plan:
                    return "Склад Электрика (план)";
                #endregion
                #region WH_Electrician_Time_Actual
                case ProjectPropertyType.WH_Electrician_Time_Actual:
                    return "Склад Электрика (факт)";
                #endregion
                #region WH_R_Time_Plan
                case ProjectPropertyType.WH_R_Time_Plan:
                    return "Склад Рама (план)";
                #endregion
                #region WH_Rama_Time_Actual
                case ProjectPropertyType.WH_Rama_Time_Actual:
                    return "Склад Рама (факт)";
                #endregion

                #region OMTS_G_Time_Plan
                case ProjectPropertyType.OMTS_G_Time_Plan:
                    return "ОМТС Гидравлика (план)";
                #endregion
                #region OMTS_Hydraulics_Time_Actual
                case ProjectPropertyType.OMTS_Hydraulics_Time_Actual:
                    return "ОМТС Гидравлика (факт)";
                #endregion
                #region OMTS_E_Time_Plan
                case ProjectPropertyType.OMTS_E_Time_Plan:
                    return "ОМТС Электрика (план)";
                #endregion
                #region OMTS_Electrician_Time_Actual
                case ProjectPropertyType.OMTS_Electrician_Time_Actual:
                    return "ОМТС Электрика (факт)";
                #endregion

                #region Is_WH_G_NotNeed
                case ProjectPropertyType.Is_WH_G_NotNeed:
                    return "Склад Гидравлика не требуется";
                #endregion
                #region Is_WH_E_NotNeed
                case ProjectPropertyType.Is_WH_E_NotNeed:
                    return "Склад Электрика не требуется";
                #endregion
                #region Is_WH_R_NotNeed
                case ProjectPropertyType.Is_WH_R_NotNeed:
                    return "Склад Рама не требуется";
                #endregion
                #region Time_WH_G_Requests_Create
                case ProjectPropertyType.Time_WH_G_Requests_Create:
                    return "Склад время создания недокоплекта гидравлика";
                #endregion
                #region Time_WH_E_Requests_Create
                case ProjectPropertyType.Time_WH_E_Requests_Create:
                    return "Склад время создания недокоплекта Электрика";
                #endregion

                #region OTK_Time_Plan
                case ProjectPropertyType.OTK_Time_Plan:
                    return "Передача на ОТК (план)";
                #endregion
                #region OTK_Hydraulics_Time_Actual
                case ProjectPropertyType.OTK_Hydraulics_Time_Actual:
                    return "ОТК Испытания Гидравлики";
                #endregion
                #region OTK_Electrician_Time_Actual
                case ProjectPropertyType.OTK_Electrician_Time_Actual:
                    return "ОТК Испытания Электрики";
                #endregion

                #region Is_OTK_G_NotNeed
                case ProjectPropertyType.Is_OTK_G_NotNeed:
                    return "ОТК Гидравлика не требуется";
                #endregion
                #region Is_OTK_E_NotNeed
                case ProjectPropertyType.Is_OTK_E_NotNeed:
                    return "ОТК Электрика не требуется";
                #endregion

                #region MF_Time_Planed
                case ProjectPropertyType.MF_Time_Planed:
                    return "Производство Дата завершения (план)";
                #endregion
                #region MF_Level
                case ProjectPropertyType.MF_Level:
                    return "Производство Участок";
                #endregion
                #region MF_Post
                case ProjectPropertyType.MF_Post:
                    return "Производство Пост";
                #endregion
                #region MF_Rama
                case ProjectPropertyType.MF_Rama:
                    return "Производство Рама";
                #endregion
                #region MF_Collector
                case ProjectPropertyType.MF_Collector:
                    return "Производство Коллектор";
                #endregion
                #region MF_Agregat
                case ProjectPropertyType.MF_Agregat:
                    return "Производство Расключение Агрегата";
                #endregion
                #region MF_SH
                case ProjectPropertyType.MF_SH:
                    return "Производство ШУ";
                #endregion
                #region MF_SH_Place
                case ProjectPropertyType.MF_SH_Place:
                    return "ИТО Место изготовления ШУ";
                #endregion
                #region MF_Complete_Percentage
                case ProjectPropertyType.MF_Complete_Percentage:
                    return "Производство прогресс";
                #endregion

                #region MF_Time_Test_Actual
                case ProjectPropertyType.MF_Time_Test_Actual:
                    return "Производство Тест (факт)";
                #endregion

                #region Requests_E
                case ProjectPropertyType.Requests_E:
                    return "Недокомплект электрика";
                #endregion
                #region Requests_G
                case ProjectPropertyType.Requests_G:
                    return "Недокомплект Гидравлика";
                #endregion
                #region Messagess
                case ProjectPropertyType.Messagess:
                    return "Коментарии";
                #endregion
                default:
                    return type.ToString();
            }
        }
        #endregion

        public void Dispose()
        {
            PropertyChanged = null;

            UserManager.UserChanged -= UserManager_UserChanged;
            TimeManager.DayChanged -= TimeManager_DayChanged;
        }

    }
}
