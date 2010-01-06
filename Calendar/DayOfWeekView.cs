using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
    public class DayOfWeekView : UIView
    {
        #region Variables
        readonly String[]  _dayLabels;        
        #endregion

        #region Constructor
        public DayOfWeekView(RectangleF rect, DayOfWeek firstDayOfWeek) : base(rect)
        {
            _dayLabels      = new String[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            initialize(firstDayOfWeek);
        }
        protected virtual void initialize(DayOfWeek dow)
        {
            BackgroundColor = UIColor.Gray;
            for (int i = 0; i < 7; i++)
                Add(createLabel(dow, i));
        }
        #endregion

        #region Protected methods
        protected virtual UILabel createLabel(DayOfWeek firstDayOfWeek, int index)
        {
            float height = Frame.Height - 2;
            float width  = Frame.Width / 7;

            var label             = new UILabel(new RectangleF(index * width, 1, width, height));
            label.Font            = UIFont.SystemFontOfSize(height);
            label.Text            = _dayLabels[((int)firstDayOfWeek + index) % 7];
            label.TextColor       = UIColor.Black;
            label.TextAlignment   = UITextAlignment.Center;
            label.BackgroundColor = UIColor.Clear;

            return label;
        }
        #endregion
    }
}
