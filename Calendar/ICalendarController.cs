using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.UIKit;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
    public interface ICalendarController
    {
        ViewCache DayViewCache { get; }
        void DaySelected(DateTime day);        
        void MonthChanged(DateTime currentMonth, DateTime previousMonth);
    }
}
