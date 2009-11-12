using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
	[Register("CalendarView")]
    public class CalendarView : UIView
    {
        #region Variables
        public const float MONTHVIEW_WIDTH = 320f;
        public const float DAYVIEW_WIDTH = 46f;
        public const float DAYVIEW_HEIGHT = 45f;

        const String _ScrollAnimationStoppedHandler = "_ScrollAnimationStoppedHandler";
        readonly String[] DayLabels = new String[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        protected float TITLE_HEIGHT = 44f; 
        
        protected double ANIMATION_DURATION = 0.3;
        protected double ANIMATION_DELAY = 0.1;       

        protected UILabel   _titleLabel;
        protected UILabel[] _daysLabel;
        protected UIButton _leftButton, _rightButton;
        CalendarMonthView     _monthView, _nextMonthView;
        UIScrollView _scollView;
        #endregion

        #region Events
        public event EventHandler<DaySelectedEventArgs> DaySelected;
        public event MonthChangedEventHandler VisibleMonthChanged;        
        #endregion

        #region Properties        
        public CalendarMonthView MonthView { get { return _monthView; } }
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
		public CalendarView(IntPtr handle) : base(handle)
		{
			initialize();
		}
        public CalendarView(RectangleF rect) : base(rect)
        {
            initialize();
        }
        void initialize()
        {
            BackgroundColor = UIColor.Clear;            

            _monthView = CreateMonthView(DateTime.Now);
            _scollView = new UIScrollView(new RectangleF(0, TITLE_HEIGHT, MONTHVIEW_WIDTH, _monthView.Frame.Height));
            _scollView.BackgroundColor = UIColor.Clear;            
            _scollView.ShowsVerticalScrollIndicator = false;
            _scollView.ShowsHorizontalScrollIndicator = false;
            _scollView.ScrollEnabled = false;
            
            Add(_scollView);
            _scollView.Add(_monthView);

            buildButtons();
            buildTitleLabel();            
            buildDayLabels();
            
            _titleLabel.Text = _monthView.Month.ToString("y");
            updateDayLabels();
        }
        #endregion

        #region Protected methods
        protected virtual void buildButtons()
        {
            _rightButton = new UIButton(new RectangleF(MONTHVIEW_WIDTH - 56, 0, 44, 42));
            _rightButton.SetImage(UIImage.FromFile("Images/rightarrow.png"), UIControlState.Normal);
            _rightButton.TouchUpInside += delegate { moveToNextMonth(); };
            AddSubview(_rightButton);

            _leftButton = new UIButton(new RectangleF(10, 0, 44, 42));
            _leftButton.SetImage(UIImage.FromFile("Images/leftarrow.png"), UIControlState.Normal);
            _leftButton.TouchUpInside += delegate { moveToPrevMonth(); };
            AddSubview(_leftButton);
        }
        protected virtual void buildTitleLabel()
        {
            // title text            
            _titleLabel = new UILabel(new RectangleF(54, 10, MONTHVIEW_WIDTH - 54 * 2, 20));
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
                var label = new UILabel(new RectangleF(i * DAYVIEW_WIDTH, TITLE_HEIGHT - 12, DAYVIEW_WIDTH, 10));
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
            var img = UIImage.FromFile("Images/topbar.png");
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
            _scollView.Frame = new RectangleF(_scollView.Frame.Location, new SizeF( MONTHVIEW_WIDTH, _monthView.Frame.Height));
            Frame = new RectangleF(Frame.Location, new SizeF(MONTHVIEW_WIDTH, _scollView.Frame.Height + TITLE_HEIGHT));
        }                
        void scrollCalendar(float distance)
        {
            UserInteractionEnabled = false;

            Frame = new RectangleF(Frame.Location, new SizeF(MONTHVIEW_WIDTH, _nextMonthView.Frame.Height + TITLE_HEIGHT));

            // animation
            UIView.BeginAnimations("month view scrolling");
            UIView.SetAnimationDuration(ANIMATION_DURATION);
            UIView.SetAnimationDelay(ANIMATION_DELAY);
            UIView.SetAnimationDelegate(this);
            UIView.SetAnimationDidStopSelector(new Selector(_ScrollAnimationStoppedHandler));
            
            _titleLabel.Text     = _nextMonthView.Month.ToString("y");
            _monthView.Frame     = new RectangleF(_monthView.Frame.X, _monthView.Frame.Y + distance, _monthView.Frame.Width, _monthView.Frame.Height);
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X, _nextMonthView.Frame.Y + distance, _nextMonthView.Frame.Width, _nextMonthView.Frame.Height);            
            _scollView.Frame     = new RectangleF(_scollView.Frame.Location, new SizeF(MONTHVIEW_WIDTH, _nextMonthView.Frame.Height));            
           
            UIView.CommitAnimations();
            
            SetNeedsDisplay();                                   
            //Debug.WriteLine("\tCalendarView: Frame after monthview updated: {0}.", Frame);
        }
        void moveToNextMonth()
        {
            _nextMonthView = CreateMonthView(_monthView.Month.AddMonths(1));                        
            _nextMonthView.FirstDayOfWeek = _monthView.FirstDayOfWeek;
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X , _nextMonthView.Frame.Y + _monthView.Frame.Height, _nextMonthView.Frame.Width, _nextMonthView.Frame.Height);
            _scollView.Add(_nextMonthView);            
            
            scrollCalendar(-1 * _monthView.Frame.Height);            
        }        
        void moveToPrevMonth()
        {
            _nextMonthView = CreateMonthView(_monthView.Month.AddMonths(-1));
            _nextMonthView.FirstDayOfWeek = _monthView.FirstDayOfWeek;
            _nextMonthView.Frame = new RectangleF(_nextMonthView.Frame.X, _nextMonthView.Frame.Y - _nextMonthView.Frame.Height , _nextMonthView.Frame.Width, _nextMonthView.Frame.Height); 
            _scollView.Add(_nextMonthView);

            scrollCalendar(_nextMonthView.Frame.Height);            
        }
        
        void invokeVisibleMonthChanged(DateTime curDate, DateTime prevDate)
        { 
            if(null != VisibleMonthChanged)
                VisibleMonthChanged.Invoke(this, new MonthChangedEventArgs(curDate, prevDate));
        }
        
        [Export(_ScrollAnimationStoppedHandler)]
        void _AnimationStopped()
        {
            DateTime prevMonth = _monthView.Month;            

            _monthView.RemoveFromSuperview();
            _monthView = _nextMonthView;
            _nextMonthView = null;

            invokeVisibleMonthChanged(_monthView.Month, prevMonth);

            UserInteractionEnabled = true;
        }        
        
        #endregion

        #region Protected methods
        protected virtual CalendarMonthView CreateMonthView(DateTime month)
        {
            var mv   = new CalendarMonthView(new RectangleF(0, 0, MONTHVIEW_WIDTH, DAYVIEW_HEIGHT * 6));
            mv.Month = month;
            mv.DaySelected += new EventHandler<DaySelectedEventArgs>(_DaySelected);
            return mv;
        }

        protected void _DaySelected(object sender, DaySelectedEventArgs e)
        {
            if (null != DaySelected)
                DaySelected.Invoke(sender, e);
        }
        #endregion

        #region Public methods
        public void MoveToNextMonth()
        {
            moveToNextMonth();
        }
        public void MoveToPrevMonth()
        {
            moveToPrevMonth();
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
            drawTitle();
        }
        #endregion
    }	
}