
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.iPhone.Controls.ControlsDemo
{
	class ControlItem {public String Title; public UIViewController Controller;}
	
	class DataSource : UITableViewSource	
	{			
		#region Variables
		readonly ControlItem [] _controlItems;
		ControlItem _selectedItem;
		
		public event EventHandler ItemSelected;
		#endregion
		
		#region Properties
		public ControlItem SelectedItem {get{return _selectedItem;}}
		#endregion
		
		#region Constructor
		public DataSource(ControlItem[] controlItems) : base()
		{
			_selectedItem = null;
			_controlItems = controlItems;
		}		
		#endregion
		
		public override int RowsInSection (UITableView tableview, int section)		
		{				
			return _controlItems.Length;
		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string _cellID = "_viewCell";	
			var cell = tableView.DequeueReusableCell(_cellID);
			if(null == cell)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Default, _cellID);	
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}			
			
			cell.TextLabel.Text = _controlItems[indexPath.Row].Title;
			return cell;
		}	
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// invoke item selected event
			_selectedItem = _controlItems[indexPath.Row];
			if(null != ItemSelected)
				ItemSelected.Invoke(this, null);
		}
	}

}
