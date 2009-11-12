using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public abstract class AbsDayView : UIView
    {
        public virtual String CellID { get; set; }
        public virtual DateTime Day { get; set; }

        protected AbsDayView(IntPtr handle)
            : base(handle)
        { }
        protected AbsDayView(RectangleF rect)
            : base(rect)
        { }
    }
}
