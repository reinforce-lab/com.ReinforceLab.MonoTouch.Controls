using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Common;

namespace net.ReinforceLab.SQLitePersistence
{
    public abstract class AbsPersistenceManager
    {
        #region Variables
        readonly Dictionary<Type, PropertyInfo[]> _pinfo;
        protected readonly DbConnection _connection;

        protected int _DefaultStringLength = 140;        
        #endregion

        #region Constructor
        public AbsPersistenceManager(DbConnection connection)
        {
            _connection = connection;
            _pinfo = new Dictionary<Type, PropertyInfo[]>();
        }
        #endregion

        #region Abstract methods
        protected abstract string SqlDecl(PropertyInfo pinfo);
        protected abstract string SqlType(PropertyInfo pinfo);

        protected abstract String GetCreateTebleCommandText<T>();
        protected abstract String GetInsertCommandText<T>();
        protected abstract String GetUpdateCommandText<T>();
        protected abstract String GetDeleteCommandText<T>();
        protected abstract DbCommand BuildCommand<T>(String commandText,T source);
        protected abstract DbCommand GetLastRowIDCommand();
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
        protected bool isAutoIncPrimaryKey(PropertyInfo pinfo)
        {
            return isAutoInc(pinfo) && isPrimaryKey(pinfo);
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

        #region Private methods 
        int ExecuteNonQuery<T>(String cmdtxt, T obj, bool setAutoIncPrimaryKey)
        {
            var cmd = BuildCommand<T>(cmdtxt, obj);

            _connection.Open();
            int cnt = cmd.ExecuteNonQuery();
            if (setAutoIncPrimaryKey)
            {
                // set last inserted row id
                var pkinfo = getPrimaryKey(typeof(T));
                if (isAutoInc(pkinfo))
                {
                    cmd = GetLastRowIDCommand();
                    var rowid = cmd.ExecuteScalar();
                    pkinfo.SetValue(obj, Convert.ChangeType(rowid, pkinfo.PropertyType), null);
                }
            }
            _connection.Close();

            return cnt;
        }

        T Read<T>(DbDataReader reader) where T : new()
        {            
            T inst = Activator.CreateInstance<T>();
            foreach (var pinfo in getColumns(typeof(T)))
            {                       
                if (pinfo.PropertyType.IsEnum)                    
                    pinfo.SetValue(inst, System.Enum.Parse(pinfo.PropertyType, reader[pinfo.Name].ToString() ), null);
                else
                    pinfo.SetValue(inst, Convert.ChangeType(reader[pinfo.Name], pinfo.PropertyType), null);
            }            
            return inst;
        }
        #endregion

        #region Public methods
        public IEnumerable<T> Read<T>(String select_cmdtxt, T template) where T : IStorable, new()
        {            
            var rows = new List<T>();
            // FIXME maximum row length should be set here.
            _connection.Open();
            using (var cmd = BuildCommand<T>(select_cmdtxt, template))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var row = Read<T>(rdr);
                        (row as IStorable).State = DataRowState.Unchanged;
                        rows.Add(row);
                    }
                }
            }
            _connection.Close();

            return rows;
        }
        public int AcceptChanges<T>(IEnumerable<T> objs) where T : IStorable
        {
            var updateItems = from item in objs where item.State == DataRowState.Modified select item;
            var insertItems = from item in objs where item.State == DataRowState.Added    select item;
            var deteteItems = from item in objs where item.State == DataRowState.Deleted  select item;

            int cnt = 0;
            var type = typeof(T);
            var pkinfo = getPrimaryKey(type);
            bool isAutoIncPrimaryKey = isAutoInc(pkinfo);

            _connection.Open();
            using (var tran = _connection.BeginTransaction())
            {
                // insert
                var insertCmdTxt = GetInsertCommandText<T>();
                foreach (var obj in insertItems)
                {
                    var cmd  = BuildCommand<T>(insertCmdTxt, obj);
                    var rows = cmd.ExecuteNonQuery();
                    cnt += rows;
                    // set last inserted row id
                    if (isAutoIncPrimaryKey)
                    {
                        cmd = GetLastRowIDCommand();
                        int rowid = cmd.ExecuteNonQuery();                        

                        pkinfo.SetValue(obj, Convert.ChangeType(rowid, pkinfo.PropertyType), null);
                    }
                    (obj as IStorable).State = DataRowState.Unchanged;
                }
                // update
                var updateCmdTxt = GetUpdateCommandText<T>();
                foreach (var obj in updateItems)
                {
                    var cmd = BuildCommand<T>(updateCmdTxt, obj);
                    var rows = cmd.ExecuteNonQuery();
                    cnt += rows;
                    (obj as IStorable).State = DataRowState.Unchanged;
                }
                // delete
                var deleteCmdTxt = GetDeleteCommandText<T>();
                foreach (var obj in updateItems)
                {
                    var cmd = BuildCommand<T>(deleteCmdTxt, obj);
                    var rows = cmd.ExecuteNonQuery();
                    cnt += rows;
                    (obj as IStorable).State = DataRowState.Deleted;
                }

                tran.Commit();
            }
            _connection.Close();

            return cnt;
        }
        public void CreateTeble<T>()
        {
            var command         = _connection.CreateCommand();
            command.CommandText = GetCreateTebleCommandText<T>();
            
            _connection.Open();
            command.ExecuteNonQuery();
            _connection.Close();
        }
        public int Insert<T>(T obj) where T : IStorable
        {            
            int cnt = ExecuteNonQuery<T>(GetInsertCommandText<T>(), obj, true);

            (obj as IStorable).State = DataRowState.Unchanged;

            return cnt;
        }
        public int Update<T>(T obj) where T : IStorable
        {
            int cnt = ExecuteNonQuery<T>(GetUpdateCommandText<T>(), obj, false);
            
            (obj as IStorable).State = DataRowState.Unchanged;

            return cnt;
        }
        public int Delete<T>(T obj)
        {
            int cnt = ExecuteNonQuery<T>(GetDeleteCommandText<T>(), obj, false);
            
            (obj as IStorable).State = DataRowState.Deleted;
            
            return cnt;
        }
        #endregion       
    }
}
