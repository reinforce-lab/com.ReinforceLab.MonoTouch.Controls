using System;
using System.Text;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace com.ReinforceLab.iPhone.Controls.AugmentedRealityBase
{
    class EdgeDetectionView : UIView
    {
        #region Variables
        CGPath  _path;
        NSTimer _timer;
        RawBitmap _drawnImage, _buffer;
        #endregion

        #region Constructor
        public EdgeDetectionView()
            : base()
        {
            initialize();
        }
        void initialize()
        {
            _path = null;

            Frame = new System.Drawing.RectangleF(0, 0, 320, 480 - 44);
            // important - it needs to be transparent so the camera preview shows through!
            Opaque = false;
            BackgroundColor = UIColor.Clear;

            // allocating bitmap buffer
            _buffer = new RawBitmap((int)Frame.Width, (int)Frame.Height);
            _drawnImage = new RawBitmap((int)Frame.Width, (int)Frame.Height);
            for (int y = 0; y < _drawnImage.Height; y++)
                for (int x = 0; x < _drawnImage.Width; x++)
                    _drawnImage.WritePixel(x, y, 0);
        }
        #endregion

        #region Private methods
        // image processing method
        void worker()
        {            
            // turn screen image into gray-scale raw bitmap byte-stream
            using (var screenImage = CGImage.ScreenImage.WithImageInRect(Frame))
            {
                _buffer.Draw(screenImage);
            }

            // process the image to remove our drawing - WARNING the edge pixels of the image are not processed                
            if (null != _drawnImage)
            {
                for (int y = 1; y < _buffer.Height; y++)
                {
                    for (int x = 1; x < _buffer.Width; x++)
                    {
                        // if we draw to this pixel replace it with the average of the surrounding pixels                    
                        if (_drawnImage.ReadPixel(x, y) != 0)
                        {
                            var val = (byte)(((int)_buffer.ReadPixel(x, y - 1) + (int) _buffer.ReadPixel(x, y + 1) + (int)_buffer.ReadPixel(x - 1, y) + (int)_buffer.ReadPixel(x + 1, y)) / 4);
                            _buffer.WritePixel(x, y, val);
                        }
                    }
                }
            }

            var path = new CGPath();
            int lastX = -1000, lastY = -1000;
            for (int y = 0; y < _buffer.Height - 1; y++)
            {
                for (int x = 0; x < _buffer.Width - 1; x++)
                {
                    int edge = (Math.Abs(_buffer.ReadPixel(x, y) - _buffer.ReadPixel(x + 1, y)) + Math.Abs(_buffer.ReadPixel(x, y) - _buffer.ReadPixel(x, y + 1))) / 2;
                    if (edge > 20)
                    {
                        int dist = (int)(Math.Pow(x - lastX, 2) + Math.Pow(y - lastY, 2));
                        if (dist > 50)
                        {
                            lastX = x;
                            lastY = y;
                        }
                        else if (dist > 10)
                        {
                            path.AddLines(new PointF[] { new PointF(lastX, lastY), new PointF(x, y) });
                            lastX = x;
                            lastY = y;
                        }
                    }
                }
            }

            _path = path;
            SetNeedsDisplay();
        }       
        #endregion

        #region Public methods
        public void StartWorker()
        {
            // starting timer
            if (null == _timer)
            {
                // 50msec
                _timer = NSTimer.CreateRepeatingTimer(new TimeSpan(10000L * 50), delegate { worker(); }); 
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
        public override void Draw(System.Drawing.RectangleF rect)
        {
            base.Draw(rect);

            // drawing a path
            if (null != _path)
            {
                System.Diagnostics.Debug.WriteLine("\tupdating path.");
                // we're going to draw into an image using our checkerboard mask
                UIGraphics.BeginImageContext(this.Bounds.Size);
                var context = UIGraphics.GetCurrentContext();
                //context.ClipToMask(this.Bounds, _maskImage);
                // do your drawing here                
                context.SetLineWidth(2);
                context.SetStrokeColorWithColor(UIColor.Green.CGColor);
                context.AddPath(_path);
                context.StrokePath();
                var imageToDraw = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                // now do the actual drawing of the image
                var drawContext = UIGraphics.GetCurrentContext();
                drawContext.ScaleCTM(1, -1); // upside down
                drawContext.TranslateCTM(1, -1 * Frame.Height); // Set O()
                // very important to switch these off - we don't wnat our grid pattern to be disturbed in any way
                drawContext.InterpolationQuality = CGInterpolationQuality.None;
                drawContext.SetAllowsAntialiasing(false);
                drawContext.DrawImage(Frame, imageToDraw.CGImage);
                // stash the results of our drawing so we can remove them later
                _drawnImage.Draw(imageToDraw.CGImage);
                imageToDraw.Dispose();

                _path.Dispose();
                _path = null;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (null != _timer)
            {
                _timer.Invalidate();
                _timer.Dispose();
                _timer = null;
            }
            if (null != _buffer)
            {
                _buffer.Dispose();
                _buffer = null;
            }
            if (null != _drawnImage)
            {
                _drawnImage.Dispose();
                _drawnImage = null;
            }
            base.Dispose(disposing); 
        }
        #endregion
    }
}
