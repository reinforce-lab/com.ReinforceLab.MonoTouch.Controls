using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
	public class Application
	{
        
		[STAThread]
		public static void Main (string[] args)
		{
            UIApplication.Main(args, null, "AppDelegate");
		}
	}

	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
        CalendarController _calCtr;
        UIWindow     _window;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
            _calCtr = new CalendarController();

            _window.Add(_calCtr.View);            
			_window.MakeKeyAndVisible ();
			return true;
		}

        // This method is required in iPhoneOS 3.0
        public override void OnActivated(UIApplication application)
        {
        }

	}
}
