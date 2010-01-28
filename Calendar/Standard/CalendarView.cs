using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar.Standard
{
	[Register("CalendarView")]
    public class CalendarView : UIView
    {
        #region Variables
        const String _ScrollAnimationStoppedHandler = "_ScrollAnimationStoppedHandler";
        readonly String[] DayLabels = new String[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        readonly CalendarController _ctr;
        readonly DayOfWeek          _firstDayOfWeek;

        UILabel    _titleLabel;
        UILabel[]  _daysLabel;
        UIButton   _leftButton, _rightButton;
        MonthView  _monthView, _nextMonthView;
        UIScrollView _scollView;
        #endregion

        #region Properties        
        public MonthView MonthView { get { return _monthView; } }
        public DayOfWeek FirstDayOfWeek {get {return _monthView.FirstDayOfWeek;} }        
        #endregion

        #region Constructors
       public CalendarView(IntPtr ptr)
            : base(ptr)
        { }
        public CalendarView(RectangleF rect, CalendarController ctr, DayOfWeek firstDayOfWeek)
            : base(rect)
        {
            _ctr = ctr;            
            _firstDayOfWeek = firstDayOfWeek;

            _monthView = new MonthView(new RectangleF(0, 0, Resources.MONTHVIEW_WIDTH, 300), _ctr, DateTime.Now, _firstDayOfWeek);

            _scollView = new UIScrollView(new RectangleF(0, Resources.TITLE_HEIGHT, Resources.MONTHVIEW_WIDTH, _monthView.Frame.Height));
            _scollView.ScrollEnabled = false;
            _scollView.Add(_monthView); 
            Add(_scollView);            

            buildButtons();
            buildTitleLabel();
            buildDayLabels();
            updateDayLabels();
            updateFrame();

            _titleLabel.Text = _monthView.Month.ToString("y");

            //Frame = new RectangleF(Frame.Location, new SizeF(Resources.MONTHVIEW_WIDTH, _monthView.Frame.Height + Resources.TITLE_HEIGHT));
        }
        #endregion

        #region Protected methods
        protected virtual void buildButtons()
        {
            _rightButton = new UIButton(new RectangleF(Resources.MONTHVIEW_WIDTH - 56, 0, 44, 42));
            _rightButton.SetImage(UIImage.FromFile(Resources.RightArrorImage), UIControlState.Normal);
            _rightButton.TouchUpInside += delegate { moveToNextMonth(_monthView.Month.AddMonths(1) ); };
            AddSubview(_rightButton);

            _leftButton = new UIButton(new RectangleF(10, 0, 44, 42));
            _leftButton.SetImage(UIImage.FromFile(Resources.LeftArrorImage), UIControlState.Normal);
            _leftButton.TouchUpInside += delegate { moveToPrevMonth(_monthView.Month.AddMonths(-1) ); };
            AddSubview(_leftButton);
        }
        protected virtual void buildTitleLabel()
        {
            // title text            
            _titleLabel = new UILabel(new RectangleF(54, 10, Resources.MONTHVIEW_WIDTH - 54 * 2, 20));
            _titleLabel.Font = UIFont.BoldSystemFontOfSize(20);
            _titleLabel.TextColor = UIColor.DarkGray;
            _titleLabel.Text = String.Empty;
            _titleLabel.TextAlignment = UITextAlignment.Center;
            _titleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            _titleLabel.BackgroundColor = UIColor.Clear;
            Add(_titleLabel);
        }
        protected virtual void buildDayLabels()
        {
            _daysLabel = new UILabel[7];
            for (int i = 0; i < 7; i++)
            {
                var label = new UILabel(new RectangleF(i * Resources.DAYVIEW_WIDTH, Resources.TITLE_HEIGHT - 12, Resources.DAYVIEW_WIDTH, 10));
                label.Font = UIFont.SystemFontOfSize(10);
                label.Text = String.Empty;
                label.TextColor = UIColor.DarkGray;
                label.TextAlignment = UITextAlignment.Center;
                label.LineBreakMode = UILineBreakMode.WordWrap;
                label.BackgroundColor = UIColor.Clear;
                _daysLabel[i] = label;
                Add(label);
            }
        }
        protected virtual void drawTitle()
        {
            var img = UIImage.FromFile(Resources.TopBarImage);
            img.Draw(new PointF(0, 0));
        }
        #endregion

        #region private methods
        void updateDayLabels() 
        {                                    
            for (int i = 0; i < 7; i++)
                _daysLabel[i].Text = DayLabels[((int)_monthView.FirstDayOfWeek + i) % 7];
        }
        void updateFrame()
        {
            _scollView.Frame = new RectangleF(_scollView.Frame.Location, new SizeF(Resources.MONTHVIEW_WIDTH, _monthView.Frame.Height));
            Frame = new RectangleF(Frame.Location, new SizeF(Resources.MONTHVIEW_WIDTH, _scollView.Frame.Height + Resources.TITLE_HEIGHT));
        }                
        void scrollCalendar(float distance)
        {
            UserInteractionEnabled = false;

            Frame = new RectangleF(Frame.Location, new SizeF(Resources.MONTHVIEW_WIDTH, _nextMonthView.Frame.Height + Resources.TITLE_HEIGHT));

            // animation
            UIView.BeginAnimations("month view scrolling");
            UIView.SetAnimationDuration(Resources.ANIMATION_DURATION);
            UIView.SetAnimationDelay(Resources.ANIMATION_DELAY);
            UIView.SetAnimationDelegate(this);
            UIView.SetAnimationDidStopSelector(new Selector(_ScrollAnimationStoppedHandler));
            
            _titleLabel.Text     = _nextMonthView.Month.ToString("y");
            _monthView.Frame     = new RectangleF(_monthView.Frame.X, _monthView.Frame.Y + distance, _monthView.Frame.Width, _monthView.Frame.Height);
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X, _nextMonthView.Frame.Y + distance, _nextMonthView.Frame.Width, _nextMonthView.Frame.Height);
            _scollView.Frame = new RectangleF(_scollView.Frame.Location, new SizeF(Resources.MONTHVIEW_WIDTH, _nextMonthView.Frame.Height));            
           
            UIView.CommitAnimations();
            
            SetNeedsDisplay();                                   
            //Debug.WriteLine("\tCalendarView: Frame after monthview updated: {0}.", Frame);
        }
        void moveToNextMonth(DateTime nextMonth)
        {
            _nextMonthView = new MonthView(RectangleF.Empty, _ctr, nextMonth, _firstDayOfWeek);
            _nextMonthView.Frame = new RectangleF(0, _monthView.Frame.Height, Resources.MONTHVIEW_WIDTH, _nextMonthView.Frame.Height);
            _scollView.Add(_nextMonthView);            
            
            scrollCalendar(-1 * _monthView.Frame.Height);            
        }        
        void moveToPrevMonth(DateTime prevMonth )
        {
            _nextMonthView = new MonthView(RectangleF.Empty, _ctr, prevMonth, _firstDayOfWeek);
            _nextMonthView.Frame = new RectangleF(0, -_nextMonthView.Frame.Height, Resources.MONTHVIEW_WIDTH, _nextMonthView.Frame.Height); 
            _scollView.Add(_nextMonthView);

            scrollCalendar(_nextMonthView.Frame.Height);            
        }        
        
        [Export(_ScrollAnimationStoppedHandler)]
        void _AnimationStopped()
        {
            DateTime prevMonth = _monthView.Month;            

            _monthView.RemoveFromSuperview();
            _monthView.Dispose(); // reuse day views

            _monthView = _nextMonthView;            
            _nextMonthView = null;
            
            _ctr.CalendarViewChanged(_monthView.Month, prevMonth);

            UserInteractionEnabled = true;
        }        
        
        #endregion

        #region Public methods
        public void MoveToNextMonth(DateTime nextMonth)
        {
            moveToNextMonth(new DateTime(nextMonth.Year, nextMonth.Month,1) );
        }
        public void MoveToPrevMonth(DateTime prevMonth )
        {
            moveToPrevMonth(new DateTime(prevMonth.Year, prevMonth.Month, 1));
        }
        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
            drawTitle();
        }
        #endregion
    }	
}