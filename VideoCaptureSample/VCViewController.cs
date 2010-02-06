using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.VideoCaptureSample
{
    public class VCViewController : UIImagePickerController
    {
        #region Variables
        DeviceHardware.HardwareVersion _hardwareVersion;
        OverLayView _overlayView;
        #endregion

        #region Constructor
        public VCViewController()
            : base()
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
            if (null != _overlayView)
                _overlayView.StopWorker();
            DismissModalViewControllerAnimated(true);
        }
        #endregion

        #region Override methods
        public override void LoadView()
        {
            base.LoadView();

            // Loading edge detection view layer 
            if (_hardwareVersion != DeviceHardware.HardwareVersion.Simulator                
                && IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                _overlayView = new OverLayView();
                CameraOverlayView = _overlayView;
            }
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
            
            if(null != _overlayView)
                _overlayView.StartWorker();
        }
        public override void ViewWillDisappear(bool animated)
        {
            if (null != _overlayView)
                _overlayView.StopWorker();

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
