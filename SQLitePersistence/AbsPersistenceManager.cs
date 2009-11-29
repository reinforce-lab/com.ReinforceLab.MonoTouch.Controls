using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace net.ReinforceLab.SQLitePersistence
{
    public abstract class AbsPersistenceManager
    {
        #region Variables
        readonly Dictionary<Type, PropertyInfo[]> _pinfo;

        protected int _DefaultStringLength = 140;        
        #endregion

        #region Constructor
        public AbsPersistenceManager()
        {
            _pinfo = new Dictionary<Type, PropertyInfo[]>();
        }
        #endregion

        #region Abstract methods
        protected abstract string SqlDecl(PropertyInfo pinfo);
        protected abstract string SqlType(PropertyInfo pinfo);
        #endregion

        #region Protected methods
        protected int getMaxStringLength(PropertyInfo pinfo)
        {
            var attrs = pinfo.GetCustomAttributes(typeof(MaxLengthAttribute), true);
            if (attrs.Length > 0)
                return ((MaxLengthAttribute)attrs[0]).Value;
            else
                return _DefaultStringLength;
        }
        protected PropertyInfo getPrimaryKeyPropertyInfo(Type type)
        {
            var query = from   p in getColumns(type)
                        where  isPrimaryKey(p)
                        select p;
            return query.FirstOrDefault<PropertyInfo>();
        }
        protected IEnumerable<PropertyInfo> getNotPrimaryKeyPropertyInfo(Type type)
        {
            return  from p in getColumns(type)
                        where ! isPrimaryKey(p)
                        select p;            
        }
        protected bool isPrimaryKey(PropertyInfo pinfo)
        {
            var attrs = pinfo.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            return attrs.Length > 0;
        }
        protected bool isAutoInc(PropertyInfo pinfo)
        {
            var attrs = pinfo.GetCustomAttributes(typeof(AutoIncrementAttribute), true);
            return attrs.Length > 0;
        }
        protected bool isIgnored(PropertyInfo pinfo)
        {
            var attrs = pinfo.GetCustomAttributes(typeof(IgnoreAttribute), true);
            return attrs.Length > 0;
        }
        protected String getColumnConstrain(PropertyInfo pinfo)
        {
            var attrs = pinfo.GetCustomAttributes(typeof(ColumnConstraintAttribute), true);
            if (attrs.Length > 0)
                return ((ColumnConstraintAttribute)attrs[0]).Constraint;
            else
                return String.Empty;
        }
        protected IEnumerable<PropertyInfo> getColumns(Type type)
        {
            if (!_pinfo.ContainsKey(type))
            {
                var pinfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var query = from p in pinfo
                            where !isIgnored(p) && p.CanRead
                            select p;
                _pinfo[type] = query.ToArray();
            }

            return _pinfo[type];
        }
        protected PropertyInfo getPrimaryKey(Type type)
        {
            var props = getColumns(type);

            foreach (var p in props)
                if (isPrimaryKey(p)) return p;

            return null;
        }
        #endregion

        #region Public methods
        public abstract String GetCreateTebleCommandText(Type type);
        public abstract String GetInsertCommandText(Type type);
        public abstract String GetUpdateCommandText(Type type);
        public abstract String GetDeleteCommandText(Type type);
        #endregion       
    }
}
