using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.ReinforceLab.SQLitePersistence;

namespace com.ReinforceLab.SQLitePersistence.Test
{
    public enum ArbEnumType { One, Two, Three }

    public interface IPrimitiveValues : com.ReinforceLab.SQLitePersistence.IStorable
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
