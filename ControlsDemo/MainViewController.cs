using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using  net.ReinforceLab.MonoTouch.Controls.Calendar.Standard;

namespace net.ReinforceLab.iPhone.Controls.ControlsDemo
{
    class MainViewController : UITableViewController
    {
        #region Variables
        DataSource _list;
        CalendarController _calendarCtr;
        #endregion

        #region Constructor
        public MainViewController() : base()
        {
            initialize();
        }
        void initialize()
        {
        }
        #endregion

        #region Public methods
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "ReinforceLab UICatalog";            
            _calendarCtr = new CalendarController();
            _calendarCtr.DaySelectionChanged += new EventHandler<EventArgs>(daySelectionChanged);

            _list = new DataSource(this, new ControlItem[] { 
                new ControlItem() {Title ="Calendar", Controller = _calendarCtr }
            });

            TableView.Source = _list;

            NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "Back" };
        }
        #endregion

        #region Event hander
        void daySelectionChanged(Object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Calendar day selection changed: current date {0}.", ((CalendarController)sender).CurrentDay.ToShortDateString());
        }
        #endregion

        #region Data source
        class ControlItem {public String Title; public UIViewController Controller;}

        class DataSource : UITableViewSource
        {
            #region Variables
            readonly MainViewController _mvc;
            readonly ControlItem[] _controlItems;
            #endregion

            #region Constructor
            public DataSource(MainViewController mvc, ControlItem[] controlItems)
                : base()
            {
                _mvc = mvc;
                _controlItems = controlItems;
            }
            #endregion

            public override int RowsInSection(UITableView tableview, int section)
            {
                return _controlItems.Length;
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                const string _cellID = "_viewCell";
                var cell = tableView.DequeueReusableCell(_cellID);
                if (null == cell)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, _cellID);
                    cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                }

                cell.TextLabel.Text = _controlItems[indexPath.Row].Title;
                return cell;
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                _mvc.NavigationController.PushViewController(_controlItems[indexPath.Row].Controller, true);
            }
        } 
        #endregion
    }
}
