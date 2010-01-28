using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace com.ReinforceLab.SQLitePersistence.Test
{
    public class PrimitiveValuesDAO : AbsStorable, IPrimitiveValues
    {
        #region IPrimitiveValues メンバ
        public int ID { get; set; }
        public byte ByteValue {get; set;}
        public string StringValue {get; set;}
        public DateTime DateTimeValue {get; set;}
        public int IntValue {get; set;}
        public double DoubleValue {get; set;}
        public ArbEnumType ArbVal {get; set;}
        #endregion

        #region IStorable メンバ
        public DataRowState State {get; set;}
        #endregion

        public PrimitiveValuesDAO()
            : base()
        { }
    }
}
