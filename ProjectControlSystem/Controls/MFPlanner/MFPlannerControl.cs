using ArgDb;
using ArgLib.Logger;
using ProjectControlSystem.Managers;
using ProjectControlSystem.MFPlannerWindows;
using ProjectControlSystem.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProjectControlSystem
{
    public class MFPlannerControl : Canvas
    {
        #region Поля
        const string unit = "MFPlannerControl";

        bool isAddActionInProgress = false,
             isShowEndWorkWokers = true;

        Rect rectGrid = Rect.Empty;

        DragMode mode = DragMode.None;
        bool isDown = false,
             isDirty = true; // флаг обновления элементов

        static Pen penGrid;
        static readonly Typeface typeface = new Typeface("Tahoma");
        static readonly Brush brushSaturdaySunday,
                              brushSelection,
                              brushNow,
                              brushEndWork;

        double rowHeight = 50,
               rowMonthAndDay = 20,
               mouseStartMoveX = 0;

        // таймер перерисовки
        readonly DispatcherTimer timerUpdateAction = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        // время
        DateTime timeStart = DateTime.Now.Date.AddDays(-12),
                 timeEnd = DateTime.Now.Date.AddDays(15);

        // Элементы id action/control
        readonly Dictionary<int, MFPlannerProjectControl> controlsCache = new Dictionary<int, MFPlannerProjectControl>();
        #endregion

        #region Свойства
        public DateTime TimeStart
        {
            get { return timeStart; }
            set
            {
                if (timeStart == value)
                    return;

                timeStart = value;

                lock (timerUpdateAction)
                    timerUpdateAction.Start();
            }
        }
        public DateTime TimeEnd
        {
            get { return timeEnd; }
            set
            {
                if (timeEnd == value)
                    return;

                timeEnd = value;

                lock (timerUpdateAction)
                    timerUpdateAction.Start();
            }
        }

        /// <summary>Показывать уволенных</summary>
        public bool IsShowEndWorkWokers
        {
            get { return isShowEndWorkWokers; }
            set
            {
                if (isShowEndWorkWokers == value)
                    return;

                isShowEndWorkWokers = value;

                isDirty = true;
                InvalidateVisual();
            }
        }
        #endregion

        static MFPlannerControl()
        {
            penGrid = new Pen(Brushes.Gray, 1);
            penGrid.Freeze();

            brushSaturdaySunday = new SolidColorBrush(Color.FromArgb(50, Colors.Brown.R, Colors.Brown.G, Colors.Brown.B));
            ((SolidColorBrush)brushSaturdaySunday).Freeze();

            brushSelection = new SolidColorBrush(Color.FromArgb(100, Colors.LightSkyBlue.R, Colors.LightSkyBlue.G, Colors.LightSkyBlue.B));
            ((SolidColorBrush)brushSelection).Freeze();

            brushNow = new SolidColorBrush(Color.FromArgb(50, 100, 230, 150));
            ((SolidColorBrush)brushNow).Freeze();

            brushEndWork = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            ((SolidColorBrush)brushEndWork).Freeze();
        }
        public MFPlannerControl()
        {
            try
            {
                // контекстное меню
                ContextMenu = new ContextMenu();
                ContextMenu.Opened += ContextMenu_Opened;

                var mnu = new MenuItem { Header = "Проект", Tag = MFWorkerActionType.Project };
                mnu.Click += mnu_Click;
                ContextMenu.Items.Add(mnu);

                mnu = new MenuItem { Header = "Резервирование проекта", Tag = MFWorkerActionType.ReseveProject };
                mnu.Click += mnu_Click;
                ContextMenu.Items.Add(mnu);

                ContextMenu.Items.Add(new Separator());

                mnu = new MenuItem { Header = "Простой", Tag = MFWorkerActionType.Avait };
                mnu.Click += mnu_Click;
                ContextMenu.Items.Add(mnu);

                ContextMenu.Items.Add(new Separator());

                mnu = new MenuItem { Header = "Отпуск", Tag = MFWorkerActionType.Holiday };
                mnu.Click += mnu_Click;
                ContextMenu.Items.Add(mnu);

                mnu = new MenuItem { Header = "Больничный", Tag = MFWorkerActionType.Hospital };
                mnu.Click += mnu_Click;
                ContextMenu.Items.Add(mnu);

                ClipToBounds = true;

                timerUpdateAction.Tick += timerUpdateAction_Tick;

                MFPlannerManager.WorkersChanged += MFPlannerManager_WorkersChanged;
                MFPlannerManager.ActionsChanged += MFPlannerManager_ActionsChanged;

                MFPlannerManager.ReloadWorkersAsync();
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка инициализации окна планировщика производства", ex);
            }
        }

        /// <summary>Выполняем запрос от проекта</summary>
        internal void MakeCommand(PlannerMode mode, Point pos, MFPlannerProjectControl sender)
        {
            MFWorker worker = null;
            DateTime? time = null;

            GetCell(pos, out worker, out time);
            if (worker == null || time == null)
                return;

            switch (mode)
            {
                #region PlanProject_Confirm - Подтвердить проект
                case PlannerMode.PlanProject_Confirm:
                    {
                        // Выбираем проект
                        var win = new MFPlannerSelectProjectWindow(TimeStart, TimeEnd) { Owner = Window.GetWindow(this) };
                        if (win.ShowDialog() != true || win.SelectedProject == null)
                            return;

                        sender.Action.TargetId = win.SelectedProject.ProjectID;
                        sender.Action.Type = MFWorkerActionType.Project;

                        if (!MFPlannerManager.MFPlannerChangeAction(sender.Action))
                        {
                            sender.Action.TargetId = -1;
                            sender.Action.Type = MFWorkerActionType.ReseveProject;

                            MessageBox.Show("Не удадось подтвердить проект", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        // Устанавливаем пост
                        var sPost = CreatePost(win.SelectedProject.MF_Post, worker.Post.ToString());
                        if (win.SelectedProject.MF_Post != sPost)
                            ProjectManager.ChangeProjectPropety(win.SelectedProject.ProjectID, ProjectPropertyType.MF_Post, sPost);

                        // Устанавливаем ориентировочное время 
                        var dateMFPlaned = sender.Action.TimeEnd;
                        var isChange = true;
                        if (win.SelectedProject.MF_Time_Plan.HasValue)
                        {
                            var msg = string.Format("Изменить 'Планируемое время завершения производства'\n с {0} на {1}",
                                win.SelectedProject.MF_Time_Plan.Value.ToString(ProjectConfiguration.DateFormat),
                                dateMFPlaned.ToString(ProjectConfiguration.DateFormat));

                            isChange = MessageBox.Show(msg, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                        }

                        if (isChange)
                            ProjectManager.ChangeProjectPropety(win.SelectedProject.ProjectID, ProjectPropertyType.MF_Time_Planed, dateMFPlaned);

                        break;
                    }
                #endregion

                #region CommentEdit
                case PlannerMode.CommentEdit:
                    {
                        var win = new MFPlannerCommentWindow { Title = "Изменить комментарий", Comment = sender.Action.Comment, Owner = Window.GetWindow(this) };
                        if (win.ShowDialog() != true)
                            return;

                        var prev = sender.Action.Comment;
                        sender.Action.Comment = win.Comment;

                        if (!MFPlannerManager.MFPlannerChangeAction(sender.Action))
                        {
                            sender.Action.Comment = prev;
                            MessageBox.Show("Не удалось изменить описание проекта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        sender.InvalidateVisual();

                        break;
                    }
                #endregion

                #region ChangeDays - Изменить продолжительность дней
                case PlannerMode.ChangeDays:
                    {
                        var res = MessageBox.Show(string.Format("Изменить продолжительность дней?"), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (res != MessageBoxResult.Yes)
                            return;

                        var winDays = new DaysWindow(sender.Action.Days) { Owner = Window.GetWindow(this) };
                        if (winDays.ShowDialog() != true)
                            return;

                        var prev = sender.Action.Days;
                        sender.Action.Days = winDays.Count;

                        // изменяем
                        if (!MFPlannerManager.MFPlannerChangeAction(sender.Action))
                        {
                            sender.Action.Days = prev;

                            MessageBox.Show("Не удалось сохранить изменения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        // Устанавливаем ориентировочное время 
                        ProjectManager.ChangeProjectPropety(sender.Action.TargetId, ProjectPropertyType.MF_Time_Planed, sender.Action.TimeEnd);

                        isDirty = true;

                        break;
                    }
                #endregion
                #region Break - Прервать
                case PlannerMode.Break:
                    {
                        var days = (time.Value - sender.Action.TimeBegin).TotalDays;
                        if (days <= 1)
                            return;

                        var res = MessageBox.Show(string.Format("Сократить проект до: {0} дней?", days), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (res != MessageBoxResult.Yes)
                            return;

                        var prev = sender.Action.Days;
                        sender.Action.Days = (int)days;

                        // изменяем
                        if (!MFPlannerManager.MFPlannerChangeAction(sender.Action))
                        {
                            sender.Action.Days = prev;

                            MessageBox.Show("Не удалось сохранить изменения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        // Устанавливаем ориентировочное время 
                        ProjectManager.ChangeProjectPropety(sender.Action.TargetId, ProjectPropertyType.MF_Time_Planed, sender.Action.TimeEnd);

                        isDirty = true;

                        break;
                    }
                #endregion
                #region Remove - удаление
                case PlannerMode.Remove:
                    {
                        var res = MessageBox.Show(string.Format("Удалить выбранный?"), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (res != MessageBoxResult.Yes)
                            return;

                        // удаляем
                        if (!MFPlannerManager.RemoveAction(sender.Action))
                        {
                            MessageBox.Show("Удаление не удалось", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        var cProject = controlsCache[sender.Action.Id];

                        controlsCache.Remove(sender.Action.Id);
                        Children.Remove(cProject);

                        // Удаляем пост
                        var p = ServiceManager.LoadProject(sender.Action.TargetId);
                        var sPost = RemovePost(p.MF_Post, worker.Post.ToString());
                        if (p.MF_Post != sPost)
                            ProjectManager.ChangeProjectPropety(sender.Action.TargetId, ProjectPropertyType.MF_Post, sPost);

                        cProject.Dispose();
                        cProject = null;

                        break;
                    }
                #endregion

                #region ChangeProgressNone/ChangeProgressStart/ChangeProgressHalf/ChangeProgressFinish
                case PlannerMode.ChangeProgressNone:
                case PlannerMode.ChangeProgressStart:
                case PlannerMode.ChangeProgressHalf:
                case PlannerMode.ChangeProgressFinish:
                    {
                        var name = "";
                        if (mode == PlannerMode.ChangeProgressStart)
                            name = Project.inStart;
                        else if (mode == PlannerMode.ChangeProgressHalf)
                            name = Project.inHalf;
                        else if (mode == PlannerMode.ChangeProgressFinish)
                            name = Project.inFinish;

                        // Удаляем пост
                        var p = ServiceManager.LoadProject(sender.Action.TargetId);
                        if (p.MF_Complete_Percentage != name)
                            ProjectManager.ChangeProjectPropety(sender.Action.TargetId, ProjectPropertyType.MF_Complete_Percentage, name);

                        break;
                    }
                #endregion
            }
        }

        /// <summary>Возвращает информацию по ячейке</summary>
        void GetCell(Point pos, out  MFWorker worker, out  DateTime? t)
        {
            worker = null;
            t = null;
            if (rectGrid == Rect.Empty)
                return;

            var workers = MFPlannerManager.GetWorkers(IsShowEndWorkWokers);
            var index = 0;
            for (; index < workers.Count; index++)
            {
                var rectTmp = new Rect(0, rectGrid.Y + rowHeight * index, rectGrid.Right, rowHeight);
                if (rectTmp.Contains(pos))
                    worker = workers[index];
            }

            var d = TimeStart;

            var cellWidth = rectGrid.Width / (TimeEnd - TimeStart).TotalDays;
            index = 0;
            while (d < TimeEnd)
            {
                var rectTmp = new Rect(rectGrid.X + index * cellWidth, rectGrid.Top - rowMonthAndDay, cellWidth, rectGrid.Height + rowMonthAndDay);
                if (rectTmp.Contains(pos))
                {
                    t = d;
                    return;
                }

                d = d.AddDays(1);
                index++;
            }
        }

        #region Отрисовка
        void Rendraw(DrawingContext dc)
        {
            if (double.IsNaN(ActualWidth) || double.IsNaN(ActualHeight))
                return;

            var t0 = DateTime.Now;

            FormattedText txt = null;
            int index = 0;
            var posMouse = Mouse.GetPosition(this);

            rectGrid = new Rect(0, (rowMonthAndDay * 2 + 3), ActualWidth, ActualHeight - (rowMonthAndDay * 2 + 3));
            Rect rectTmp = Rect.Empty;

            // фон
            dc.DrawRectangle(Brushes.White, penGrid, new Rect(0, 0, ActualWidth, ActualHeight));

            #region  рисуем шкалу человеков
            var workers = MFPlannerManager.GetWorkers(IsShowEndWorkWokers);
            double colWorkerWidth = 100;
            for (index = 0; index < workers.Count; index++)
            {
                var name = string.Format("{0} {1} [{2}]", workers[index].SecondName, workers[index].Name, workers[index].Post);

                // Если уволенный
                if (workers[index].EndWorkTime.HasValue)
                {
                    dc.DrawRectangle(brushEndWork, null, new Rect(0, rectGrid.Y + rowHeight * index, rectGrid.Right, rowHeight));
                    name += string.Format("\n(уволен)");
                }

                txt = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 14, Brushes.Black);

                if (colWorkerWidth < txt.Width + 10)
                    colWorkerWidth = txt.Width + 10;

                var nameHeight = txt.Height + 2;

                // фамиля + имя
                dc.DrawText(txt, new Point(5, rectGrid.Y + rowHeight * index + (rowHeight - nameHeight) / 2));

                //name = string.Format("пост: {0}", workers[index].Post);
                //txt = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 11, Brushes.Gray);
                //dc.DrawText(txt, new Point((colWorkerWidth - txt.Width) / 2, rectGrid.Y + rowHeight * index + (rowHeight - nameHeight) / 2 + nameHeight));

                dc.DrawLine(penGrid, new Point(0, rectGrid.Y + rowHeight * index), new Point(rectGrid.Right, rectGrid.Y + rowHeight * index));

                // Выделение для мыши
                rectTmp = new Rect(0, rectGrid.Y + rowHeight * index, rectGrid.Right, rowHeight);
                if (rectTmp.Contains(posMouse))
                    dc.DrawRectangle(brushSelection, null, rectTmp);
            }

            dc.DrawLine(penGrid, new Point(0, rectGrid.Y + rowHeight * (index)), new Point(rectGrid.Right, rectGrid.Y + rowHeight * (index)));
            #endregion

            // максимальное имя
            rectGrid.X = colWorkerWidth;
            rectGrid.Width -= colWorkerWidth;
            rectGrid.Height = 50 * workers.Count;

            var cellWidth = rectGrid.Width / (TimeEnd - TimeStart).TotalDays;

            #region Рисуем шкалу дат
            DateTime? datePrev = null;
            var d = TimeStart;
            var now = DateTime.Now.Date;

            index = 0;
            while (d < TimeEnd)
            {
                // если у нас выходные
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                {
                    rectTmp = new Rect(rectGrid.X + index * cellWidth, rectGrid.Top - rowMonthAndDay, cellWidth, rectGrid.Height + rowMonthAndDay);
                    dc.DrawRectangle(brushSaturdaySunday, null, rectTmp);
                }

                // текущий день
                if (d.Date == now)
                {
                    rectTmp = new Rect(rectGrid.X + index * cellWidth, rectGrid.Top - rowMonthAndDay, cellWidth, rectGrid.Height + rowMonthAndDay);
                    dc.DrawRectangle(brushNow, null, rectTmp);
                }

                // месяц изменился полная линия
                if (!datePrev.HasValue || (datePrev.Value.Month != d.Month))
                {
                    txt = new FormattedText(d.ToString("MMMM"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);
                    dc.DrawText(txt, new Point(rectGrid.X + index * cellWidth + 5, 2));

                    dc.DrawLine(penGrid, new Point(rectGrid.X + index * cellWidth, 0), new Point(rectGrid.X + index * cellWidth, rectGrid.Bottom));
                }
                else // обычная линия
                {
                    dc.DrawLine(penGrid, new Point(rectGrid.X + index * cellWidth, rectGrid.Y - rowMonthAndDay), new Point(rectGrid.X + index * cellWidth, rectGrid.Bottom));
                }

                // рисуем день                
                txt = new FormattedText(d.ToString("dd"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 11, Brushes.Black);
                if (cellWidth >= txt.Width)
                    dc.DrawText(txt, new Point(rectGrid.X + index * cellWidth + (cellWidth - txt.Width) / 2, rowMonthAndDay + 6));



                // Выделение для мыши
                rectTmp = new Rect(rectGrid.X + index * cellWidth, rectGrid.Top - rowMonthAndDay, cellWidth, rectGrid.Height + rowMonthAndDay);
                if (rectTmp.Contains(posMouse))
                {
                    dc.DrawRectangle(brushSelection, null, rectTmp);

                    // отрисовываем дату шкалы
                    txt = new FormattedText(d.ToString("dd MMMM yyyy"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);

                    rectTmp = new Rect(rectGrid.X + index * cellWidth, rectGrid.Bottom, txt.Width + 10, rowMonthAndDay);
                    dc.DrawRectangle(brushSelection, null, rectTmp);

                    dc.DrawText(txt, new Point(rectTmp.Left + 5, rectTmp.Top));
                }

                datePrev = d;
                d = d.AddDays(1);
                index++;
            }

            // Горизонтальная линия
            // дата
            dc.DrawLine(penGrid, new Point(rectGrid.Left, rowMonthAndDay + 3), new Point(rectGrid.Right, rowMonthAndDay + 3));
            //dc.DrawLine(penGrid, new Point(0, rowMonthAndDay * 2 + 3), new Point(rect.Right, rowMonthAndDay * 2 + 3));
            #endregion

            // Добавляем проекты
            if (isDirty)
            {
                var dictionary = new Dictionary<int, List<ActionTemp>>();
                List<MFPlannerAction> actions = MFPlannerManager.GetActions();
                foreach (MFPlannerAction action in actions)
                {
                    if (!dictionary.ContainsKey(action.WorkerId))
                        dictionary[action.WorkerId] = new List<ActionTemp>();

                    //if ((a.TimeBegin.Date >= TimeStart && a.TimeBegin.Date <= TimeEnd)
                    //    || (a.TimeEnd.Date >= TimeStart && a.TimeEnd.Date <= TimeEnd))
                    {
                        for (int indexWorker = 0; indexWorker < workers.Count; indexWorker++)
                        {
                            if (workers[indexWorker].Id == action.WorkerId)
                            {
                                double sIndex = (int)(action.TimeBegin.Date - TimeStart).TotalDays,
                                       eIndex = (int)(action.TimeEnd.Date - TimeStart).TotalDays;

                                double x = Math.Max(rectGrid.X, rectGrid.X + sIndex * cellWidth),
                                       w = (eIndex - Math.Max(0, sIndex) + 1) * cellWidth;
                                if (w > 0 && x < rectGrid.Right)
                                {
                                    rectTmp = new Rect(x, rectGrid.Y + rowHeight * indexWorker, w, rowHeight);
                                    rectTmp.Inflate(0, -1);
                                }
                                else
                                    rectTmp = Rect.Empty;

                                dictionary[action.WorkerId].Add(new ActionTemp { Action = action, Rect = rectTmp });
                            }
                        }
                    }
                }

                // выравниваем
                foreach (var kv in dictionary)
                {
                    foreach (var a in kv.Value)
                    {
                        var intersectItems = kv.Value.Where(x => x != a && x.Rect.IntersectsWith(a.Rect) && x.Rect != Rect.Empty);
                        rectTmp = a.Rect;
                        double y = rectTmp.Y,
                               count = intersectItems.Count();
                        if (count > 0)
                        {
                            rectTmp.Height = rectTmp.Height / (count + 1);
                            rectTmp.Y = y;

                            y += rectTmp.Height + 0.5;
                            a.Rect = rectTmp;
                        }

                        foreach (var item in intersectItems)
                        {
                            rectTmp = item.Rect;
                            rectTmp.Height = rectTmp.Height / (count + 1);
                            rectTmp.Y = y;

                            item.Rect = rectTmp;
                            y += rectTmp.Height + 0.5;
                        }
                    }
                }

                foreach (var v in dictionary)
                {
                    // Анализируем положения
                    foreach (var a in v.Value)
                    {
                        MFPlannerProjectControl cProject = null;

                        if (!controlsCache.ContainsKey(a.Action.Id))
                        {
                            cProject = new MFPlannerProjectControl { Tag = a.Action, Width = a.Rect.Width, Height = a.Rect.Height };
                            cProject.InitContextMenu();

                            controlsCache[a.Action.Id] = cProject;
                            Children.Add(cProject);
                        }
                        else
                            cProject = controlsCache[a.Action.Id];

                        // видимость
                        //cProject.Visibility = rectGrid.IntersectsWith(a.Rect) ? Visibility.Visible : Visibility.Collapsed;
                        if (a.Rect == Rect.Empty)
                        {
                            cProject.Visibility = Visibility.Collapsed;
                            continue;
                        }

                        cProject.Visibility = Visibility.Visible;

                        var h = a.Rect.Height;
                        //// уменьшаем
                        //var count = v.Value.Count(x => x != a && x.Rect.IntersectsWith(a.Rect));
                        //if (count >= 1)
                        //{ 
                        //    h = h / (count + 1);
                        //}

                        cProject.Width = a.Rect.Width;
                        cProject.Height = h;
                        Canvas.SetLeft(cProject, a.Rect.Left);
                        Canvas.SetTop(cProject, a.Rect.Top);
                        cProject.InvalidateVisual();
                    }
                }


                isDirty = false;
            }

            //var res = (DateTime.Now - t0).TotalMilliseconds;
            //txt = new FormattedText(res.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);
            //dc.DrawText(txt, new Point(0, 0));
        }
        #endregion

        #region Стандартные обработчики событий
        // Отрисовка
        protected override void OnRender(DrawingContext dc)
        {
            // не перерисовываем при поднятых окнах
            // или открытом контектeсном меню
            var win = Window.GetWindow(this);
            if (ContextMenu.IsOpen || isAddActionInProgress || win.OwnedWindows.Count > 0)
                return;

            Rendraw(dc);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var pos = e.GetPosition(this);
            var rHeader = new Rect(0, 0, ActualWidth, rowMonthAndDay);
            if (rHeader.Contains(pos))
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Arrow;

            if (isDown)
            {
                switch (mode)
                {
                    #region Move
                    case DragMode.Move:
                        {
                            if (mouseStartMoveX - pos.X > 20)
                            {
                                TimeStart = TimeStart.AddDays(1);
                                TimeEnd = TimeEnd.AddDays(1);

                                mouseStartMoveX = pos.X;

                                isDirty = true;
                            }
                            else if (mouseStartMoveX - pos.X < -20)
                            {
                                TimeStart = TimeStart.AddDays(-1);
                                TimeEnd = TimeEnd.AddDays(-1);

                                mouseStartMoveX = pos.X;

                                isDirty = true;
                            }

                            break;
                        }
                    #endregion
                }
            }

            InvalidateVisual();
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var time = TimeEnd - TimeStart;
            if (e.Delta < 0)
            {
                if (time.Days > 182)
                    return;

                TimeStart = TimeStart.AddDays(-1);
                TimeEnd = TimeEnd.AddDays(1);
            }
            else
            {
                if (time.Days < 7)
                    return;

                TimeStart = TimeStart.AddDays(1);
                TimeEnd = TimeEnd.AddDays(-1);
            }

            isDirty = true;
            InvalidateVisual();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var pos = e.GetPosition(this);
            var rHeader = new Rect(0, 0, ActualWidth, rowMonthAndDay);

            if (rHeader.Contains(pos))
            {
                mouseStartMoveX = pos.X;
                isDown = true;
                mode = DragMode.Move;

                this.CaptureMouse();
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            isDown = false;
            mode = DragMode.None;

            this.ReleaseMouseCapture();
        }

        // Размеры
        protected override Size MeasureOverride(Size constraint)
        {
            var workers = MFPlannerManager.GetWorkers(IsShowEndWorkWokers);
            return new Size(constraint.Width, workers.Count * rowHeight + rowMonthAndDay * 3);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            isDirty = true;

            //    base.OnRenderSizeChanged(sizeInfo);

            //    if (double.IsNaN(ActualWidth) || double.IsNaN(ActualHeight))
            //        return;
            //    Rect rect = new Rect(0, (rowMonthAndDay * 2 + 3), ActualWidth, ActualHeight - (rowMonthAndDay * 2 + 3));

            //    // рисуем шкалу человеков
            //    var workers = PlannedManager.GetWorkers();
            //    double colWorkerWidth = 100;
            //    for (var index = 0; index < workers.Length; index++)
            //    {
            //        var name = string.Format("{0} ({1})", workers[index].Name, workers[index].Post);
            //        var txt = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);

            //        if (colWorkerWidth < txt.Width + 10)
            //            colWorkerWidth = txt.Width + 10;
            //    }

            //    // максимальное имя
            //    rect.X = colWorkerWidth;
            //    rect.Width -= colWorkerWidth;
            //    rect.Height = 50 * workers.Length;

            //    cellWidth = rect.Width / ((TimeEnd - TimeStart).TotalDays);
        }
        #endregion

        #region Обработчики событий
        void mnu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAddActionInProgress = true;

                var pos = (Point)ContextMenu.Tag;

                MFWorker worker = null;
                DateTime? date = null;
                GetCell(pos, out worker, out date);
                if (worker == null || !date.HasValue)
                    return;

                // Если уволен то выходим
                if (worker.EndWorkTime.HasValue)
                {
                    MessageBox.Show("Невозможно дать задание уволенному работнику", "Внимание", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                var t = (MFWorkerActionType)((FrameworkElement)sender).Tag;

                // Количество дней
                var winDays = new DaysWindow(14) { Owner = Window.GetWindow(this) };
                if (winDays.ShowDialog() != true)
                    return;

                switch (t)
                {
                    #region Holiday/Hospital
                    case MFWorkerActionType.Holiday:
                    case MFWorkerActionType.Hospital:
                        {
                            MFPlannerManager.CreateNewAction(t, worker.Id, -1, date.Value, winDays.Count);
                            break;
                        }
                    #endregion
                    #region Project
                    case MFWorkerActionType.Project:
                        {
                            // Выбираем проект
                            var win = new MFPlannerSelectProjectWindow(TimeStart, TimeEnd) { Owner = Window.GetWindow(this) };
                            if (win.ShowDialog() != true || win.SelectedProject == null)
                                return;

                            MFPlannerManager.CreateNewAction(t, worker.Id, win.SelectedProject.ProjectID, date.Value, winDays.Count);

                            // Устанавливаем пост
                            var sPost = CreatePost(win.SelectedProject.MF_Post, worker.Post.ToString());
                            if (win.SelectedProject.MF_Post != sPost)
                                ProjectManager.ChangeProjectPropety(win.SelectedProject.ProjectID, ProjectPropertyType.MF_Post, sPost);

                            // Устанавливаем ориентировочное время 
                            var dateMFPlaned = date.Value.AddDays(winDays.Count);
                            var isChange = true;
                            if (win.SelectedProject.MF_Time_Plan.HasValue)
                            {
                                var msg = string.Format("Изменить 'Планируемое время завершения производства'\n с {0} на {1}",
                                    win.SelectedProject.MF_Time_Plan.Value.ToString(ProjectConfiguration.DateFormat),
                                    dateMFPlaned.ToString(ProjectConfiguration.DateFormat));

                                isChange = MessageBox.Show(msg, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                            }

                            if (isChange)
                                ProjectManager.ChangeProjectPropety(win.SelectedProject.ProjectID, ProjectPropertyType.MF_Time_Planed, dateMFPlaned);

                            //actions.Add(new MFPlannerAction { Type = t, WorkerId = worker.Id, TimeBegin = date.Value, Days = winDays.Count, Id = actions.Count + 1, TargetId = win.SelectedProject.ProjectID });
                            break;
                        }
                    #endregion
                    #region ReseveProject
                    case MFWorkerActionType.ReseveProject:
                        {
                            var win = new MFPlannerCommentWindow { Owner = Window.GetWindow(this) };
                            if (win.ShowDialog() != true)
                                return;

                            MFPlannerManager.CreateNewAction(t, worker.Id, -1, date.Value, winDays.Count, win.Comment);

                            break;
                        }
                    #endregion
                    #region Avait
                    case MFWorkerActionType.Avait:
                        {
                            MFPlannerManager.CreateNewAction(t, worker.Id, -1, date.Value, 1);
                            break;
                        }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(unit, "Ошибка добавления нового действия в проект", ex);
            }
            finally
            {
                isAddActionInProgress = false;

                //projects[1].MF_Post = worker.Post.ToString();
                isDirty = true;
                InvalidateVisual();
            }
        }

        // таймер запроса обновления данных
        void timerUpdateAction_Tick(object sender, EventArgs e)
        {
            lock (timerUpdateAction)
                timerUpdateAction.Stop();

            MFPlannerManager.ReloadActionsAsync(TimeStart, TimeEnd);
        }

        // Контекстное меню открыто
        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu.Tag = Mouse.GetPosition(this);
        }

        // Событие изменения работников
        void MFPlannerManager_WorkersChanged(object sender, EventArgs e)
        {
            isDirty = true;
            InvalidateVisual();

            MFPlannerManager.ReloadActionsAsync(TimeStart, TimeEnd);
        }

        // Изменился список действий для пользователя
        void MFPlannerManager_ActionsChanged(object sender, EventArgs e)
        {
            isDirty = true;
            InvalidateVisual();
        }
        #endregion

        #region Вспомогательные классы
        enum DragMode
        {
            None,
            Move
        }
        class ActionTemp
        {
            public MFPlannerAction Action { get; set; }
            public Rect Rect { get; set; }
        }
        #endregion

        #region Методы
        /// <summary>Создаем пост</summary>
        static string CreatePost(string projectPost, string workerPost)
        {
            var sPost = projectPost;

            // устанавливаем пост
            if (!string.IsNullOrEmpty(sPost))
            {
                var s = ' ';
                string[] items = null;
                var separators = new char[] { ' ', ',', ';' };
                foreach (var c in separators)
                {
                    if (sPost.Contains(c))
                    {
                        s = c;
                        items = sPost.Split(c);
                        break;
                    }
                }

                if (items != null)
                {
                    var isAdd = true;
                    sPost = "";

                    foreach (var p in items)
                    {
                        // Если такой пост уже есть
                        if (p.Trim() == workerPost)
                            return projectPost;

                        if (string.IsNullOrEmpty(sPost))
                            sPost = p;
                        else
                            sPost += "," + p.Trim();
                    }

                    // добавляем новый пост
                    if (isAdd)
                    {
                        if (string.IsNullOrEmpty(sPost))
                            sPost = workerPost;
                        else
                            sPost += "," + workerPost;
                    }
                }
                else
                    sPost = string.Format("{0},{1}", sPost, workerPost);
            }
            else
                sPost = workerPost;

            return sPost;
        }
        static string RemovePost(string projectPost, string workerPost)
        {
            var sPost = projectPost;


            if (string.IsNullOrEmpty(sPost))
                return sPost;

            var s = ' ';
            string[] items = null;
            var separators = new char[] { ' ', ',', ';' };
            foreach (var c in separators)
            {
                if (sPost.Contains(c))
                {
                    s = c;
                    items = sPost.Split(c);
                    break;
                }
            }

            if (items == null)
                return sPost;

            sPost = "";
            foreach (var p in items)
            {
                // Если такой пост уже есть пропускаем
                if (p.Trim() == workerPost)
                    continue;

                if (string.IsNullOrEmpty(sPost))
                    sPost = p;
                else
                    sPost += "," + p.Trim();
            }

            return sPost;
        }
        #endregion
    }
}
