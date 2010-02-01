using System;
using System.Text;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace com.ReinforceLab.MonoTouch.Controls.AugmentedRealityBase
{
    class EdgeDetectionView : UIView
    {
        #region Variables
        CGPath  _path;
        int _fps;
        DateTime _lastFpsCheckedAt;

        NSTimer _timer;
        RawBitmap _drawnImage, _buffer;
        CGImage _maskImage;
        #endregion

        #region Constructor
        public EdgeDetectionView()
            : base()
        {
            initialize();
        }
        void initialize()
        {
            _fps = 0;
            _lastFpsCheckedAt = DateTime.Now;

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
            
            // creating checkerboard mask image
            using (var checkerBoardImage = new RawBitmap((int)Frame.Width, (int)Frame.Height))
            { 
                for(int y =0; y < checkerBoardImage.Height; y++)
                {                    
                    for(int x=0; x<checkerBoardImage.Width; x++)
                    {
                        var val = ((y%2 == 0 && x%2 == 0) || (y%2 == 1 && x%2==1)) ? 255 : 0;                        
                        checkerBoardImage.WritePixel(x, y, (Byte)val);
                    }
                }                
                _maskImage = checkerBoardImage.Context.ToImage();
			}
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
                // TODO use OpenTK FBO to get hardware acceleration
                unsafe
                {
                    var bufPtr   = _buffer.BaseAddress;
                    var drawnPtr = _drawnImage.BaseAddress;
                    var height   = _buffer.Height;
                    var width    = _buffer.Width;
                    // if we draw to this pixel replace it with the average of the surrounding pixels
                    for (int y = 1; y < height-1; y++)
                    {
                        var scan0 = width * (y -1);
                        var scan1 = width * y;
                        var scan2 = width * (y +1);
                        for (int x = 1; x < width-1; x++)
                        {
                            ++scan0;
                            ++scan1;
                            ++scan2;

                            if (drawnPtr[scan1] != 0)
                            {
                                var val = (bufPtr[scan0] + bufPtr[scan2] + bufPtr[scan1 -1] + bufPtr[scan1 + 1]) / 4;
                                bufPtr[scan1] = (Byte)val;                                
                            }
                        }
                    }
                }
            }
            /* trial 1, marshaling, ~0.5fps
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
            */

            var path = new CGPath();
            int lastX = -1000, lastY = -1000;
            unsafe
            {                    
                var bufPtr   = _buffer.BaseAddress;                                        
                var height   = _buffer.Height;                    
                var width    = _buffer.Width;
                for (int y = 0; y < height - 1; y++)
                {
                    var scan0 = width * y;
                    var scan1 = width * (y + 1);
                    for (int x = 0; x < width - 1; x++)
                    {
                        int edge = (Math.Abs(bufPtr[scan0] - bufPtr[scan0 + 1]) + Math.Abs(bufPtr[scan0] - bufPtr[scan1])) / 2;
                        if (edge > 10)
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
                        ++scan0;
                        ++scan1;
                    }
                }
            }


            /* trial 1, marshaling, ~0.5fps
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
            }*/

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
                //System.Diagnostics.Debug.WriteLine("\tupdating path.");
                // we're going to draw into an image using our checkerboard mask
                UIGraphics.BeginImageContext(this.Bounds.Size);
                var context = UIGraphics.GetCurrentContext();
                context.ClipToMask(this.Bounds, _maskImage);
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

                _fps++;
                if (_fps >= 20)
                {
                    var dtnow = DateTime.Now;
                    var span  = dtnow - _lastFpsCheckedAt;
                    _lastFpsCheckedAt = dtnow;
                    var fps = (int)((float)_fps * 1000f / (float)span.Milliseconds);
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} Fps.", fps));

                    _fps = 0;
                }
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
