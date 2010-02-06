using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace com.ReinforceLab.MonoTouch.Controls.VideoCaptureSample
{
    class OverLayView : UIView
    {
        #region Variables
        int _fpsCount;
        NSDate _start, _end;
        NSTimer _timer;
        UILabel _fpsText;
        UILabel _fpsLabel;
        UIView _frameView;
        UIImageView _imageView;        
        #endregion

        #region Constructor
        public OverLayView() : base()
        {
            initialize();
        }
        void initialize()
        {            
            Frame = new RectangleF(0, 0, 320, 480 - 54);
            // important - it needs to be transparent so the camera preview shows through!
            Opaque = false;
            BackgroundColor = UIColor.Clear;

            _fpsCount = 0;
            // loading subviews
            _frameView = new UIView(new RectangleF(10, 276, 100, 140)) { Opaque = true, BackgroundColor = UIColor.White };
            Add(_frameView);

            _imageView = new UIImageView(new Rectangle(5, 5, 90, 130)) { Opaque = true };            
            _frameView.Add(_imageView);

            _fpsText = new UILabel(new RectangleF(110, 390, 100, 26))
            {
                Opaque = true,
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(24),
                TextAlignment = UITextAlignment.Left,
                Text = "0"
            };
            Add(_fpsText);

            _fpsLabel = new UILabel(new RectangleF(210, 390, 100, 26))
            {
                Opaque = true,
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(24),
                TextAlignment = UITextAlignment.Left,
                Text = "fps"
            };
            Add(_fpsLabel);
        }
        #endregion

        #region Private methods
        // image processing method
        void worker()
        {
            // capturing screen image            
            
            using (var screenImage = CGImage.ScreenImage.WithImageInRect(Frame))
            {
                // without disposing this uiimage instance, app fails within a minute.
                if (null != _imageView.Image)
                    _imageView.Image.Dispose();
                _imageView.Image = UIImage.FromImage(screenImage);
                /*
                var jpgimg = _imageView.Image.AsPNG();
                jpgimg.Dispose();*/
            }

            _fpsCount++;
            if (_fpsCount >= 20)
            {
                _end = NSDate.Now;
                if (null != _start && null != _end)
                {
                    var secs = _end.SecondsSinceReferenceDate - _start.SecondsSinceReferenceDate;
                    var fps  =(int)(20f / secs);
                    _fpsText.Text = fps.ToString();
                }

                if (null != _start)
                    _start.Dispose();

                _start = _end;
                _fpsCount = 0;
            }
        }
        #endregion

        #region Public methods
        public void StartWorker()
        {
            // starting timer
            if (null == _timer)
            {
                _timer = NSTimer.CreateRepeatingTimer(new TimeSpan(10000L * 1), delegate { worker(); });
                NSRunLoop.Current.AddTimer(_timer, "NSDefaultRunLoopMode");
            }
        }
        public void StopWorker()
        {
            // starting timer
            if (null != _timer)
            {
                _timer.Invalidate();
                _timer.Dispose();
                _timer = null;
            }
        }
        #endregion

        #region Override methods       
        protected override void Dispose(bool disposing)
        {
            StopWorker();            
            base.Dispose(disposing);
        }
        #endregion
    }
}
