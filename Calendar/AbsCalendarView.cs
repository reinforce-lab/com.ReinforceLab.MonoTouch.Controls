using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{	
	public abstract class AbsCalendarView<T> : UIView 
        where T: UIView, IDayView
	{
		#region Variables
        protected readonly ICalendarController _ctr;
        protected T[] _dayViews;
        T _selectedView;        
		#endregion
				
		#region Properties
        public T[] DayViews { get { return _dayViews; } }
		#endregion
		
		#region Constructors
		public AbsCalendarView (RectangleF rect, ICalendarController ctr) : base(rect)
		{
            _ctr = ctr;            
		}
		#endregion	

        #region Protected methods        
        protected abstract T hitTestDayView(PointF point);        
        
        protected virtual void dayViewSelected(T selected, T previous) { }        
        protected virtual void selectDayView(UITouch touch)
        {
            PointF point = touch.LocationInView(this);
            T dv = hitTestDayView(point);

            if (dv == _selectedView) return;

            dayViewSelected(dv, _selectedView);
            _selectedView = dv;
            if (null != dv)
                _ctr.DaySelected(dv.Day);            
        }       
        protected virtual T createDayView(RectangleF rect, DateTime date)
        {
            var dayView = _ctr.DayViewCache.GetView("_cell") as T;
            dayView.Day = date;
            return dayView;
        }
        #endregion

        #region public methods
        public void SelectDay(DateTime day)
        {
            T dv = null;
            foreach (var item in _dayViews)
            {
                if (item.Day == day)
                {
                    dv = item;
                    break;
                }
            }
            if (dv == _selectedView) return;

            dayViewSelected(dv, _selectedView);
            _selectedView = dv;
            if (null != dv)
                _ctr.DaySelected(dv.Day);
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
        }
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            selectDayView((UITouch)touches.AnyObject);
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            selectDayView((UITouch)touches.AnyObject);
        }
        protected override void Dispose(bool disposing)
        {
            foreach (var item in _dayViews)
                item.RemoveFromSuperview();

            base.Dispose(disposing);
        }
        #endregion     
    }
}
