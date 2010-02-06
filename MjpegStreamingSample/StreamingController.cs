using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Web;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    public class StreamingController : IStreamingController, IDisposable
    {
        #region Variables
        readonly WebServiceHost _host;
        readonly IImageSource   _imageSource;
        readonly IMjpegStreamingService _service;

        int _frameRate;
        NSTimer _timer;
        #endregion

        #region Constructor
        public StreamingController(Uri baseAddress, RectangleF frame)
        {
            // setting up data source and web service
            _service     = new MjpegStreamingService(this);
            _imageSource = new CameraImageSource(frame);

            // setting up a web service point
            _host = new WebServiceHost(_service, baseAddress);
            var bnd = new WebHttpBinding();
            // -important- to stream image, "streamed" transfermode is required.
            bnd.TransferMode = TransferMode.Streamed;
            // streaming session time is restricted to 30-sec in this demonstration code
            bnd.SendTimeout = new TimeSpan(0, 0, 30);
            _host.AddServiceEndpoint(typeof(IMjpegStreamingService), bnd, baseAddress);

            // start streaming
            setFrameRate(3);
            startTimer();
            _host.Open();
        }
        #endregion

        #region Private methods
        void stopTimer()
        {
            if (null != _timer)
            {
                _timer.Invalidate();                
                _timer.Dispose();
                _timer = null;
            }
        }
        void startTimer()
        {
            stopTimer();

            _timer = NSTimer.CreateRepeatingTimer(new TimeSpan(0,0,0,0, (int)(1000.0 / _frameRate)), delegate { _timer_Elapsed(); });
            NSRunLoop.Current.AddTimer(_timer, "NSDefaultRunLoopMode");
            
        }
        void restartTimer()
        {
            startTimer();
        }
        void setFrameRate(int frameRate)
        {
            if (_frameRate == frameRate) return;
            _frameRate = Math.Min(15, Math.Max(1, frameRate));
        }
        #endregion

        #region Event handlers
        void _timer_Elapsed()
        {
            //System.Diagnostics.Debug.WriteLine("StreamingController._timer_Elapsed()");

            // check streaming buffer
            if (_service.StreamingBufferStalled) return;

            // get camaera image
            var img = _imageSource.GetImageAsJpeg(_imageSource.Frame);
            if (null == img) return;

            // stream it
            _service.AddImage(img);
        }
        #endregion

        #region IStreamingController メンバ
        public int FrameRate
        {
            get { return _frameRate; }
            set { setFrameRate(value); }
        }
        public System.Drawing.RectangleF Frame
        {
            get { return _imageSource.Frame; }
        }
        public System.Drawing.RectangleF RegionOfInterest
        {
            get { return _imageSource.Frame; }
            set { }
        }
        #endregion

        #region IDisposable メンバ
        public void Dispose()
        {
            stopTimer();
            _host.Close();
        }
        #endregion
    }
}
