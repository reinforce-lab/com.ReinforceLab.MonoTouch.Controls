using System;
using System.Drawing;
using System.Runtime.InteropServices;

using MonoTouch.CoreGraphics;

namespace com.ReinforceLab.iPhone.Controls.AugmentedRealityBase
{
    // gray-scale raw bitmap class
    public class RawBitmap : IDisposable
    {
        #region Variables
        IntPtr _buffer;
        CGBitmapContext _context;
        #endregion

        #region Properties
        public CGBitmapContext Context { get { return _context; } }

        public int Width { get { return _context.Width; } }
        public int Height { get { return _context.Height; } }
        #endregion

        #region Constructor
        public RawBitmap(int width, int height)
        {
            _buffer = Marshal.AllocHGlobal(width * height);
            var colorSpace = CGColorSpace.CreateDeviceGray();
            _context = new CGBitmapContext(
                           _buffer, width, height,
                           8, width,
                           colorSpace,
                           CGImageAlphaInfo.None);
            colorSpace.Dispose();
            // lowest possible quality for speed                    
            _context.InterpolationQuality = CGInterpolationQuality.None;
            _context.SetAllowsAntialiasing(false);
        }
        #endregion

        #region Public methods
        public void Draw(CGImage image)
        {
            _context.DrawImage(new RectangleF(0, 0, _context.Width, _context.Height), image);
        }
        public Byte ReadPixel(int x, int y)
        {
            return System.Runtime.InteropServices.Marshal.ReadByte(_buffer, x + y * _context.Width);
        }
        public void WritePixel(int x, int y, byte val)
        {
            System.Runtime.InteropServices.Marshal.WriteByte(_buffer, x + y * _context.Width, val);
        }
        #endregion

        #region IDisposable メンバ
        public void Dispose()
        {
            if (null != _context)
            {
                _context.Dispose();
                _context = null;

                Marshal.FreeHGlobal(_buffer);
            }
        }
        #endregion
    }
}
