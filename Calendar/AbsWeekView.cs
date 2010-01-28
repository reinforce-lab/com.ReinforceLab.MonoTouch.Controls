using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar
{	
	public abstract class AbsWeekView<T> : AbsCalendarView<T>
         where T : UIView, IDayView	
	{
		#region Variables
        protected float _DayViewHeight = 45f;
        protected float _DayViewWidth  = 46f;

        protected readonly DateTime            _firstDayOfWeek;		        
		#endregion
				
		#region Properties
        /// <summary>
        /// get the 1st day of a week. This property can be set until this view drawin.
        /// </summary>
        public DateTime FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
        }
		#endregion
		
		#region Constructors
		public AbsWeekView (RectangleF rect, ICalendarController ctr, DateTime dow) : base(rect, ctr)
		{            
            _firstDayOfWeek = new DateTime(dow.Year, dow.Month, dow.Day);
            initialize();                
		}
        protected virtual void initialize()
        {
            Frame = new RectangleF(Frame.Location, new SizeF(_DayViewWidth *7, _DayViewHeight)); 
            buildDayViews();
        }
		#endregion	
		
		#region Private methods		       
        void buildDayViews()
        {
            if (null != _dayViews) return;

            List<DateTime> days = new List<DateTime>();
            // setting previous month days
            DateTime dt = new DateTime(_firstDayOfWeek.Ticks);
            for(int i = 0; i < 7; i++)
            {
                days.Add(dt);
                dt = dt.AddDays(1.0);
            }
                        
            var dayViews = new List<T>();
            for(int i =0; i < days.Count; i++)
            {
                var rect = new RectangleF(i * _DayViewWidth, 0, _DayViewWidth, _DayViewHeight);
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
            int index = (int)point.X / (int)_DayViewWidth;
            if (0 <= index && index < _dayViews.Length)
                return _dayViews[index];
            else
                return null;
        }
        #endregion

    }
}
