using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{	
	public class CalendarMonthView : UIView
	{
		#region Variables
        readonly DateTime           _month;
		readonly DayOfWeek          _firstDayofWeek;        
        readonly CalendarController _ctr;

        CalendarDayView[] _dayViews;
		#endregion
				
		#region Properties
        public CalendarDayView[] DayViews { get { return _dayViews; } }

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
        /*
        public CalendarMonthView(IntPtr ptr)
            : base(ptr)
        { }*/
		public CalendarMonthView (RectangleF rect, CalendarController ctr, DateTime month, DayOfWeek firstDayOfWeek) : base(rect)
		{
            _ctr = ctr;
            _month = new DateTime(month.Year, month.Month, 1);            
            _firstDayofWeek = firstDayOfWeek;

            BackgroundColor = UIColor.LightGray;
            setFrame();
            buildDayViews();
		}
		#endregion	
		
		#region Private methods		
        void setFrame()
        {
            int days = (7 + (int)_month.DayOfWeek - (int)_firstDayofWeek) % 7;
            float height = CalendarView.DAYVIEW_HEIGHT * (float)Math.Ceiling((double)(DateTime.DaysInMonth(_month.Year, _month.Month) + days) / 7);
            Frame = new RectangleF(Frame.Location, new SizeF(CalendarView.MONTHVIEW_WIDTH, height));
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
            
            var dayViews = new List<CalendarDayView>();
            for(int i =0; i < days.Count; i++)
            {
                var rect = new RectangleF((i % 7) * CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT * (int)(i / 7), CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT);
                var dayView = _ctr.Source.GetDayView(_ctr, rect, days[i]) as CalendarDayView;
                dayView.IsActive = (days[i].Year == _month.Year) && (days[i].Month == _month.Month);
                dayView.IsToday  = (days[i].Year == DateTime.Today.Year) && (days[i].Month == DateTime.Today.Month) && (days[i].Day == DateTime.Today.Day);
                
                Add(dayView as UIView);                
                dayViews.Add(dayView);
            }
            _dayViews = dayViews.ToArray();            
        }
        void selectDay(UITouch touch)
        {
            PointF point = touch.LocationInView(this);
            //Debug.WriteLine("\tCalendarMonthView: point: {0}. touch.view: {1}, mode: {2}. exclusible touch:{3}.", point, touch.View.GetType(), tmode, this.ExclusiveTouch);
            
            int dx = (int)point.X / (int)CalendarView.DAYVIEW_WIDTH;
            int dy = (int)point.Y / (int)CalendarView.DAYVIEW_HEIGHT;
            int index = dx + 7 * dy;

            if (0<= index && index < _dayViews.Length)
                _ctr.DaySelected(_dayViews[index]);
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
            selectDay((UITouch)touches.AnyObject);
        }
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
 	        base.TouchesCancelled(touches, evt);
            selectDay((UITouch)touches.AnyObject);             
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            selectDay((UITouch)touches.AnyObject); 
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            selectDay((UITouch)touches.AnyObject); 
        }
        
        protected override void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine("MonthView: Dispose()");
            foreach (var item in _dayViews)
            {
                item.Superview.RemoveFromSuperview();
                _ctr.EnqueueReusableView(item);
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
