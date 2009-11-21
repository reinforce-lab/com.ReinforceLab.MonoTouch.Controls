using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{	
	public abstract class AbsWeekView<T> : UIView 
        where T: UIView, IDayView
	{
		#region Variables
        protected float _DayViewHeight = 45f;
        protected float _DayViewWidth  = 46f;

        protected readonly DateTime            _firstDayOfWeek;		
        protected readonly ICalendarController _ctr;

        protected T[] _dayViews;
		#endregion
				
		#region Properties
        public T[] DayViews { get { return _dayViews; } }

        /// <summary>
        /// get the 1st day of a week. This property can be set until this view drawin.
        /// </summary>
        public DateTime FirstDayOfWeek
        {
            get { return _firstDayOfWeek; }
        }
		#endregion
		
		#region Constructors
		public AbsWeekView (RectangleF rect, ICalendarController ctr, DateTime dow) : base(rect)
		{
            _ctr = ctr;
            _firstDayOfWeek = new DateTime(dow.Year, dow.Month, dow.Day);
            initialize();                
		}
        protected virtual void initialize()
        {
            Frame = new RectangleF(Frame.Location, new SizeF(_DayViewWidth *7, _DayViewHeight)); 
            buildDayViews();
        }
		#endregion	
		
		#region Private methods		       
        void buildDayViews()
        {
            if (null != _dayViews) return;

            List<DateTime> days = new List<DateTime>();
            // setting previous month days
            DateTime dt = new DateTime(_firstDayOfWeek.Ticks);
            for(int i = 0; i < 7; i++)
            {
                days.Add(dt);
                dt = dt.AddDays(1.0);
            }
                        
            var dayViews = new List<T>();
            for(int i =0; i < days.Count; i++)
            {
                var rect = new RectangleF(i * _DayViewWidth, 0, _DayViewWidth, _DayViewHeight);
                var dayView = createDayView(rect, days[i]);
                Add(dayView as UIView);                
                dayViews.Add(dayView);
            }
            _dayViews = dayViews.ToArray();            
        }
        void selectDay(UITouch touch)
        {
            PointF point = touch.LocationInView(this);
            
            int index = (int)point.X / (int)_DayViewWidth;

            if (0<= index && index < _dayViews.Length)
                _ctr.DaySelected(_dayViews[index].Day);
        }
		#endregion

        #region protected methods
        protected virtual T createDayView(RectangleF rect, DateTime date)
        {
            var dayView = _ctr.DayViewCache.GetView("_cell") as T;
            dayView.Day = date;
            return dayView;
        }
        #endregion

        #region public methods
        public override void Draw(RectangleF rect)
        {
            base.Draw(rect); 
        }
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);                                    
            selectDay((UITouch)touches.AnyObject);
        }
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
 	        base.TouchesCancelled(touches, evt);
            selectDay((UITouch)touches.AnyObject);             
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            selectDay((UITouch)touches.AnyObject); 
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            selectDay((UITouch)touches.AnyObject); 
        }        
        protected override void Dispose(bool disposing)
        {
            //System.Diagnostics.Debug.WriteLine("MonthView: Dispose()");            
            foreach (var item in _dayViews)
                item.RemoveFromSuperview();

            base.Dispose(disposing);
        }
        #endregion
    }
}
