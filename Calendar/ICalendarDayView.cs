using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public interface ICalendarDayView
    {
        DateTime Day    {get; set;}        
        bool IsSelected {get; set;}
        bool IsActive   {get; set;}     
        bool IsToday    {get; set;}
        bool IsMarked   {get; set;}        
    }
}
