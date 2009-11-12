using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MonoTouch.UIKit;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public class CalendarController : UIViewController
    {
        #region Variables
        protected CalendarView    _calendarView;
        protected ICalendarDayView _currentDay;
        #endregion

        #region Constructor
        public CalendarController() : base()
        { }
        #endregion

        #region Private methods
        void view_DaySelected(object sender, DaySelectedEventArgs e)
        {
            Debug.WriteLine(String.Format("DayView is selected. date: {0}, mode: {1}.", e.DayView.Day.Date, e.Mode));
            if (e.DayView.Day.Month != e.MonthView.Month.Month)
            {
                if (e.Mode == TouchMode.Ended)
                {
                    _currentDay = e.DayView;
                    if (e.DayView.Day.Month > e.MonthView.Month.Month)
                        _calendarView.MoveToNextMonth();
                    else
                        _calendarView.MoveToPrevMonth();
                }
            }
            else
            {
                switch (e.Mode)
                {
                    case TouchMode.Canceled: break;
                    case TouchMode.Began: break;
                    case TouchMode.Ended:
                        if (null != _currentDay)
                            _currentDay.IsSelected = false;
                        e.DayView.IsSelected = true;
                        _currentDay = e.DayView;
                        break;
                    case TouchMode.Moved:
                        if (null != _currentDay && _currentDay.Day != e.DayView.Day)
                            _currentDay.IsSelected = false;
                        e.DayView.IsSelected = true;
                        _currentDay = e.DayView;
                        break;
                }
            }
        }
        void view_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            Debug.WriteLine(String.Format("Visible month changed. new date:{0} prev:{1}.", e.NewDate.ToString("y"), e.PreviousDate.ToString("y")));
            if (null != _currentDay)
            {
                var cview = sender as CalendarView;
                foreach (var day in cview.MonthView.DayViews)
                {
                    if (day.Day == _currentDay.Day)
                    {
                        day.IsSelected = true;
                        _currentDay = day;
                        break;
                    }
                }
            }
        }
        #endregion

        #region Public methods
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _calendarView = new CalendarView(new RectangleF(0, 0, 320, 200));
            _calendarView.VisibleMonthChanged += new MonthChangedEventHandler(view_VisibleMonthChanged);
            _calendarView.DaySelected         += new EventHandler<DaySelectedEventArgs>(view_DaySelected);

            View.Add(_calendarView);
        }
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            _calendarView.VisibleMonthChanged -= new MonthChangedEventHandler(view_VisibleMonthChanged);
            _calendarView.DaySelected         -= new EventHandler<DaySelectedEventArgs>(view_DaySelected);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }
        #endregion
    }
}
