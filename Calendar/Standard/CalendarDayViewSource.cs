using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{
    public class CalendarDayViewSource : IDayViewSource
    {
        #region IDayViewSource メンバ
        public AbsDayView GetDayView(ICalendarController ctr, System.Drawing.RectangleF rect, DateTime date)
        {
            const string cell_id = "standard_calendarDayView";

            var view = ctr.DequeueReusableView(cell_id) as CalendarDayView;
            if (null != view)
            {
                view.Frame = rect;
                view.IsActive   = true;
                view.IsToday    = false;
                view.IsMarked   = false;
                view.IsSelected = false;
                view.Day        = date;
                
                return view;
            }
            else
            {
                return new CalendarDayView(rect) { Day = date, CellID = cell_id };
            }
        }
        #endregion
    }
}
