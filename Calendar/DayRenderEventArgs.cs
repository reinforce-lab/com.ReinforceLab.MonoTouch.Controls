using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{    
    public delegate void DayRenderEventHandler(Object sender, DayRenderEventArgs e);

    public sealed class DayRenderEventArgs : EventArgs
    {
        #region Property
        public CalendarDayView Day { get; private set; }
        #endregion

        #region Constructor
        public DayRenderEventArgs(CalendarDayView dayView) : base()
        {
            Day = dayView;
        }
        #endregion
    }
}
