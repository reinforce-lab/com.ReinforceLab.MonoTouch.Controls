using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
    public abstract class CacheableView : UIView
    {
        #region Properties
        public String CellID { get; set; }
        public ViewCache ViewCache { get; set; }
        #endregion

        #region Constructor
        public CacheableView(IntPtr ptr)
            : base(ptr)
        {
            initialize();
        }
        public CacheableView(RectangleF rect)
            : base(rect)
        {
            initialize();
        }
        void initialize()
        {
            CellID = String.Empty;
        }
        #endregion

        #region Public methods
        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            if (!String.IsNullOrEmpty(CellID) && null != ViewCache)
                ViewCache.Enqueue(this);
        }
        #endregion
    }
}
