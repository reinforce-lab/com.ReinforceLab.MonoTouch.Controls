using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{
    public class CalendarController : UIViewController, ICalendarController
    {
        #region Variables        
        readonly ViewCache _dayViewCache;
        CalendarView _calendarView;
        DateTime _currentDay;
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
        #endregion

        #region ICalendarController メンバ
        public ViewCache DayViewCache { get { return _dayViewCache; } }

        public void DaySelected(DateTime day)
        {
            Debug.WriteLine(String.Format("DayView is selected. date: {0}.", day.Day));

            if (_calendarView.MonthView.Month.Month != day.Month)
            {
                // move to next/prev month
                if (_calendarView.MonthView.Month < day)
                    _calendarView.MoveToNextMonth();
                else
                    _calendarView.MoveToPrevMonth();

            }
            if (_currentDay != day)
            {
                foreach (var view in _calendarView.MonthView.DayViews)
                {
                    if (view.Day == _currentDay)
                        view.IsSelected = false;                        
                    if (view.Day == day)
                        view.IsSelected = true;
                }
            }            
            _currentDay = day;
        }

        public void MonthChanged(DateTime currentMonth, DateTime previousMonth)
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
            _currentDay = DateTime.MinValue; 
        }
        #endregion
    }
}
