using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;

namespace com.ReinforceLab.MonoTouch.Controls.Calendar
{
    public interface IViewFactory
    {
        CacheableView Create(String cell_id); 
    }
}
