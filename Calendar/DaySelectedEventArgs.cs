using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public enum TouchMode {Began, Ended, Moved, Canceled};
    public class DaySelectedEventArgs : EventArgs
    {
        public TouchMode Mode { get; private set; }
        public CalendarMonthView MonthView { get; private set; }
        public ICalendarDayView DayView { get; private set; }

        public DaySelectedEventArgs(CalendarMonthView mv, ICalendarDayView dv, TouchMode tmode)
        {
            MonthView = mv;
            DayView   = dv;

            Mode = tmode;
        }
    }
}
