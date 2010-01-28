using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MonoTouch.UIKit;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar.Standard
{
    public class CalendarController : UIViewController, ICalendarController
    {
        #region Variables        
        readonly ViewCache _dayViewCache;
        CalendarView _calendarView;
        DateTime _currentDay;

        public EventHandler<EventArgs> DaySelectionChanged;
		public EventHandler<EventArgs> MonthSelectionChanged;
        #endregion

        #region Properties 
        public CalendarView CalendarView { get { return _calendarView; } }
        public DateTime CurrentDay       { get { return _currentDay; } }
        public DateTime CurrentMonth     { get { return _calendarView.MonthView.Month; } }
        #endregion

        #region Constructor
        public CalendarController() : base()
        {
            _currentDay   = DateTime.MinValue;
            _dayViewCache = new ViewCache(new ViewFactory());
        }
        #endregion

        #region Public methods
        public override void LoadView()
        {
            base.LoadView();

            _calendarView = new CalendarView(new RectangleF(0, 0, Resources.MONTHVIEW_WIDTH, 200), this, DayOfWeek.Sunday);
            Add(_calendarView);
        }
        public void HighlightDayView(DateTime currentDay) //RJG 1/9/10
        {
            var highlightDay = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day);

            foreach (var day in _calendarView.MonthView.DayViews)
            {
                if (day.Day == highlightDay)
                {
                    day.IsSelected = true;
                }
                else
                {
                    day.IsSelected = false;
                }
            }
        }
        /// <summary>
        /// Setting a marker on a specified date.
        /// </summary>
        /// <param name="day">date to set marker</param>
        public void MarkDay(DateTime day)
        {
            SetDayMark(day, true);
        }
        /// <summary>
        /// Setting a marker on/off of a specified date.
        /// </summary>        
        public void SetDayMark(DateTime day, bool isMarked)
        {
            foreach (var view in _calendarView.MonthView.DayViews)
            {
                if (view.Day == day)
                {
                    view.IsMarked = isMarked;
                    return;
                }
            }
        }
        /// <summary>
        /// Setting markers of days. Markers of days not specified in a argument are cleared.
        /// </summary>        
        public void MarkDay(DateTime [] days)
        {
            Dictionary<DateTime, bool> marking = new Dictionary<DateTime, bool>();
            // building a hash table
            foreach (var day in days)
                marking[day.Date] = true;

            // marker setting
            foreach (var view in _calendarView.MonthView.DayViews)
            {
                if (!marking.ContainsKey(view.Day))
                {
                    view.IsMarked = false; 
                }
                else
                {
                    view.IsMarked = marking[view.Day];
                }
            }
        }
        #endregion

        #region ICalendarController メンバ
        public ViewCache DayViewCache { get { return _dayViewCache; } }

        public void DaySelected(DateTime day)
        {
            Debug.WriteLine(String.Format("DayView is selected. date: {0}.", day.ToString("d")));            
            _currentDay = day;
            // move to next/prev month
            if (_calendarView.MonthView.Month.Year != day.Year || _calendarView.MonthView.Month.Month != day.Month)
            {                
                if (_calendarView.MonthView.Month < day)
                    _calendarView.MoveToNextMonth(day);
                else
                    _calendarView.MoveToPrevMonth(day);
            }
			HighlightDayView(_currentDay);
            // Invoke dayselectionchanged event 
            if (null != DaySelectionChanged)
            {
                DaySelectionChanged.Invoke(this, null);
            }
        }
						
        public void CalendarViewChanged(DateTime currentMonth, DateTime previousMonth)
        {
            Debug.WriteLine(String.Format("Visible month changed. new date:{0} prev:{1}.", currentMonth.ToString("y"), previousMonth.ToString("y")));            
			foreach (var day in _calendarView.MonthView.DayViews)
            {
                if (day.Day == _currentDay)
                {
                    day.IsSelected = true;
                    return;
                }
            }
            			            
            // Invoke dayselectionchanged event 
            if (null != MonthSelectionChanged)
            {
                MonthSelectionChanged.Invoke(this, null);
            }
        }
        #endregion
    }
}
