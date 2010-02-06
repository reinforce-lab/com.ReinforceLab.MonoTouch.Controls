using System;
using System.Drawing;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    public interface IImageSource
    {
        RectangleF Frame { get; }
        Byte[] GetImageAsJpeg(RectangleF roi);
    }
}
