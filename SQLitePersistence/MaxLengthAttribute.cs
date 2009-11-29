using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.SQLitePersistence
{
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; private set; }
        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }
}
