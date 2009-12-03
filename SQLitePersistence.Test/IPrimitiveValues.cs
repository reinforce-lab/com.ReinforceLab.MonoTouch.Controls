using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using net.ReinforceLab.iPhone.Utils.SQLitePersistence;

namespace net.ReinforceLab.SQLitePersistence.Test
{
    public enum ArbEnumType { One, Two, Three }

    public interface IPrimitiveValues : net.ReinforceLab.SQLitePersistence.IStorable
    {
        [PrimaryKey]
        [AutoIncrement]
        int ID { get; }

        Byte ByteValue { get; set; }
        String StringValue { get; set; }
        DateTime DateTimeValue { get; set; }
        System.Int32 IntValue { get; set; }
        Double DoubleValue { get; set; }        
        ArbEnumType ArbVal {get; set;}             
    }
}
