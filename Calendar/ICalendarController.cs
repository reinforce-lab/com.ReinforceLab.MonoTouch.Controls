using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.UIKit;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar
{
    public interface ICalendarController
    {
        ViewCache DayViewCache { get; }
        void DaySelected(DateTime day);        
        void CalendarViewChanged(DateTime currentDate, DateTime previousDate);        
    }
}
