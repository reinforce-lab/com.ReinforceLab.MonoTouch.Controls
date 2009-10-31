using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{	
	public class CalendarMonthView : UIView
	{
		#region Variables
        public event EventHandler<DaySelectedEventArgs> DaySelected;

        DateTime  _month;
		DayOfWeek _firstDayofWeek;
        CalendarDayView[] _dayViews;
		#endregion
				
		#region Properties
        public CalendarDayView[] DayViews { get { return _dayViews; } }

        /// <summary>
        /// get or set month (1st day of the month). This property can be set until this view drawin.
        /// </summary>
        public DateTime Month 
        {
            get { return _month; }
            set
            {
                if (null == _dayViews&& value != _month )
                {
                    _month = new DateTime(value.Year, value.Month, 1);
                    setFrameHeight();
                }
            }
        }
        /// <summary>
        /// get or set first day of a week. This property can be set until this view drawin.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return _firstDayofWeek; }
            set
            {
                if (null == _dayViews && value != _firstDayofWeek)
                {
                    _firstDayofWeek = value;
                    setFrameHeight();
                }
            }
        }
        //public DateTime SelectedDate { get; private set; }        
		//public CalendarDayView[] Days {get{return _dayViews;}}
		#endregion
		
		#region Constructors
		public CalendarMonthView (RectangleF rect) : base(rect)
		{
			Initialize ();
		}
		void Initialize ()
		{            
            BackgroundColor = UIColor.LightGray;

            _month          = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);            
            _firstDayofWeek = DayOfWeek.Sunday;
            setFrameHeight();
		}
		#endregion	
		
		#region Private methods		
        float getFrameHeight()
        {            
            int days = (7 + (int)_month.DayOfWeek - (int)_firstDayofWeek) % 7;            
            float height=  CalendarView.DAYVIEW_HEIGHT * (float)Math.Ceiling((double)(DateTime.DaysInMonth(_month.Year, _month.Month) + days) / 7);
            //Debug.WriteLine("\tCalendarMonthView: getFrameHeight() height:{0}, days:{1}, _firstDayOfWeek:{2}, _month.DayOfWeek:{3}, _month:{4}.", height, days, (int)_firstDayofWeek, (int)_month.DayOfWeek, _month);            
            return height;                
        }
        void setFrameHeight()
        {
            Frame = new RectangleF(Frame.Location, new SizeF(CalendarView.MONTHVIEW_WIDTH, getFrameHeight()));
        }
        void buildDayViews()
        {
            if (null != _dayViews) return;            

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
            List<CalendarDayView> dayViews = new List<CalendarDayView>();
            for(int i =0; i < days.Count; i++)
            {
                var dayView = CreateDayView(new RectangleF((i % 7) * CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT * (int)(i / 7), CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT));
                
                dayView.Day      = days[i];
                dayView.IsActive = (days[i].Year == _month.Year) && (days[i].Month == _month.Month);
                dayView.IsToday  = (days[i].Year == DateTime.Today.Year) && (days[i].Month == DateTime.Today.Month) && (days[i].Day == DateTime.Today.Day);
                
                Add(dayView);                
                dayViews.Add(dayView);
            }
            _dayViews = dayViews.ToArray();            
        }
        void selectDay(UITouch touch, TouchMode tmode)
        {            
            PointF point = touch.LocationInView(this);
            //Debug.WriteLine("\tCalendarMonthView: point: {0}. touch.view: {1}, mode: {2}. exclusible touch:{3}.", point, touch.View.GetType(), tmode, this.ExclusiveTouch);
            
            int dx = (int)point.X / (int)CalendarView.DAYVIEW_WIDTH;
            int dy = (int)point.Y / (int)CalendarView.DAYVIEW_HEIGHT;
            int index = dx + 7 * dy;

            if (index < _dayViews.Length)
            {
                // invoke day selected event
                if (null != DaySelected)
                    DaySelected.Invoke(this, new DaySelectedEventArgs(this, _dayViews[index], tmode));
            }         
        }
		#endregion

        #region protected methods
        protected virtual CalendarDayView CreateDayView(RectangleF rect)
        {
            return new CalendarDayView(rect);
        }
        #endregion

        #region public methods
        public override void Draw(RectangleF rect)
        {
            base.Draw(rect); 

            if (null == _dayViews)
                buildDayViews();
        }
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);                        
            selectDay((UITouch) touches.AnyObject, TouchMode.Began);            
        }
        public override void  TouchesCancelled(NSSet touches, UIEvent evt)
        {
 	        base.TouchesCancelled(touches, evt);            
            selectDay((UITouch)touches.AnyObject, TouchMode.Canceled);             
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);                        
            selectDay((UITouch) touches.AnyObject, TouchMode.Moved);
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);            
            selectDay((UITouch) touches.AnyObject, TouchMode.Ended);             
        }
        #endregion

	}
}
