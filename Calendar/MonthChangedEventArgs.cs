using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.MonoTouch.Controls.Calendar
{
    public delegate void MonthChangedEventHandler(Object sender, MonthChangedEventArgs e);

    public class MonthChangedEventArgs : EventArgs
    {
        #region Properties
        public DateTime NewDate      { get; private set; }
        public DateTime PreviousDate {get; private set;}
        #endregion

        #region Constructor
        public MonthChangedEventArgs(DateTime newDate, DateTime privoiousDate) : base()
        {
            NewDate      = newDate;
            PreviousDate = privoiousDate;
        }
        #endregion
    }
}
