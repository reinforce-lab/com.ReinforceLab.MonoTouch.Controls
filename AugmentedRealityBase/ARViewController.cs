using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace com.ReinforceLab.iPhone.Controls.AugmentedRealityBase
{
    public class ARViewController : UIImagePickerController
    {
        #region Variables
        //bool _isDebugRelease;
        DeviceHardware.HardwareVersion _hardwareVersion;
        EdgeDetectionView _overlayView;
        #endregion

        #region Constructor
        public ARViewController(IntPtr handle) : base(handle)  
        {
            initialize();
        }
        public ARViewController()
            : base()
        {
            initialize();
        }
        void initialize()
        {   
            // getting device info to determine whether camera capturing can work.
            /*
#if DEBUG
            _isDebugRelease = true;
#else
            _isDebugRelease = false;
#endif */
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
            if (   _hardwareVersion != DeviceHardware.HardwareVersion.Simulator
//                && !_isDebugRelease
                && IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                _overlayView = new EdgeDetectionView();
                CameraOverlayView = _overlayView;
            }
            // Setting tool bar
            var toolBar = new UIToolbar(new RectangleF(0, 480 - 44, 320, 44));
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
            }
            if (!IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                var aleart = new UIAlertView("Error", "No camera is found.", null, null, "OK");
                aleart.Clicked += new EventHandler<UIButtonEventArgs>(dismissButton_Clicked);
                aleart.Show();
            }
            /*
            if (_isDebugRelease)
            {
                var aleart = new UIAlertView("Fatal error", "Does not work with debug build configration.", null, null, "OK");
                aleart.Clicked += new EventHandler<UIButtonEventArgs>(dismissButton_Clicked);
                aleart.Show();
            }*/

            // start image processing
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
