using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
	public class CalendarDayView : UIView
	{
        #region Variables
        UILabel _label;
        //public event EventHandler Clicked;
        #endregion
		
		#region Properties
		DateTime _day;
		public DateTime Day {
			get {return _day;}
            set { 
                _day = value;            
                _label.Text = _day.Day.ToString();
            }
		}
				
		public String Text {
			get{return _label.Text;}
			set {_label.Text = value;}
		}
		
		bool _isSelected;
		public bool IsSelected {
			get {return _isSelected;}
			set
			{
				if(value != _isSelected )
				{
					_isSelected = value;
					SetNeedsDisplay();
				}
			}
		}
		
		bool _isActive;
		public bool IsActive
		{
			get{ return _isActive;}
			set
			{
				if(value != _isActive )
				{
					_isActive = value;
					SetNeedsDisplay();
				}
			}
		}
		
		bool _isToday;	
		public bool IsToday
		{
			get{return _isToday;}
			set
			{
				if(value != _isToday)
				{
					_isToday = value;
					SetNeedsDisplay();
				}
			}
		}
		
		bool _isMarked;
		public bool IsMarked 
		{
			get {return _isMarked;}
			set
			{
				if(value != _isMarked)
				{
					_isMarked = value;
					SetNeedsDisplay();
				}
			}
		}
		#endregion
		
		#region Constructors
		public CalendarDayView (RectangleF rect) : base(rect)
		{
            initialize();
		}
		void initialize ()
		{
            BackgroundColor = UIColor.Clear;
                        
			_isActive   = true;
			_isToday    = false;
			_isMarked   = false;
			_isSelected = false;

            Opaque = true;
            UserInteractionEnabled = false;

            _label = new UILabel(new RectangleF( 9, 6, Bounds.Width - 18, 22));
            _label.Text = String.Empty;
            _label.Font = UIFont.BoldSystemFontOfSize(22);
            _label.TextColor = UIColor.Gray;
            _label.TextAlignment = UITextAlignment.Center;            
            _label.BackgroundColor = UIColor.Clear;
            _label.UserInteractionEnabled = false;
            Add(_label);
		}		
		#endregion
		
		#region Private methods
        /*
		void invokeClickedEvent()
		{
            Debug.WriteLine("\tCalendarDayView: ClickedEvent {0}.", Day.ToShortDateString());
			if(null != Clicked)
				Clicked.Invoke(this, null);
		}
         * */
        void drawBackGroundColorAndImage()
        {            
            UIImage img;			

            if (!_isActive)
            {
                _label.TextColor = UIColor.Gray;
                img = UIImage.FromFile("image/datecell.png");
            }
            else if (_isToday && _isSelected)
            {
                _label.TextColor = UIColor.White;
                img = UIImage.FromFile("image/todayselected.png");
            }
            else if (_isToday)
            {
                _label.TextColor = UIColor.White;
                img = UIImage.FromFile("image/today.png");
            }
            else if (_isSelected)
            {
                _label.TextColor = UIColor.White;
                img = UIImage.FromFile("image/datecellselected.png");
            }
            else
            {
                _label.TextColor = UIColor.FromRGB(75, 92, 111);
                img = UIImage.FromFile("image/datecell.png");
            }
            img.Draw(new PointF(0, 0));            
        }
        void drawMarker()
        {                    
            if (_isMarked)
            {
                Debug.WriteLine("\tCalendarDayView: Drawing a marker.");
                var context = UIGraphics.GetCurrentContext();
                if (_isSelected || _isToday)
                    context.SetRGBFillColor(1, 1, 1, 1);
                else
                    context.SetRGBFillColor(75 / 255, 92 / 255, 111 / 255, 1);

                context.SetLineWidth(0);
                context.AddEllipseInRect(new RectangleF(Bounds.Size.Width / 2 - 2, Bounds.Size.Height - 10, 4, 4));
                context.FillPath();
            }             
        }
		#endregion
		
		#region override method        
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
            //Debug.WriteLine("\tCalendarDayView: Draw() day:{0}.", _day);
            drawBackGroundColorAndImage();            
            drawMarker();
		}
        /*
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
            Debug.WriteLine("\tCalendarDayView: TouchesBegan day:{0}.", _day);
            //NextResponder.TouchesBegan(touches, evt);
			//invokeClickedEvent();
		}        
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
            Debug.WriteLine("\tCalendarDayView: TouchesMoved day:{0}.", _day);
            //NextResponder.TouchesMoved(touches, evt);
			//invokeClickedEvent();
		}
        */
		#endregion
	}
}
