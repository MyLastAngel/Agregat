using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PostTest
{
    public class PostControl : Canvas
    {
        #region Поля
        static readonly long[] timePeriod = new[]
                                                {
                                                    6 * 3600000L, 12 * 3600000L, 24 * 3600000L,
                                                    2 * 24 * 3600000L, 3 * 24 * 3600000L, 4 * 24 * 3600000L,  10 * 24 * 3600000L, 15 * 24 * 3600000L, 30 * 24 * 360000L, 
                                                    2 * 30 * 24 * 3600000L,
                                                };

        // выысота текста
        Size sizeText = new Size(14, 14);
        const double textSpan = 2, fontSize = 14;

        // отрисовка
        Typeface face = new Typeface("Tahoma");

        static Pen penStroke = new Pen(Brushes.Gray, 1),
                   penLine = new Pen(Brushes.Gray, 1);

        #endregion

        #region Свойства
        public Brush Foreground { get { return Brushes.Black; } }

        public DateTimePeriod Period { get; set; }
        #endregion

        static PostControl()
        {
            penStroke.DashStyle = new DashStyle(new double[] { 1, 4 }, 0);
            penStroke.Freeze();

            penLine.Freeze();
        }
        public PostControl()
        {
            Period = new DateTimePeriod(DateTime.Now.Date.AddDays(-14), DateTime.Now.Date);

            var txt = new FormattedText("00", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, fontSize, Foreground);
            sizeText = new Size(txt.Width, txt.Height);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var CursorTime = new DateTime((Period.Start.Ticks + Period.End.Ticks) / 2);

            var time = Period.End - Period.Start;
            if (time.Days < 2)
                return;

            if (e.Delta < 0)
            {
                Period = new DateTimePeriod(Period.Start.AddHours(-1), Period.End.AddHours(1));
            }
            else
            {
                Period = new DateTimePeriod(Period.Start.AddHours(1), Period.End.AddHours(-1));
            }

            //var d1 = (double)(CursorTime.Ticks - Period.Start.Ticks) / (Period.End.Ticks - Period.Start.Ticks);
            //var d2 = (double)(Period.End.Ticks - CursorTime.Ticks) / (Period.End.Ticks - Period.Start.Ticks);
            //var d = (Period.End.Ticks - Period.Start.Ticks) / 10;
            //DateTime start, end;
            //if (e.Delta < 0)
            //{
            //    start = Period.Start.AddTicks((long)(-d * d1));
            //    end = Period.End.AddTicks((long)(d * d2));
            //}
            //else
            //{
            //    start = Period.Start.AddTicks((long)(d * d1));
            //    end = Period.End.AddTicks((long)(-d * d2));
            //}

            //if ((end - start).TotalMilliseconds > 1)
            {

                InvalidateVisual();

                //Console.WriteLine(string.Format("{0} - {1}", Period.Start, Period.End));
            }
        }

        Brush brushForeground = Brushes.Black,
              brushForegroundHolidays = Brushes.Red;

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            double top = sizeText.Height * 2 + textSpan;
            var bounds = new Rect(0, top, ActualWidth, ActualHeight - top - sizeText.Height - 2);

            dc.DrawRectangle(Brushes.White, penStroke, bounds);


            var ticks = (Period.End.Ticks - Period.Start.Ticks) / 10000;
            var displayTicksCount = ActualWidth / (sizeText.Width + 4);

            foreach (var period in timePeriod)
            {
                var ticksCount = (double)ticks / period;
                if (ticksCount > displayTicksCount)
                    continue;

                var tick = ((Period.Start.Ticks / 10000) / period) * period;
                DateTime day = DateTime.MinValue;
                double prevDayX = 0,
                       delta = double.NaN;

                for (int i = 0; i <= ticksCount + 1; i++)
                {
                    var time = new DateTime(tick * 10000);
                    double x = CalcX(time, Period.Start, Period.End, 0, ActualWidth);

                    //if (HApp.ApplicationConfig.IsShowGridMinorLines)
                    //{
                    //delta = (x - prevX) / 4;
                    //for (var ms = x; ms > prevX && ms > 0; ms -= delta)
                    //{
                    //    if (ms > ActualWidth)
                    //        continue;

                    //    dc.DrawLine(strokePen, new Point(Math.Ceiling(ms) + strokePen.Thickness / 2, 0), new Point(Math.Ceiling(ms) + strokePen.Thickness / 2, Math.Ceiling(ActualHeight) + strokePen.Thickness / 2));
                    //}
                    //}

                    if (x >= 0 && x <= ActualWidth)
                    {
                        //if (time.Date.Day != day.Day)
                        //{
                        //    structure[time.Date] = Math.Ceiling(x);
                        //    day = time.Date;
                        //}

                        // месяц
                        if (time.Month != day.Month)
                        {
                            var txt = new FormattedText(time.ToString("MMMM"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, fontSize, brushForeground);
                            dc.DrawText(txt, new Point(x, 2));
                        }

                        // день
                        if (time.Date.Day != day.Day)
                        {
                            var b = brushForeground;
                            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
                                b = brushForegroundHolidays;

                            var txt = new FormattedText(time.ToString("dd"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, fontSize, b);
                            var width = x - prevDayX;
                            if (width >= txt.Width)
                                dc.DrawText(txt, new Point(x, txt.Height + 2));

                            day = time.Date;
                            dc.DrawLine(penLine, new Point(Math.Ceiling(x), bounds.Top), new Point(Math.Ceiling(x), bounds.Bottom));

                            prevDayX = x;
                        }
                    }

                    tick += period;

                }
                break;
            }
        }


        void RenderTimeScale(DrawingContext dc)
        {

        }

        #region Вспомогательные классы
        enum MouseMode
        {
            None,
            Move
        }

        public class DateTimePeriod
        {
            #region Свойства.
            public DateTime Start { get; set; }
            public DateTime End { get; set; }

            public bool IsValid { get { return End > Start; } }
            public TimeSpan Value { get { return End - Start; } }
            public long Ticks { get { return End.Ticks - Start.Ticks; } }
            #endregion

            public DateTimePeriod(DateTime start, DateTime end)
            {
                Start = start;
                End = end;
            }

            public bool Contains(DateTime time)
            {
                return time >= Start && time <= End;
            }
            public bool IntersectsWith(DateTimePeriod period)
            {
                Rect r1 = new Rect(Start.Ticks, 0, End.Ticks, 10),
                     r2 = new Rect(period.Start.Ticks, 0, period.End.Ticks, 10);
                return r1.IntersectsWith(r2);
            }
            public static DateTimePeriod Union(DateTimePeriod p1, DateTimePeriod p2)
            {
                DateTime t1 = p1.Start < p2.Start ? p1.Start : p2.Start,
                         t2 = p1.End > p2.End ? p1.End : p2.End;
                return new DateTimePeriod(t1, t2);
            }
        }
        #endregion

        public static double CalcX(DateTime time, DateTime start, DateTime end, double left, double right)
        {
            var x = (left + ((time.Ticks - start.Ticks) * (right - left)) / (end.Ticks - start.Ticks));

            var dx = Math.Abs(right - left);

            if (x < left - dx)
                return Math.Round(left - dx, 2);
            else if (x > right + dx)
                return Math.Round(right + dx, 2);
            else
                return Math.Round(x, 2);

        }
    }
}
