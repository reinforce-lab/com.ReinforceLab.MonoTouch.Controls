
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.ControlsDemo
{
	public partial class RootViewController : UIViewController
	{
		DataSource _source;
		#region Constructors

		// The IntPtr and NSCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public RootViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public RootViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public RootViewController () : base("RootViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			var controlItems = new ControlItem[]
			{
				new ControlItem(){Title = "Calendar", Controller = new CalendarController() }
			};
			_source = new DataSource(controlItems);
		}
		#endregion

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			this.tableView.Source = _source;			
		}		
	}
}
