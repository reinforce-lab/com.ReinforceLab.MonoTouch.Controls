using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace com.ReinforceLab.MonoTouch.Controls.ControlsDemo
{
	public class Application
	{
        [STAThread]
		public static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
        UIWindow               _window;
        UINavigationController _navCtr;
        MainViewController     _mainCtr;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{                        
            _mainCtr = new MainViewController();            
            _navCtr  = new UINavigationController(_mainCtr);

            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            _window.Add(_navCtr.View);			
            _window.MakeKeyAndVisible ();
			
			return true;
		}
		
		public override void OnActivated (UIApplication application)
		{
		}

        /// <summary>
        /// GC is nessessary due to periodical memory alloc/free in camera capturing class.
        /// </summary>        
        public override void ReceiveMemoryWarning(UIApplication application)
        {
            System.GC.Collect();
        }
	}
}
