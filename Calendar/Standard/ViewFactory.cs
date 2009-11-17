using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{
    public class ViewFactory : IViewFactory
    {
        #region IViewFactory メンバ
        public CacheableView Create(string cell_id)
        {
            return new DayView(new System.Drawing.RectangleF(0, 0, Resources.DAYVIEW_WIDTH, Resources.DAYVIEW_HEIGHT)) as CacheableView;
        }
        #endregion
    }
}
