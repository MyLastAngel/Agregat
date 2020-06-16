using ArgDb;
using ProjectControlSystem.Controls;
using ProjectControlSystem.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ProjectControlSystem
{
    internal class MFPlannerProjectControl : Canvas
    {
        #region Поля
        bool isInitialized = false;

        static readonly Pen penRound = null,
                            penPercentage = new Pen(Brushes.DarkBlue, 2); //new Pen(Brushes.Black, 0.5);

        static Typeface typeface = new Typeface("Tahoma");

        static SolidColorBrush dOk = new SolidColorBrush(Color.FromArgb(255, 144, 238, 144)),
                                    dWarn = new SolidColorBrush(Color.FromArgb(255, 216, 224, 130)),
                                    dError = new SolidColorBrush(Color.FromArgb(255, 178, 100, 100)),
                                    dStop = new SolidColorBrush(Color.FromArgb(255, 0, 50, 90)),
                                    dHoliday = new SolidColorBrush(Color.FromArgb(255, 139, 123, 139)),
                                    dHospital = new SolidColorBrush(Color.FromArgb(255, 205, 104, 137)),
                                    dPlan = new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)),
                                    dAvait = new SolidColorBrush(Color.FromArgb(255, 240, 128, 128));

        static readonly OuterGlowBitmapEffect selectDataEffect = new OuterGlowBitmapEffect { GlowColor = Colors.Red };
        #endregion

        #region Свойства
        public MFPlannerAction Action { get { return Tag as MFPlannerAction; } }
        #endregion

        static MFPlannerProjectControl()
        {
            dOk.Freeze();
            dWarn.Freeze();
            dError.Freeze();
            dStop.Freeze();
            dAvait.Freeze();

            //penRound.Freeze();
            penPercentage.Freeze();

            selectDataEffect.Freeze();
        }
        public MFPlannerProjectControl()
        {
            ClipToBounds = true;
            ToolTip = "";

            ContextMenu = new ContextMenu();
            ContextMenu.Opened += ContextMenu_Opened;

            Opacity = .8;
        }

        public void InitContextMenu()
        {
            if (Action == null || isInitialized)
                return;

            MenuItem mnu = null;

            switch (Action.Type)
            {
                #region Project
                case MFWorkerActionType.Project:
                    {
                        mnu = new MenuItem { Header = "Изменить 'Стадию готовности'" };
                        mnu.SubmenuOpened += mnuPerentage_SubmenuOpened;
                        ContextMenu.Items.Add(mnu);

                        var mnuSub = new MenuItem { Header = "Не установлено", Tag = PlannerMode.ChangeProgressNone };
                        mnuSub.Click += mnu_Click;
                        mnu.Items.Add(mnuSub);

                        mnuSub = new MenuItem { Header = Project.inStart, Tag = PlannerMode.ChangeProgressStart };
                        mnuSub.Click += mnu_Click;
                        mnu.Items.Add(mnuSub);

                        mnuSub = new MenuItem { Header = Project.inHalf, Tag = PlannerMode.ChangeProgressHalf };
                        mnuSub.Click += mnu_Click;
                        mnu.Items.Add(mnuSub);

                        mnuSub = new MenuItem { Header = Project.inFinish, Tag = PlannerMode.ChangeProgressFinish };
                        mnuSub.Click += mnu_Click;
                        mnu.Items.Add(mnuSub);

                        break;
                    }
                #endregion
                #region PlanProject
                case MFWorkerActionType.ReseveProject:
                    {
                        mnu = new MenuItem { Header = "Подтвердить проект", Tag = PlannerMode.PlanProject_Confirm };
                        mnu.Click += mnu_Click;
                        ContextMenu.Items.Add(mnu);

                        break;
                    }
                #endregion
            }

            mnu = new MenuItem { Header = "Изменить описание", Tag = PlannerMode.CommentEdit };
            mnu.Click += mnu_Click;
            ContextMenu.Items.Add(mnu);

            mnu = new MenuItem { Header = "Изменить продолжительность", Tag = PlannerMode.ChangeDays };
            mnu.Click += mnu_Click;
            ContextMenu.Items.Add(mnu);

            ContextMenu.Items.Add(new Separator());

            mnu = new MenuItem { Header = "Прервать", Tag = PlannerMode.Break };
            mnu.Click += mnu_Click;
            ContextMenu.Items.Add(mnu);

            mnu = new MenuItem { Header = "Удалить", Tag = PlannerMode.Remove };
            mnu.Click += mnu_Click;
            ContextMenu.Items.Add(mnu);

            isInitialized = true;
        }

        #region Отрисовка
        void RedrawCommon(DrawingContext dc, string text, Brush brush)
        {
            // Фон
            dc.DrawRectangle(brush, penRound, new Rect(0, 0, Width, Height));

            // Название
            var txt = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 13, Brushes.Black);
            dc.DrawText(txt, new Point((Width - txt.Width) / 2, (Height - txt.Height) / 2));
        }
        void RedrawProject(DrawingContext dc)
        {
            var projects = ServiceManager.LoadProjects();
            var p = GetProject(Action);
            if (p == null)
            {
                RedrawERROR(dc);
                return;
            }

            var brush = Brushes.Green;
            switch (p.State)
            {
                #region OK
                case ProjectState.Ok:
                    {
                        brush = dOk;
                        break;
                    }
                #endregion
                #region Warning
                case ProjectState.Warning:
                    {
                        brush = dWarn;
                        break;
                    }
                #endregion
                #region Error
                case ProjectState.Error:
                    {
                        brush = dError;
                        break;
                    }
                #endregion
                #region Stop
                case ProjectState.Stop:
                    {
                        brush = dStop;
                        break;
                    }
                #endregion
                default:
                    brush = Brushes.Transparent;
                    break;
            }

            // Фон
            dc.DrawRectangle(brush, penRound, new Rect(0, 0, Width, Height));

            // Название
            var typeface = new Typeface("Tahoma");
            var txt = new FormattedText(string.Format("[{0}] {1}\n{2}", p.ID, p.CustomerName, p.MF_Complete_Percentage), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);
            if (txt.Width > Width)
                txt = new FormattedText(p.ID.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);

            dc.DrawText(txt, new Point((Width - txt.Width) / 2, (Height - txt.Height) / 2));

            //var progress = 80;
            //if (int.TryParse(p.MF_Complete_Percentage, out progress) && progress > 0)
            //{
            //    var w = Width * progress / 100;
            //    dc.DrawLine(penPercentage, new Point(0, Height - 4), new Point(Math.Min(w, Width), Height - 4));
            //}
        }

        void RedrawERROR(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Red, penRound, new Rect(0, 0, Width, Height));

            var txt = new FormattedText("ОШИБКА!!! Внимание!", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 15, Brushes.Black);
            dc.DrawText(txt, new Point((Width - txt.Width) / 2, (Height - txt.Height) / 2));
        }
        #endregion

        #region Обработчики событий
        // выполнение контекстного меню
        void mnu_Click(object sender, RoutedEventArgs e)
        {
            var mode = (PlannerMode)((FrameworkElement)sender).Tag;

            var parent = Parent as MFPlannerControl;
            if (parent != null && ContextMenu.Tag is Point)
                parent.MakeCommand(mode, (Point)ContextMenu.Tag, this);
        }

        // Окно подсказки открылось
        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu.Tag = null;

            // если нет элементов то контекстное меню не требуется
            if (ContextMenu.Items.Count == 0)
            {
                ContextMenu.IsOpen = false;
                return;
            }

            var parent = Parent as MFPlannerControl;
            if (parent != null)
                ContextMenu.Tag = Mouse.GetPosition(parent);
        }

        // меню процентов
        void mnuPerentage_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var mnuSub = sender as MenuItem;
            if (mnuSub == null)
                return;

            switch (Action.Type)
            {
                #region Project
                case MFWorkerActionType.Project:
                    {
                        var p = GetProject(Action);
                        if (p == null)
                            return;

                        //Устанавливаем меню процентов
                        foreach (MenuItem mnu in mnuSub.Items)
                        {
                            var name = "Не установлено";
                            if (!string.IsNullOrEmpty(p.MF_Complete_Percentage))
                                name = p.MF_Complete_Percentage;

                            mnu.IsChecked = mnu.Header.ToString() == name;
                        }

                        break;
                    }
                #endregion
            }
        }
        #endregion

        #region Перегрузка стандартных обработчиков событий
        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            base.OnToolTipOpening(e);

            var tip = "";

            if (Action != null)
            {
                switch (Action.Type)
                {
                    #region Holiday
                    case MFWorkerActionType.Holiday:
                        {
                            tip = string.Format("Отпуск на {0} дней", Action.Days);
                            break;
                        }
                    #endregion
                    #region Hospital
                    case MFWorkerActionType.Hospital:
                        {
                            tip = string.Format("Больничный на {0} дней", Action.Days);
                            break;
                        }
                    #endregion
                    #region PlanProject
                    case MFWorkerActionType.ReseveProject:
                        {
                            tip = "Планируемый проект";
                            break;
                        }
                    #endregion
                    #region Project
                    case MFWorkerActionType.Project:
                        {
                            var p = GetProject(Action);
                            if (p == null)
                                break;

                            var cProjectAbout = new ProjectAboutControl { DataContext = p, Width = 500 };
                            ToolTip = cProjectAbout;
                            return;
                        }
                    #endregion
                }
            }

            ToolTip = tip;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Action == null)
            {
                RedrawERROR(dc);
                return;
            }

            switch (Action.Type)
            {
                #region Holiday
                case MFWorkerActionType.Holiday:
                    {
                        RedrawCommon(dc, "Отпуск", dHoliday);
                        break;
                    }
                #endregion
                #region Hospital
                case MFWorkerActionType.Hospital:
                    {
                        RedrawCommon(dc, "Больничный", dHospital);
                        break;
                    }
                #endregion
                #region ReseveProject
                case MFWorkerActionType.ReseveProject:
                    {
                        var name = "Резервирование проекта";
                        if (!string.IsNullOrEmpty(Action.Comment))
                            name = Action.Comment;

                        RedrawCommon(dc, name, dPlan);
                        break;
                    }
                #endregion
                #region Project
                case MFWorkerActionType.Project:
                    {
                        RedrawProject(dc);
                        break;
                    }
                #endregion
                #region Avait
                case MFWorkerActionType.Avait:
                    {
                        RedrawCommon(dc, "П", dAvait);
                        break;
                    }
                #endregion
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.Cursor = Cursors.Hand;
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                var cPlanned = Parent as MFPlannerControl;
                if (cPlanned != null)
                    cPlanned.ContextMenu.IsOpen = true;

                e.Handled = true;
            }
        }
        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);

        //    ReleaseMouseCapture();
        //    this.Cursor = Cursors.Arrow;
        //}
        #endregion

        #region IDisposable
        public void Dispose()
        {

        }
        #endregion

        #region Методы
        static Project GetProject(MFPlannerAction a)
        {
            if (a == null || a.Type != MFWorkerActionType.Project)
                return null;

            return ServiceManager.LoadProject(a.TargetId);
        }
        #endregion
    }
}
