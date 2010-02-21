using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{
    class BTDeviceInfoCellView : UITableViewCell
    {
        #region Variable
        BTDeviceInfo device_info;
        readonly System.ComponentModel.PropertyChangedEventHandler prop_changed_handler;
        UILabel bt_device_name_label;
        UILabel bt_address_label;
        UILabel bt_class_of_device_label;
        UILabel bt_clock_offset_label;
        UILabel bt_page_scan_repetition_mode_label;
        UILabel bt_rssi_label;
        UILabel bt_state_label;
        #endregion

        #region Constructor
        public BTDeviceInfoCellView() : base(UITableViewCellStyle.Default, typeof(BTDeviceInfoCellView).Name )
        {
            this.Frame = new RectangleF(Frame.Location, new SizeF(210, 80));            

            prop_changed_handler = new System.ComponentModel.PropertyChangedEventHandler(device_info_PropertyChanged);

            RectangleF frame = new RectangleF(10, 5, 140, 24);
            bt_device_name_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_device_name_label);

            frame.X += frame.Width + 10;
            bt_address_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_address_label);

            frame = new RectangleF(10, 5 + 24, 140, 15);
            bt_state_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_state_label);

            frame = new RectangleF(10, 5 + 24 + 15, 140, 15);
            bt_class_of_device_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_class_of_device_label);

            frame.X += frame.Width + 10;
            bt_clock_offset_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_clock_offset_label);

            frame = new RectangleF(10, 5 + 24 + 15 + 15, 140, 15);
            bt_page_scan_repetition_mode_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_page_scan_repetition_mode_label);

            frame.X += frame.Width + 10;
            bt_rssi_label = new UILabel(frame) { TextAlignment = UITextAlignment.Left, AdjustsFontSizeToFitWidth = true };
            ContentView.Add(bt_rssi_label);

            initView();
        }
        #endregion

        #region Public methods
        protected override void Dispose(bool disposing)
        {
            if (null != device_info)
            {
                device_info.PropertyChanged -= prop_changed_handler;
                device_info = null;
            }
            
            base.Dispose(disposing);
        }
        public void SetDeviceInfo(BTDeviceInfo dinfo)
        {
            if (dinfo == device_info) return;

            if (null != device_info)
            {
                device_info.PropertyChanged -= prop_changed_handler;
                device_info = null;
            }

            device_info = dinfo;
            if (null != device_info)
            {
                device_info.PropertyChanged += prop_changed_handler;
                initView();
            }
        }
        #endregion

        #region Private methods
        void initView()
        {
            bt_device_name_label.Text = "device name";
            bt_device_name_label.TextColor = UIColor.Gray;
            bt_address_label.Text = "address";
            bt_address_label.TextColor = UIColor.Gray;
            bt_class_of_device_label.Text = "class of device";
            bt_class_of_device_label.TextColor = UIColor.Gray;
            bt_clock_offset_label.Text = "clock offset";
            bt_clock_offset_label.TextColor = UIColor.Gray;
            bt_page_scan_repetition_mode_label.Text = "page scan repetition mode";
            bt_page_scan_repetition_mode_label.TextColor = UIColor.Gray;
            bt_rssi_label.Text = "rssi";
            bt_rssi_label.TextColor = UIColor.Gray;
            bt_state_label.Text = "initial state";
            bt_state_label.TextColor = UIColor.Gray;

            if (null != device_info)
            {
                if (!String.IsNullOrEmpty(device_info.DeviceName))
                {
                    bt_device_name_label.Text = String.Format("Device: {0}", device_info.DeviceName);
                    bt_device_name_label.TextColor = UIColor.Black;
                }
                bt_address_label.Text = String.Format("Address: {0}", device_info.GetBtAddress());
                bt_address_label.TextColor = UIColor.Black;
                if (0 != device_info.ClassOfDevice)
                {
                    bt_class_of_device_label.Text = String.Format("Class: 0x{0:X}",device_info.ClassOfDevice) ;
                    bt_class_of_device_label.TextColor = UIColor.Black;
                }
                if (0 != device_info.ClockOffset)
                {
                    bt_clock_offset_label.Text = String.Format("Clock offset: 0x{0:X}", device_info.ClockOffset);
                    bt_clock_offset_label.TextColor = UIColor.Black;
                }
                if (0 != device_info.PageScanRepetitionMode)
                {
                    bt_page_scan_repetition_mode_label.Text = String.Format("Page scan: 0x{0:X}", device_info.PageScanRepetitionMode);
                    bt_page_scan_repetition_mode_label.TextColor = UIColor.Black;
                }
                if (0 != device_info.Rssi)
                {
                    bt_rssi_label.Text = String.Format("RSSI: 0x{0:X}", device_info.Rssi);
                    bt_rssi_label.TextColor = UIColor.Black;
                }
                bt_state_label.Text = String.Format("State: {0}", device_info.State.ToString("G"));
                bt_state_label.TextColor = UIColor.Black;
            }
        }
        void device_info_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            initView();
            /*
            switch (e.PropertyName)
            {
                case BTDeviceInfo.PN_Address:
                    break;
                case BTDeviceInfo.PN_ClassOfDevice:
                    break;
                case BTDeviceInfo.PN_ClockOffset:
                    break;
                case BTDeviceInfo.PN_PageScanRepetitionMode:
                    break;
                case BTDeviceInfo.PN_Rssi:
                    break;
                case BTDeviceInfo.PN_State:
                    break;
                default:
                    break;
            }*/
        }
        #endregion
    }
}
