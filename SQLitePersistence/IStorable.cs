﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net.ReinforceLab.SQLitePersistence
{
    public interface IStorable
    {
        [Ignore]
        DataRowState State { get; set; }
    }
}
