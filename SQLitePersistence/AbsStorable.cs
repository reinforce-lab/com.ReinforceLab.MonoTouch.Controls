using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ReinforceLab.SQLitePersistence
{
    public abstract class AbsStorable : IStorable
    {
        #region IStorable メンバ
        public System.Data.DataRowState State {get;set;}
        #endregion

        #region Constructor
        public AbsStorable()
        {
            State = System.Data.DataRowState.Added;
        }
        #endregion
    }
}
