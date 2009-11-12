using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public interface ICalendarController
    {
        #region Internal methods
        IDayViewSource Source { get; set; }
        
        AbsDayView DequeueReusableView(String cell_id);
        void       EnqueueReusableView(AbsDayView view);
        
        /// <summary>
        /// view calls this method when day is selected.
        /// </summary>        
        void DaySelected(AbsDayView dayView);
        void MonthChanged(DateTime currentMonth, DateTime previousMonth);
        #endregion

        #region External methods
        /// <summary>
        /// external control method to focus to the date.
        /// </summary>
        void FocusToDate(DateTime date, bool activateDay);
        #endregion
    }
}
