using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
    public class CalendarView : UIView
    {
        #region Variables
        const String _ScrollAnimationStoppedHandler = "_ScrollAnimationStoppedHandler";
        readonly String[] DayLabels = new String[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public const float TITLE_HEIGHT    = 44f;
        public const float MONTHVIEW_WIDTH = 320f;
        public const float DAYVIEW_WIDTH   = 46f;
        public const float DAYVIEW_HEIGHT  = 45f;

        const double ANIMATION_DURATION = 0.3;
        const double ANIMATION_DELAY = 0.1;

        UILabel   _titleLabel;
        UILabel[] _daysLabel;
        UIButton _leftButton, _rightButton;
        CalendarMonthView     _monthView, _nextMonthView;
        UIScrollView _scollView;
        #endregion

        #region Events
        //public event DayRenderEventHandler DayRender;
        //public event EventHandler             SelectionChanged;
        public event EventHandler DaySelected;
        public event MonthChangedEventHandler VisibleMonthChanged;        
        #endregion

        #region Properties        
        public DayOfWeek FirstDayOfWeek 
        {
            get {return _monthView.FirstDayOfWeek;}
            set {
                if (value != _monthView.FirstDayOfWeek)
                {
                    _monthView.FirstDayOfWeek = value;
                    updateDayLabels();
                    updateFrame(); 
                }                
            }
        }        
        /// <summary>
        /// Gets or sets the current calendar month
        /// </summary>
        public DateTime CurrentMonth {
            get {return _monthView.Month;}
            set {
                if (value != _monthView.Month)
                {
                    _monthView.Month = value; 
                    updateFrame();
                }                 
            }
        }
        // currently not supported       
        /*
        public DateTime SelectedDate     { get; private set; }
        bool _ShowNextPrevMonth;
        public bool ShowNextPrevMonth 
        { 
            get {return _ShowNextPrevMonth; }
            set
            {
                if (value != _ShowNextPrevMonth)
                {
                    _ShowNextPrevMonth = value;
                    _buildSubView      = true;
                    SetNeedsDisplay(); // redraw
                }
            }
        }
        public bool ShowTitle { get; set; }
        public DateTime SelectedDates { get; set; } 
        public CalendarSelectionMode SelectionMode { get; set; }
        */
        #endregion

        #region Constructors
        public CalendarView(RectangleF rect) : base(rect)
        {
            initialize();
        }
        void initialize()
        {
            BackgroundColor = UIColor.Clear;

            _monthView = createMonthView(DateTime.Now);
            _scollView = new UIScrollView(new RectangleF(0, TITLE_HEIGHT, MONTHVIEW_WIDTH, _monthView.Frame.Height));
            _scollView.BackgroundColor = UIColor.Clear;
            Add(_scollView);
            _scollView.Add(_monthView);

            buildButtons();
            buildTitleLabel();            
            buildDayLabels();
            
            _titleLabel.Text = _monthView.Month.ToString("y");
            updateDayLabels();
        }
        #endregion

        #region private methods
        void buildButtons()
        {            
            _rightButton = new UIButton(new RectangleF(MONTHVIEW_WIDTH - 56, 0, 44, 42));
            _rightButton.SetImage(UIImage.FromFile("image/rightarrow.png"), UIControlState.Normal);
            _rightButton.TouchUpInside += delegate { moveToNextMonth(); };
            AddSubview(_rightButton);

            _leftButton = new UIButton(new RectangleF(10, 0, 44, 42));
            _leftButton.SetImage(UIImage.FromFile("image/leftarrow.png"), UIControlState.Normal);
            _leftButton.TouchUpInside += delegate { moveToPrevMonth(); };
            AddSubview(_leftButton);
        }
        void buildTitleLabel()
        {
            // title text            
            _titleLabel = new UILabel(new RectangleF(54, 10, MONTHVIEW_WIDTH - 54 * 2, 20));
            _titleLabel.Font      = UIFont.BoldSystemFontOfSize(20);
            _titleLabel.TextColor = UIColor.DarkGray;
            _titleLabel.Text      = String.Empty;
            _titleLabel.TextAlignment = UITextAlignment.Center;
            _titleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            _titleLabel.BackgroundColor = UIColor.Clear;
            Add(_titleLabel);
        }
        void buildDayLabels()
        {
            _daysLabel = new UILabel[7];            
            for (int i = 0; i < 7; i++)
            {
                var label = new UILabel(new RectangleF(i * DAYVIEW_WIDTH, TITLE_HEIGHT - 12, DAYVIEW_WIDTH, 10));
                label.Font = UIFont.SystemFontOfSize(10);
                label.Text = String.Empty;                
                label.TextColor       = UIColor.DarkGray;
                label.TextAlignment   = UITextAlignment.Center;
                label.LineBreakMode   = UILineBreakMode.WordWrap;
                label.BackgroundColor = UIColor.Clear;
                _daysLabel[i] = label;
                Add(label);                
            }            
        }
        CalendarMonthView createMonthView(DateTime month)
        {
            var mv = new CalendarMonthView(new RectangleF(0, 0, MONTHVIEW_WIDTH, DAYVIEW_HEIGHT * 6));
            mv.Month = month;            
            mv.DaySelected +=new EventHandler(_DaySelected);
            return mv;     
        }
        void updateDayLabels() 
        {                                    
            for (int i = 0; i < 7; i++)
                _daysLabel[i].Text = DayLabels[((int)_monthView.FirstDayOfWeek + i) % 7];
        }
        void updateFrame()
        {
            _scollView.Frame = new RectangleF(_scollView.Frame.Location, new SizeF( MONTHVIEW_WIDTH, _monthView.Frame.Height));
            Frame = new RectangleF(Frame.Location, new SizeF(MONTHVIEW_WIDTH, _scollView.Frame.Height + TITLE_HEIGHT));
        }
        void drawTitle()
        {
            var img = UIImage.FromFile("image/topbar.png");
            img.Draw(new PointF(0, 0));
        }
        void scrollCalendar(float distance)
        {
            UserInteractionEnabled = false;

            // animation
            UIView.BeginAnimations("month view showing");
            UIView.SetAnimationDuration(ANIMATION_DURATION);
            UIView.SetAnimationDelay(ANIMATION_DELAY);
            UIView.SetAnimationDelegate(this);
            UIView.SetAnimationDidStopSelector(new Selector(_ScrollAnimationStoppedHandler));
            
            _titleLabel.Text     = _nextMonthView.Month.ToString("y");
            _monthView.Frame     = new RectangleF(_monthView.Frame.X, _monthView.Frame.Y + distance, _monthView.Frame.Width, _monthView.Frame.Height);
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X, _nextMonthView.Frame.Y + distance, _nextMonthView.Frame.Width, _nextMonthView.Frame.Height);            
            _scollView.Frame     = new RectangleF(_scollView.Frame.Location, new SizeF(MONTHVIEW_WIDTH, _monthView.Frame.Height));
                       
            UIView.CommitAnimations();

            Frame = new RectangleF(Frame.Location, new SizeF(MONTHVIEW_WIDTH, _scollView.Frame.Height + TITLE_HEIGHT));
            SetNeedsDisplay();                        
            //BringSubviewToFront(monthView);
            //Frame = new RectangleF(Frame.Location, new SizeF(MONTHVIEW_WIDTH, monthView.Frame.Height + TITLE_HEIGHT));
           
            Debug.WriteLine("\tCalendarView: Frame after monthview updated: {0}.", Frame);
        }
        void moveToNextMonth()
        {
            var prevMonth = _monthView.Month;

            _nextMonthView = createMonthView(_monthView.Month.AddMonths(1));                        
            _nextMonthView.FirstDayOfWeek = _monthView.FirstDayOfWeek;
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X , _nextMonthView.Frame.Y + _monthView.Frame.Height, _nextMonthView.Frame.Width, _nextMonthView.Frame.Height);
            _scollView.Add(_nextMonthView);            
            
            scrollCalendar(-1 * _monthView.Frame.Height);

            invokeVisibleMonthChanged(_monthView.Month, prevMonth);            
        }        
        void moveToPrevMonth()
        {
            var prevMonth = _monthView.Month;

            _nextMonthView = createMonthView(_monthView.Month.AddMonths(-1));
            _nextMonthView.FirstDayOfWeek = _monthView.FirstDayOfWeek;
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X, _nextMonthView.Frame.Y - _monthView.Frame.Height , _nextMonthView.Frame.Width, _nextMonthView.Frame.Height); 
            _scollView.Add(_nextMonthView);            

            scrollCalendar(_monthView.Frame.Height);

            invokeVisibleMonthChanged(_monthView.Month, prevMonth);            
        }
        
        void invokeVisibleMonthChanged(DateTime curDate, DateTime prevDate)
        { 
            if(null != VisibleMonthChanged)
                VisibleMonthChanged.Invoke(this, new MonthChangedEventArgs(curDate, prevDate));
        }
        [Export(_ScrollAnimationStoppedHandler)]
        void _AnimationStopped()
        {
            _monthView.RemoveFromSuperview();
            _monthView = _nextMonthView;
            _nextMonthView = null;

            UserInteractionEnabled = true;
        }        
        void _DaySelected(object sender, EventArgs e)
        {
            var dayView = sender as CalendarDayView;
            if ( dayView.Day.Month != _monthView.Month.Month)
            {
                if (dayView.Day > _monthView.Month)
                    moveToNextMonth();
                else
                    moveToPrevMonth();
            }
            else
            {
                if (null != DaySelected)
                    DaySelected.Invoke(sender, e);
            }
        }
        #endregion
               
        #region override methods
        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
            drawTitle();            
        }
        #endregion
    }	
}