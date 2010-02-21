    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using MonoTouch.UIKit;
    using MonoTouch.Foundation;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{
    public class BTInquiryViewController : UITableViewController
    {
        #region Variables
        DataSource data_source;
        bool is_bt_available;
        #endregion

        #region Constructor
        public BTInquiryViewController()
        {
            is_bt_available = false;
            data_source = new DataSource(this);
        }
        #endregion

        #region Override methods
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Bluetooth device inquiry";
            data_source = new DataSource(this);
            TableView.Source = data_source;
            NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "Back" };
        }
        public override void ViewDidUnload()
        {
            data_source.Dispose();

            base.ViewDidUnload();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var aleart = new UIAlertView() { Title = "Warning", Message = "Jail break iphone with btstack is required.\nShould start inquiry?" };
            aleart.AddButton("Cancel");
            aleart.AddButton("OK");
            aleart.Dismissed += new EventHandler<UIButtonEventArgs>(aleart_Dismissed);
            aleart.Show();
        }
        public override void ViewDidDisappear(bool animated)
        {
            if(is_bt_available)
                BTPacketHandler.StopInquiry();

            base.ViewDidDisappear(animated);
        }
        #endregion

        #region Data source  
        internal class DataSource : UITableViewSource
        {
            #region Variables
            readonly BTInquiryViewController mvc;
            readonly EventHandler listChangedHandler;
            #endregion

            #region Constructor
            public DataSource(BTInquiryViewController view_controller)
                : base()
            {
                mvc = view_controller;
                listChangedHandler = new EventHandler(listChanged);
                BTPacketHandler.DeviceListChangedEvent += listChangedHandler;
            }
            #endregion

            #region private methods
            void listChanged(Object sender, EventArgs e)
            {
                mvc.TableView.ReloadData();
            }
            #endregion

            #region Public methods            
            protected override void Dispose(bool disposing)
            {
                BTPacketHandler.DeviceListChangedEvent -= listChangedHandler;

                base.Dispose(disposing);                
            }
            public override NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
            {
                return null;                
            }
            
            public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 80f;
            }
            public override int SectionFor(UITableView tableView, string title, int atIndex)
            {
                return 1;                
            }
            public override int RowsInSection(UITableView tableview, int section)
            {
                return BTPacketHandler.DeviceList.Length;
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(typeof(BTDeviceInfoCellView).Name) as BTDeviceInfoCellView;
                if (null == cell)
                    cell = new BTDeviceInfoCellView();
                                
                cell.SetDeviceInfo(BTPacketHandler.DeviceList[indexPath.Row]);

                return cell;
            }
            #endregion
        } 
        #endregion

        #region Private methods
        void aleart_Dismissed(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                // canceled                
            }
            else
            { 
                // OK
                BTPacketHandler.StartInquiry();
                is_bt_available = true;
            }
        }
        #endregion


    }
}
    
