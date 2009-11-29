using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.SQLitePersistence
{
    public class ColumnConstraintAttribute : Attribute
    {
        public String Constraint { get; private set; }

        public ColumnConstraintAttribute(String constraint)
        {
            Constraint = constraint;
        }
    }
}
