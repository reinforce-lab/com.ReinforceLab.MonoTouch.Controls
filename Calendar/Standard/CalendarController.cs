using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{
    public class CalendarController : AbsCalendarController
    {
        #region Variables
        protected CalendarView    _calendarView;
        DateTime _currentDay;
        #endregion

        #region Constructor
        public CalendarController() : base()
        {
            _currentDay = DateTime.MinValue;
        }
        #endregion

        #region Public methods
        public override void LoadView()
        {
            base.LoadView();

            _calendarView = new CalendarView(new RectangleF(0, 0, 320, 200), this, DayOfWeek.Sunday);
            Add(_calendarView);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        #endregion

        #region AbsCalendarController imp methods
        public void MonthViewChanged(DateTime currentMonth, DateTime previousMonth)
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
        public override void DaySelected(AbsDayView view)
        {
            var dayView = view as CalendarDayView;

            Debug.WriteLine(String.Format("DayView is selected. date: {0}.", dayView.Day));

            if (_calendarView.MonthView.Month.Month != dayView.Day.Month)
            {
                // move to next/prev month
                if (_calendarView.MonthView.Month < dayView.Day)
                    _calendarView.MoveToNextMonth();
                else
                    _calendarView.MoveToPrevMonth();

            }
            foreach (var day in _calendarView.MonthView.DayViews)
            {
                if (day.Day == _currentDay)
                {
                    day.IsSelected = false;
                    break;
                }
            }
            dayView.IsSelected = true;
            _currentDay = dayView.Day;
        }
        public override void FocusToDate(DateTime date, bool activateDay)
        {
        }
        #endregion
    }
}
