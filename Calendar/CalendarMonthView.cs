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
        public event EventHandler DaySelected;

        DateTime  _month;
		DayOfWeek _firstDayofWeek;
        CalendarDayView[] _dayViews;
		#endregion
				
		#region Properties
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
            BackgroundColor = UIColor.Clear;

            _month          = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //SelectedDate    = DateTime.MinValue;
            _firstDayofWeek = DayOfWeek.Sunday;
            setFrameHeight();
		}
		#endregion	
		
		#region Private methods		
        float getFrameHeight()
        {            
            int days = (7 + (int)_month.DayOfWeek - (int)_firstDayofWeek) % 7;            
            float height=  CalendarView.DAYVIEW_HEIGHT * (float)Math.Ceiling((double)(DateTime.DaysInMonth(_month.Year, _month.Month) + days) / 7);
            Debug.WriteLine("\tCalendarMonthView: getFrameHeight() height:{0}, days:{1}, _firstDayOfWeek:{2}, _month.DayOfWeek:{3}, _month:{4}.", height, days, (int)_firstDayofWeek, (int)_month.DayOfWeek, _month);            
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
                var dayView = new CalendarDayView(new RectangleF((i % 7) * CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT * (int)(i / 7), CalendarView.DAYVIEW_WIDTH, CalendarView.DAYVIEW_HEIGHT)) 
                {
                    Day = days[i]
                };
                dayView.IsActive = (days[i].Year == _month.Year) && (days[i].Month == _month.Month);
                dayView.IsToday  = (days[i].Year == DateTime.Today.Year) && (days[i].Month == DateTime.Today.Month) && (days[i].Day == DateTime.Today.Day);
                dayView.Clicked += new EventHandler(_dayViewClicked);
                Add(dayView);                
                dayViews.Add(dayView);
            }
            _dayViews = dayViews.ToArray();            
        }

		void _dayViewClicked(Object sender, EventArgs arg)
		{
			if(null != DaySelected)
				DaySelected.Invoke(sender, arg);
		}
		#endregion
		
		#region override method
        public override void Draw(RectangleF rect)
        {
            base.Draw(rect); 

            if (null == _dayViews)
                buildDayViews();
        }
		#endregion

	}
}
