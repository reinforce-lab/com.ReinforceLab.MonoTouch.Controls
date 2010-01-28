using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar.Standard
{	
	public class MonthView : AbsMonthView<DayView>
	{			
		#region Constructors
		public MonthView (RectangleF rect, ICalendarController ctr, DateTime month, DayOfWeek firstDayOfWeek) : base(rect, ctr, month, firstDayOfWeek)
		{
		}
		#endregion	
	
        #region protected methods
        protected override DayView createDayView(RectangleF rect, DateTime date)
        {
            var dv = _ctr.DayViewCache.GetView("_cell") as DayView;
            dv.Day = date;
            dv.Frame = rect;
            dv.IsActive = (date.Year == _month.Year) && (date.Month == _month.Month);
            dv.IsToday = (date.Year == DateTime.Today.Year) && (date.Month == DateTime.Today.Month) && (date.Day == DateTime.Today.Day);
            dv.IsSelected = false;

            return dv;            
        }
        #endregion
    }
}
