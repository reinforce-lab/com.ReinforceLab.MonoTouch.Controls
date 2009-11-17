using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar.Standard
{
    class Resources
    {
        public const float MONTHVIEW_WIDTH = 320f;
        public const float DAYVIEW_WIDTH = 46f;
        public const float DAYVIEW_HEIGHT = 45f;

        public const float TITLE_HEIGHT = 44f;

        public const String DateCellImage = "Images/datecell.png";
        public const String DateCellSelectedImage = "Images/datecellselected.png";
        public const String TodayImage = "Images/today.png";
        public const String TodaySelectedImage = "Images/todayselected.png";
        public const String MonthCell_320x270 = "Images/MonthCell_320x270.png";

        public const String RightArrorImage = "Images/rightarrow.png";
        public const String LeftArrorImage  = "Images/leftarrow.png";

        public const String TopBarImage = "Images/topbar.png";

        public static double ANIMATION_DURATION = 0.3;
        public static double ANIMATION_DELAY = 0.1;

        public static String BuildCellID(bool isActive, bool isSelected, bool isToday)
        {
            return String.Format("{0}@{1}@{2}@{3}", typeof(DayView).Name, isActive, isSelected, isToday);
        }
        public static bool GetIsActive(String cell_id)
        {
            if (String.IsNullOrEmpty(cell_id)) return false;
            var list = cell_id.Split('@');
            return Boolean.Parse(list[1]);
        }
        public static bool GetIsSelected(String cell_id)
        {
            if (String.IsNullOrEmpty(cell_id)) return false;
            var list = cell_id.Split('@');
            return Boolean.Parse(list[2]);
        }
        public static bool GetIsToday(String cell_id)
        {
            if (String.IsNullOrEmpty(cell_id)) return false;

            var list = cell_id.Split('@');
            return Boolean.Parse(list[3]);
        }
    }
}