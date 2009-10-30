using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
	public class Application
	{
		static void Main (string[] args)
		{
            UIApplication.Main(args, null, "AppDelegate");
		}
	}

	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
        CalendarDayView _currentDay;
        CalendarView _calendarView;        
        UIWindow     _window;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
            _calendarView = new CalendarView(new RectangleF(0, 20, 320, 300));
            //view.FirstDayOfWeek = DayOfWeek.Wednesday;
            _calendarView.VisibleMonthChanged += new MonthChangedEventHandler(view_VisibleMonthChanged);
            _calendarView.DaySelected += new EventHandler<DaySelectedEventArgs>(view_DaySelected);            
            _window.Add(_calendarView);            
			_window.MakeKeyAndVisible ();
			return true;
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

        // This method is required in iPhoneOS 3.0
        public override void OnActivated(UIApplication application)
        {
        }

	}
}
