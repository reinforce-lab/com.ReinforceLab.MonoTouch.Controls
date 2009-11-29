using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

#if SD_SQLITE
using System.Data.SQLite;
#else
using Mono.Data.Sqlite;
#endif

namespace net.ReinforceLab.SQLitePersistence
{
    public class SQLitePersistenceManager : AbsPersistenceManager
    {        
        #region Variables
        const String _SqlTypeInteger = "integer";
        const String _SqlTypeBigInt = "bigint";
        const String _SqlTypeFloat = "float";
        const String _SqlTypeVarChar = "varchar";
        const String _SqlTypeDateTime = "datetime";
            
        readonly Dictionary<Type, String> _sqlType;
        #endregion

        #region Properties
        public SqliteConnection Connection { get; private set; }
        #endregion

        #region Constructor
        public SQLitePersistenceManager(SqliteConnection connection)
            : base()
        {
            Connection = connection;         
   
            _sqlType = new Dictionary<Type, string>();

            _sqlType[typeof(System.Boolean)] = _SqlTypeInteger;
            _sqlType[typeof(System.Char)] = _SqlTypeInteger;

            _sqlType[typeof(System.Byte)]  = _SqlTypeInteger;
            _sqlType[typeof(System.SByte)] = _SqlTypeInteger;
            _sqlType[typeof(System.UInt16)] = _SqlTypeInteger;
            _sqlType[typeof(System.Int16)] = _SqlTypeInteger;
            _sqlType[typeof(System.Int32)] = _SqlTypeInteger;

            _sqlType[typeof(System.Single)]  = _SqlTypeFloat;
            _sqlType[typeof(System.Double)]  = _SqlTypeFloat;
            _sqlType[typeof(System.Decimal)] = _SqlTypeFloat;

            _sqlType[typeof(System.String)] = _SqlTypeVarChar;

            _sqlType[typeof(System.DateTime)] = _SqlTypeDateTime;    
        }
        #endregion

        #region Private methods
       SqliteCommand buildCommand(String commandText, Type type)
        {
            var command = new SqliteCommand(Connection);
            command.CommandText = commandText;
            foreach (var pinfo in getColumns(type))
            {
                var p = command.CreateParameter();
                p.ParameterName = "@" + pinfo.Name;
                p.SourceColumn  = pinfo.Name;
                command.Parameters.Add(p);
            }

            return command;
        }
        #endregion

        #region Protected methods
        protected override string SqlDecl(System.Reflection.PropertyInfo pinfo)
        {
            var decl = String.Format("'{0}' {1} ", pinfo.Name, SqlType(pinfo));
                     
            if (isPrimaryKey(pinfo))                            
                decl += "PRIMARY KEY ";                        
            if (isAutoInc(pinfo))                            
                decl += "AUTOINCREMENT ";                        
            /*
            if (!isNullable(pinfo))
                            decl += "NOT NULL ";                       
             * */
            return decl; 
        }
        protected override string SqlType(PropertyInfo pinfo)
        {
            var type = pinfo.PropertyType;

            if (_sqlType.ContainsKey(type))
                return _sqlType[type];

            if (type.IsEnum)
                return _SqlTypeInteger;

            if(type == typeof(System.String))
            {
                int len = getMaxStringLength(pinfo);
                return String.Format("{0}({1})", _sqlType[type], len);
            } 
                    
            throw new NotSupportedException("Not supported value type: " + type.Name);            
        }
        #endregion

        #region Public methods
        public void CreateTable<T>()
        {
            var command = Connection.CreateCommand();
            command.CommandText = GetCreateTebleCommandText(typeof(T));
            command.ExecuteNonQuery();
        }

        public SqliteDataAdapter GetAdapter<T>(string commandText)
        {
            Type type = typeof(T);
            var adapter = new SqliteDataAdapter(commandText, Connection);
            adapter.InsertCommand = buildCommand(GetInsertCommandText(type), type);
            adapter.UpdateCommand = buildCommand(GetUpdateCommandText(type), type);
            adapter.DeleteCommand = buildCommand(GetDeleteCommandText(type), type);            

            return adapter;
        }

        public override string GetCreateTebleCommandText(Type type)
        {
            return 
            String.Format("CREATE TABLE IF NOT EXISTS '{0}'({1})",
                type.Name,
                String.Join(",", getColumns(type).Select(p => SqlDecl(p)).ToArray()));                
        }
        public override string GetInsertCommandText(Type type)
        {
            var cmd = 
            String.Format("INSERT INTO {0}({1}) VALUES ({2})",                         
                type.Name,                         
                String.Join(",", getNotPrimaryKeyPropertyInfo(type).Select(p => p.Name).ToArray()),                         
                String.Join(",", getNotPrimaryKeyPropertyInfo(type).Select(p => "@" + p.Name).ToArray()));
            return cmd;
        }
        public override string GetUpdateCommandText(Type type)
        {
            var cmd = 
            String.Format("UPDATE {0} SET {1} WHERE {2}",            
                type.Name,
                String.Join(",", getNotPrimaryKeyPropertyInfo(type).Select(p => String.Format(" {0} = @{0}", p.Name)).ToArray()), 
                String.Format("{0} = @{0}", getPrimaryKeyPropertyInfo(type).Name));
            return cmd;
        }
        public override string GetDeleteCommandText(Type type)
        {
            var cmd =
            String.Format("DELETE FROM {0} WHERE {1}",
                type.Name,                            
                String.Format("{0} = @{0}", getPrimaryKeyPropertyInfo(type).Name));
            return cmd;
        }        
        #endregion
    }
}
