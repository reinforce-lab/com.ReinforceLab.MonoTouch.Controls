using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net.ReinforceLab.iPhone.Utils.SQLitePersistence.Test
{
    public class PrimitiveValuesDAO : IPrimitiveValues
    {
        #region IPrimitiveValues メンバ
        public int ID
        {
            get { return (int)_row["ID"]; }
        }
        public byte ByteValue
        {
            get{return Convert.ToByte(_row["ByteValue"]);}            
            set{_row["ByteValue"] = value;}
        }

        public string StringValue
        {
            get{return (String)_row["StringValue"];}            
            set{_row["StringValue"] = value;}
        }
        public DateTime DateTimeValue
        {
            get {return (DateTime)_row["DateTimeValue"];}
            set {_row["DateTimeValue"] = value;}
        }
        public System.Int32 IntValue
        {
            get {return Convert.ToInt32(_row["IntValue"]);}
            set {_row["IntValue"] = value;}
        }
        public double DoubleValue
        {
            get{return Convert.ToDouble(_row["DoubleValue"]);}
            set{{_row["DoubleValue"] = value;}}
        }
        public ArbEnumType ArbVal
        {
            get{return (ArbEnumType)Convert.ToInt32(_row["ArbVal"]);}
            set{_row["ArbVal"] = (int)value;}
        }
        #endregion

        #region Constructor
        readonly DataRow _row;
        public PrimitiveValuesDAO(DataRow row)
        {
            _row = row;
        }
        #endregion                 
    }
}
