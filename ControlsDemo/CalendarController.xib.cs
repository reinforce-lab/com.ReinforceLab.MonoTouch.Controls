
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Diagnostics;

using net.ReinforceLab.iPhone.Controls.Calendar;

namespace net.ReinforceLab.iPhone.Controls.ControlsDemo
{
	public partial class CalendarController : UIViewController
	{
		#region Constructors

		// The IntPtr and NSCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public CalendarController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CalendarController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CalendarController () : base("CalendarController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
        CalendarView _calendarView;        
		CalendarDayView _currentDay;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			          
			_calendarView = new CalendarView(new RectangleF(0, 20, 320, 300));
            //view.FirstDayOfWeek = DayOfWeek.Wednesday;
            _calendarView.VisibleMonthChanged += new MonthChangedEventHandler(view_VisibleMonthChanged);
            _calendarView.DaySelected += new EventHandler<DaySelectedEventArgs>(view_DaySelected); 
			
			this.View.AddSubview(_calendarView);
		}

        void view_DaySelected(object sender, DaySelectedEventArgs e)
        {
            Debug.WriteLine("DayView is selected. date: {0}, mode: {1}.", e.DayView.Day.Date, e.Mode);
            if (e.DayView.Day.Month != e.MonthView.Month.Month)
            {
                if (e.Mode == TouchMode.Ended)
                {
					_currentDay = e.DayView;
                    if (e.DayView.Day.Month > e.MonthView.Month.Month)
                        _calendarView.MoveToNextMonth();
                    else
                        _calendarView.MoveToPrevMonth();
                }
            }
            else
            {
                switch (e.Mode)
                {
                    case TouchMode.Canceled: break;
                    case TouchMode.Began: break;
                    case TouchMode.Ended:
                        if (null != _currentDay)
                            _currentDay.IsSelected = false;
                        e.DayView.IsSelected = true;
                        _currentDay = e.DayView;
                        break;
                    case TouchMode.Moved:
                        if (null != _currentDay && _currentDay.Day != e.DayView.Day)
                            _currentDay.IsSelected = false;
                        e.DayView.IsSelected = true;
                        _currentDay = e.DayView;
                        break;
                }
            }
        }

        void view_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            Debug.WriteLine("Visible month changed. new date:{0} prev:{1}.", e.NewDate.ToString("y"), e.PreviousDate.ToString("y"));
            if (null != _currentDay)
            {
                var cview = sender as CalendarView;
                foreach (var day in cview.MonthView.DayViews)
                {
                    if (day.Day == _currentDay.Day)
                    {
                        day.IsSelected = true;
                        _currentDay = day;
                        break;
                    }
                }
            }
		}
	}
}
