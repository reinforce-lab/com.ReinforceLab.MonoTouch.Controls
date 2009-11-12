using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public interface IDayViewSource
    {
        AbsDayView GetDayView(ICalendarController ctr, RectangleF rect, DateTime date); 
    }
}
