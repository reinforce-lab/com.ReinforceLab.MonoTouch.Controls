using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{    
    public abstract class AbsCalendarController :UIViewController, ICalendarController
    {
        #region Variables
        readonly Dictionary<String, Stack<AbsDayView>> _viewCache = new Dictionary<string, Stack<AbsDayView>>();
        #endregion

        #region ICalendarController メンバ
        public IDayViewSource Source {get; set;}
        public AbsDayView DequeueReusableView(string cell_id)
        {
            if (!_viewCache.ContainsKey(cell_id) || _viewCache[cell_id].Count == 0) return null;
            return _viewCache[cell_id].Pop();
        }
        public void EnqueueReusableView(AbsDayView view)
        {
            if (!_viewCache.ContainsKey(view.CellID))
                _viewCache[view.CellID] = new Stack<AbsDayView>();
            _viewCache[view.CellID].Push(view);            
        }
        
        public virtual void DaySelected(AbsDayView dayView) {}
        public virtual void MonthChanged(DateTime currentMonth, DateTime previousMonth) { }
        #endregion

        #region ICalendarController メンバ        
        public virtual void FocusToDate(DateTime date, bool activateDay){}        
        #endregion        
    }
}
