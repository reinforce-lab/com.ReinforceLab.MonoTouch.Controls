using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    public class CameraViewController : UIImagePickerController
    {
        #region Variables
        DeviceHardware.HardwareVersion _hardwareVersion;
        StreamingController _streamingController;
        UILabel _textLabel;
        #endregion

        #region Constructor
        public CameraViewController() : base()
        {
            initialize();
        }
        void initialize()
        {
            // getting device info to determine whether camera capturing can work.
            _hardwareVersion = DeviceHardware.Version;

            if (IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
                SourceType = UIImagePickerControllerSourceType.Camera;
            else
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary;

            ShowsCameraControls = false;
            AllowsImageEditing  = false;
        }
        #endregion

        #region Private methods
        void dismissViewController()
        {
            DismissModalViewControllerAnimated(true);
        }
        #endregion

        #region Override methods
        public override void LoadView()
        {
            base.LoadView();

            // adding uri text label
            _textLabel = new UILabel(new Rectangle(0, 480 - 54 - 10, 320, 10)) { TextAlignment = UITextAlignment.Center};
            Add(_textLabel);

            // Setting tool bar
            var toolBar = new UIToolbar(new RectangleF(0, 480 - 54, 320, 54));
            toolBar.Items = new UIBarButtonItem[] { 
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem(UIBarButtonSystemItem.Done, new EventHandler(doneButton_Clicked))
            };
            Add(toolBar);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // camera capturing needs supported hardware and should be built with a release-configration.
            if (_hardwareVersion == DeviceHardware.HardwareVersion.Simulator)
            {
                var aleart = new UIAlertView("Error", "Simulator is not supported.", null, null, "OK");
                aleart.Clicked += new EventHandler<UIButtonEventArgs>(dismissButton_Clicked);
                aleart.Show();
                return;
            }
            if (!IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                var aleart = new UIAlertView("Error", "No camera is found.", null, null, "OK");
                aleart.Clicked += new EventHandler<UIButtonEventArgs>(dismissButton_Clicked);
                aleart.Show();
                return;
            }

            // Looking for wi-fi address
            var hostname = System.Net.Dns.GetHostName();
            System.Diagnostics.Debug.WriteLine("Hostname: " + hostname);
            var ipentry = System.Net.Dns.GetHostByName(hostname);
#if DEBUG            
            foreach(var address in ipentry.AddressList)
                System.Diagnostics.Debug.WriteLine("IP address: " + address.ToString());
#endif
            var url = String.Format("http://{0}:8081", ipentry.AddressList[0].ToString());
            // Loading edge detection view layer 
            _streamingController = new StreamingController(new Uri(url), new Rectangle(0, 0, 320, 480 - 54 - 10));
            
            _textLabel.Text = url;
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (null != _streamingController)
            {
                _streamingController.Dispose();
                _streamingController = null;
            }

            base.ViewWillDisappear(animated);
        }
        #endregion

        #region
        void dismissButton_Clicked(object sender, UIButtonEventArgs e)
        {
            dismissViewController();
        }
        void doneButton_Clicked(object sender, EventArgs e)
        {
            dismissViewController();
        }
        #endregion
    }
}
