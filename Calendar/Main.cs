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
        UIWindow _window;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
            var view = new CalendarView(new RectangleF(0, 20, 320, 300));
            //view.FirstDayOfWeek = DayOfWeek.Wednesday;
            view.VisibleMonthChanged += new MonthChangedEventHandler(view_VisibleMonthChanged);
            view.DaySelected += new EventHandler(view_DaySelected);            
            _window.Add(view);            
			_window.MakeKeyAndVisible ();
			return true;
		}

        void view_DaySelected(object sender, EventArgs e)
        {
            Debug.WriteLine("DayView is selected. date: {0}.", (sender as CalendarDayView).Day.Day);                        
        }

        void view_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            Debug.WriteLine("Visible month changed. new date:{0} prev:{1}.", e.NewDate.ToString("y"), e.PreviousDate.ToString("y"));            
        }
        
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{            
		}
	}
}
