using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar
{	
	public abstract class AbsMonthView<T> : AbsCalendarView<T>
         where T : UIView, IDayView
	{
		#region Variables
        protected float _DayViewHeight = 45f;
        protected float _DayViewWidth  = 46f;

        protected readonly DateTime _month;
        protected readonly DayOfWeek _firstDayofWeek;                
		#endregion
				
		#region Properties
        /// <summary>
        /// get month (1st day of the month). This property can be set until this view drawin.
        /// </summary>
        public DateTime Month 
        {
            get { return _month; }
        }
        /// <summary>
        /// get set first day of a week. This property can be set until this view drawin.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return _firstDayofWeek; }
        }
		#endregion
		
		#region Constructors
		public AbsMonthView (RectangleF rect, ICalendarController ctr, DateTime month, DayOfWeek firstDayOfWeek) : base(rect, ctr)
		{
            _month = new DateTime(month.Year, month.Month, 1);            
            _firstDayofWeek = firstDayOfWeek;

            initialize();
        }
        protected virtual void initialize()
        {
            setFrame();
            buildDayViews();
        }
        #endregion	
		
		#region Private methods		
        void setFrame()
        {
            int days = (7 + (int)_month.DayOfWeek - (int)_firstDayofWeek) % 7;
            float height = _DayViewHeight * (float)Math.Ceiling((double)(DateTime.DaysInMonth(_month.Year, _month.Month) + days) / 7);
            Frame = new RectangleF(Frame.Location, new SizeF(_DayViewWidth * 7, height));                
        }
        List<DateTime> buildDaysInMonthView()
        {
            List<DateTime> days = new List<DateTime>();
            // setting previous month days
            DateTime dt = new DateTime(_month.Year, _month.Month, 1);
            if (dt.DayOfWeek != _firstDayofWeek)
            {
                List<DateTime> prevDays = new List<DateTime>();
                do
                {
                    dt = dt.AddDays(-1.0);
                    prevDays.Add(dt);
                } while (dt.DayOfWeek != _firstDayofWeek);
                prevDays.Reverse();
                days.AddRange(prevDays);
            }
            // setting current month days
            int daysInMonth = DateTime.DaysInMonth(_month.Year, _month.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                days.Add(new DateTime(_month.Year, _month.Month, i));
            }
            // setting next month days
            dt = new DateTime(_month.Year, _month.Month, 1);
            dt = dt.AddMonths(1);
            if (dt.DayOfWeek != _firstDayofWeek)
            {
                while (dt.DayOfWeek != _firstDayofWeek)
                {
                    days.Add(dt);
                    dt = dt.AddDays(1.0);
                }
            }
            
            return days;
        }
        void buildDayViews()
        {
            if (null != _dayViews) return;

            var days = buildDaysInMonthView();
            
            var dayViews = new List<T>();
            for(int i =0; i < days.Count; i++)
            {
                var rect = new RectangleF((i % 7) * _DayViewWidth, _DayViewHeight * (int)(i / 7), _DayViewWidth, _DayViewHeight);
                var dayView = createDayView(rect, days[i]);
                Add(dayView as UIView);                
                dayViews.Add(dayView);
            }
            _dayViews = dayViews.ToArray();            
        }
		#endregion

        #region protected methods
        protected override T hitTestDayView(PointF point)
        {
            int dx = (int)point.X / (int)_DayViewWidth;
            int dy = (int)point.Y / (int)_DayViewHeight;
            int index = dx + 7 * dy;

            if (0 <= index && index < _dayViews.Length)
                return _dayViews[index];
            else
                return null;
        }        
        #endregion       
    }
}
