using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    class CameraImageSource : IImageSource
    {
        #region Variables
        readonly RectangleF _frame;
        #endregion

        #region Constructor
        public CameraImageSource(RectangleF frame)
        {
            _frame = frame;
        }
        #endregion

        #region IImageSource メンバ
        public RectangleF Frame { get { return _frame; } }
        public byte[] GetImageAsJpeg(RectangleF roi)
        {
            using (var screenImage = CGImage.ScreenImage.WithImageInRect(_frame))
            {
                using(var uiimage = UIImage.FromImage(screenImage))
                {
                    using (var data = uiimage.AsJPEG())
                    {
                        var buff = new byte[data.Length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, buff, 0, buff.Length);

                        return buff;
                    }
                }             
            }
        }
        #endregion
    }  
}
