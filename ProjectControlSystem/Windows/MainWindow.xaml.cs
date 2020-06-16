using ArgDb;
using ArgLib.Logger;
using ArgLib.Windows;
using Microsoft.Win32;
using ProjectControlSystem.Controls;
using ProjectControlSystem.Managers;
using ProjectControlSystem.MFPlannerWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using about = ProjectControlSystem.Windows.AboutWindow;

namespace ProjectControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Поля
        const string unit = "MainWindow";

        readonly Dictionary<ProjectViewMode, UserControl> controls = new Dictionary<ProjectViewMode, UserControl>();
        ProjectViewMode vMode = ProjectViewMode.Unknown;

        LogWindow logWin = null;
        HelpWindow winHelp;

        AlarmWindow winAlarm;

        static readonly DispatcherTimer timerUpdate = new DispatcherTimer(), // Получение изменений по проектам
                                        timerReconnect = new DispatcherTimer();// попытка установки соединения раз в 10 секунд
        #endregion

        #region Свойства
        ProjectApplication App { get { return Application.Current as ProjectApplication; } }

        public ProjectViewMode ViewMode
        {
            get { return vMode; }
            set
            {
                if (vMode == value)
                    return;


                UpdateState(value);
            }
        }

        public string CurentUserName { get { return UserManager.Name; } }
        public string CurentUserGroup { get { return UserManager.Group; } }

        public bool IsConnected { get { return ServiceManager.IsConnected; } }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Настройки фильтра
            ProjectManager.FilterChecker = controlFilter;
            controlFilter.FilterConfirmed += controlFilter_FilterConfirmed;
            controlFilter.FilterCleared += controlFilter_FilterCleared;

            controls[ProjectViewMode.Main] = new ProjectsControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.Commercial] = new CommercialControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.ITO] = new ITOControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.Warehouse] = new WarehouseControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.OMTS] = new OMTSControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.Manufacture] = new ManufactureControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            controls[ProjectViewMode.OTK] = new OTKControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

            ViewMode = ProjectViewMode.Main;

            Closing += MainWindow_Closing;
            Loaded += MainWindow_Loaded;

            ProjectManager.ReloadProjectsAsync();

            // настройка обновлений
            LogManager.LogInfo(unit, string.Format("Запуск проверки обновлений проектов каждые {0} секунд", ProjectConfiguration.UpdateTimeSec));

            // Получение изменений по проектам
            timerUpdate.Interval = TimeSpan.FromSeconds(ProjectConfiguration.UpdateTimeSec);
            timerUpdate.Tick += timerUpdate_Tick;
            timerUpdate.Start();

            // попытка установки соединения раз в 10 секунд
            timerReconnect.Interval = TimeSpan.FromSeconds(10);
            timerReconnect.Tick += timerReconnect_Tick;

            ServiceManager.Disconnected += ServiceManager_Disconnected;
            ServiceManager.Connected += ServiceManager_Connected;

            UserManager.UserChanged += UserManager_UserChanged;

            // События экселя
            ExcelManager.ProgressChanged += ExcelManager_ProgressChanged;
            ExcelManager.Completed += ExcelManager_Completed;

            UpdateMessagePanel();

            InitMenu();

            LoadLayout();
        }

        void UpdateState(ProjectViewMode mode)
        {
            if (ViewMode == mode)
                return;

            vMode = mode;

            try
            {
                controlFilter.UpdateState(mode);

                Project pSelected = null;
                if (panelContent.Content is IProjectViewControl)
                    pSelected = ((IProjectViewControl)panelContent.Content).SelectedProject;

                //// Очищаем 
                //if (panelContent.Content is IProjectViewControl)
                //    ((IProjectViewControl)panelContent.Content).Unload();

                panelContent.Content = null;

                if (controls.ContainsKey(mode))
                {
                    var control = controls[mode];
                    panelContent.Content = control;

                    if (control is IProjectViewControl)
                    {
                        Title = ((IProjectViewControl)control).Header;
                        ((IProjectViewControl)control).Init();

                        if (pSelected != null)
                            ((IProjectViewControl)control).Select(pSelected.ProjectID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка переключения закладок", ex);
            }

            DoPropertyChanged("ViewMode");
        }

        // настройка панели оповещений
        void UpdateMessagePanel()
        {
            var grid = messagess.Parent as Grid;
            if (grid == null || grid.RowDefinitions.Count != 3)
                return;

            if (ProjectConfiguration.IsShowMessageMenu)
            {
                splitter.Visibility = messagess.Visibility = Visibility.Visible;
                grid.RowDefinitions[2].MinHeight = 100;
                grid.RowDefinitions[2].Height = new GridLength(100);
            }
            else
            {
                splitter.Visibility = messagess.Visibility = Visibility.Collapsed;
                grid.RowDefinitions[2].MinHeight = 0;
                grid.RowDefinitions[2].Height = new GridLength();
            }
        }

        #region Обработчики стандартныйх событий
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            TrayIconManager.SetDefault();

            if (winAlarm != null)
                winAlarm.Hide();
        }
        #endregion

        void InitMenu()
        {
            mnuMFPlanerWorkers.Visibility =
            mnuMFPlaner.Visibility = ((UserManager.Rights & UserRight.EditManufacture) == UserRight.EditManufacture) ? Visibility.Visible : Visibility.Collapsed;

            sepAdmin.Visibility =
            mnuEdit.Visibility =
            mnuRemove.Visibility = ((UserManager.Rights & UserRight.All) == UserRight.All) ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Обработчики событий
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            winAlarm = new AlarmWindow { Owner = this };
            winAlarm.SelectedChanged += msgControl_SelectedChanged;
            //winAlarm.Show();

            winHelp = new HelpWindow { Owner = this };
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            SaveLayout();
        }

        // Регистрация пользователя
        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var win = new LoginWindow { Owner = this };
            win.ShowDialog();

            DoPropertyChanged("CurentUserName");
            DoPropertyChanged("CurentUserGroup");
        }
        // Изменился пользователь
        void UserManager_UserChanged(object sender, EventArgs e)
        {
            InitMenu();
        }

        // перечитываем список 
        void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.ReloadProjectsAsync();
            DoPropertyChanged("IsConnected");
        }

        // открыть главное меню
        void btnOpenMain_Click(object sender, RoutedEventArgs e)
        {
            var win = new Window
            {
                Width = 1600,
                Height = 600,
                Title = "Главное меню",
                WindowStyle = System.Windows.WindowStyle.ToolWindow,
                ShowInTaskbar = true,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var content = new ProjectsControl { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            content.Init();

            win.Content = content;
            win.Show();
        }

        // Открываем выбранный проект
        void msgControl_SelectedChanged(object sender, SelectedChangedEventArgs e)
        {
            if ((UserManager.Rights & UserRight.EditCommercial) != 0)
                ViewMode = ProjectViewMode.Commercial;
            else if ((UserManager.Rights & UserRight.EditITO) != 0)
                ViewMode = ProjectViewMode.ITO;
            else if ((UserManager.Rights & UserRight.EditManufacture) != 0)
                ViewMode = ProjectViewMode.Manufacture;
            else if ((UserManager.Rights & UserRight.EditOMTS) != 0)
                ViewMode = ProjectViewMode.OMTS;
            else if ((UserManager.Rights & UserRight.EditOTK) != 0)
                ViewMode = ProjectViewMode.OTK;
            else if ((UserManager.Rights & UserRight.EditWarehouse) != 0)
                ViewMode = ProjectViewMode.Warehouse;


            if (panelContent.Content is IProjectViewControl)
                ((IProjectViewControl)panelContent.Content).Select(e.ID);

            Show();
            Activate();
        }

        // Добавить новый проект
        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((UserManager.Rights & UserRight.AddNewProject) == 0)
                MessageBox.Show("У пользователя '" + UserManager.Name + "' нет прав на создание проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
            else
                ProjectManager.CreateNewProject(this);
        }
        // Создаем проект из файла
        void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.CreateNewProjectsFromFile(this);
        }

        // Просмотр событий
        void btnLog_Click(object sender, RoutedEventArgs e)
        {
            if (logWin != null)
            {
                try { logWin.Close(); }
                catch { }
            }

            logWin = new LogWindow(App.Log) { Owner = this };
            logWin.Show();
        }

        // Открыть окно фильтра
        void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            controlFilter.Visibility = btnFilter.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        // Состояние соединения
        void ServiceManager_Disconnected(object sender, EventArgs e)
        {
            DoPropertyChanged("IsConnected");

            // Обновление остановлены, попытки пересоединения запущены
            timerReconnect.Start();
            timerUpdate.Stop();
        }
        void ServiceManager_Connected(object sender, EventArgs e)
        {
            DoPropertyChanged("IsConnected");

            timerReconnect.Stop();
            timerUpdate.Start();
        }

        // Excel
        //завершен импорт в Excel
        void ExcelManager_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            statusText.Visibility =
            progress.Visibility = Visibility.Collapsed;

            statusText.Text = "";
            progress.Value = 0;
        }
        //прогресс импорта в Excel
        void ExcelManager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusText.Visibility =
            progress.Visibility = Visibility.Visible;

            statusText.Text = e.UserState as string;
            progress.Value = e.ProgressPercentage;
        }

        // попытка установки соединения раз в 10 секунд
        void timerReconnect_Tick(object sender, EventArgs e)
        {
            timerReconnect.Stop();

            LogManager.LogInfo(unit, "Попытка соединения");

            ProjectManager.ReloadProjectsAsync();

            // запускаем таймер обновлений снова
            if (IsConnected)
                timerUpdate.Start();

            DoPropertyChanged("IsConnected");
        }
        // Получение изменений по проектам
        static void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();

            ProjectManager.TryCheckChangies();

            timerUpdate.Start();
        }

        // Настройки программы
        void btnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var win = new ConfigWindow { Owner = this };
            win.ShowDialog();

            // Если время запроса обновлений изменилось то перезапускаем таймер
            if (timerUpdate.Interval.Seconds != ProjectConfiguration.UpdateTimeSec)
            {
                timerUpdate.Interval = TimeSpan.FromSeconds(ProjectConfiguration.UpdateTimeSec);
                LogManager.LogInfo(unit, string.Format("Проверка обновлений проектов каждые {0} секунд", ProjectConfiguration.UpdateTimeSec));
            }

            UpdateMessagePanel();
        }

        // Редактирование проекта
        void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var control = panelContent.Content as IProjectViewControl;
            if (control == null || control.SelectedProject == null)
            {
                MessageBox.Show("Выберите элемент для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var win = new EditProjectWindow(control.SelectedProject) { Owner = this };
            if (win.ShowDialog() == true)
                ProjectManager.TryCheckChangies();
        }
        // о программе
        void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<about.LibInfo>();
            list.AddRange(ProjectApplication.libInfo);

            // загружаем иформацию о сервере
            var sVersion = ServiceManager.GetServerVersion();
            if (!string.IsNullOrEmpty(sVersion))
            {
                var info = new about.LibInfo("AgrServer.exe", "Сервер проектов 'Агрегат'", sVersion, null);
                list.Add(info);
            }

            var win = new AboutWindow(list) { Owner = this };
            win.ShowDialog();
        }
        // Показать документацию
        void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            winHelp.Show();
            winHelp.Activate();
        }

        // Удаление Проекта
        void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var control = panelContent.Content as IProjectViewControl;
            if (control == null || control.SelectedProject == null)
            {
                MessageBox.Show("Выберите элемент для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var p = control.SelectedProject;

            var result = MessageBox.Show(string.Format("Уверены что хотите удалить проект: {0}\n{1}\n{2}\n{3}", p.ID, p.Customer, p.Product, p.Options),
                                            "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            // Если удаление успешно то обновляем проекты
            if (ProjectManager.TryRemoveProject(p.ProjectID))
            {
                ProjectManager.ReloadProjectsAsync();
                return;
            }

            MessageBox.Show("Не удалось удалить проект", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        // Отмена последнего изменения
        void btnRevert_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Уверены что хотите отменить последнее действие", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            if (ProjectManager.Revert())
                ProjectManager.ReloadProjectsAsync();
        }
        // Импорт в Excel
        void btnToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelManager.ToExcel(ViewMode);
        }
        // сохранить выбранный проект в Excel
        void btnProjectToExcel_Click(object sender, RoutedEventArgs e)
        {
            var control = panelContent.Content as IProjectViewControl;
            if (control == null || control.SelectedProject == null)
                return;

            ExcelManager.ToExcel(control.SelectedProject);
        }

        // Закрыть программу
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /* Фильтр */
        // установлен
        void controlFilter_FilterConfirmed(object sender, EventArgs e)
        {
            ProjectManager.UpdateProjects();
        }
        // очищаем 
        void controlFilter_FilterCleared(object sender, EventArgs e)
        {
            ProjectManager.ResetFilter();
        }

        /* Планирование производства*/
        // проекты
        void btnManufacturePlanner_Click(object sender, RoutedEventArgs e)
        {
            var win = new MFPlannerWindow { Owner = this };
            win.ShowDialog();
        }
        // работники
        void btnmnuMFPlanerWorkers_Click(object sender, RoutedEventArgs e)
        {
            var win = new MFPlannerWorkersWindow { Owner = this };
            win.ShowDialog();
        }
        #endregion

        #region Настройки
        static string GetConfigPath()
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(Application.Current.GetType().Assembly.Location);
            return ProjectConfiguration.GetConfigFile(fileName);
        }

        void SaveLayout()
        {
            try
            {
                var doc = new XDocument();
                doc.Add(new XElement(XName.Get("LAYOUT")));

                doc.Root.Add(new XElement[]
                        {
                            new XElement(XName.Get("Left"), (int)this.Left),
                            new XElement(XName.Get("Top"), (int)this.Top),
                            new XElement(XName.Get("Width"), (int)this.Width),
                            new XElement(XName.Get("Height"), (int)this.Height)
                        });

                var path = GetConfigPath();
                doc.Save(path);
            }
            catch { }
        }
        void LoadLayout()
        {
            try
            {
                var path = GetConfigPath();
                if (!File.Exists(path))
                    return;

                var doc = XDocument.Load(path);
                if (doc.Root == null || doc.Root.Name.LocalName != "LAYOUT" || !doc.Root.HasElements)
                    return;

                foreach (XElement xElement in doc.Root.Elements())
                {
                    switch (xElement.Name.LocalName)
                    {
                        #region Left
                        case "Left":
                            {
                                var v = 0;
                                if (int.TryParse(xElement.Value, out v))
                                    this.Left = v;

                                break;
                            }
                        #endregion
                        #region Top
                        case "Top":
                            {
                                var v = 0;
                                if (int.TryParse(xElement.Value, out v))
                                    this.Top = v;

                                break;
                            }
                        #endregion
                        #region Width
                        case "Width":
                            {
                                var v = 0;
                                if (int.TryParse(xElement.Value, out v))
                                    this.Width = v;

                                break;
                            }
                        #endregion
                        #region Height
                        case "Height":
                            {
                                var v = 0;
                                if (int.TryParse(xElement.Value, out v))
                                    this.Height = v;

                                break;
                            }
                            #endregion
                    }
                }
            }
            catch { }
        }
        #endregion

        #region События.
        public event PropertyChangedEventHandler PropertyChanged;
        private void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
