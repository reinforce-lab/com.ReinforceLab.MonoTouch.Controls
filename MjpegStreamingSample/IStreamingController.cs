using System;
using System.Drawing;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    public interface IStreamingController
    {
        int FrameRate { get; set; }
        RectangleF Frame { get; }
        RectangleF RegionOfInterest { get; set; }
    }
}
