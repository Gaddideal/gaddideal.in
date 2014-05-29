using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Common
{

    public struct SqlParameter
    {
        public DbType type { get; set; }
        public String name { get; set; }
        public object value { get; set; }
    }
    public class ConcreteDbContext : AbstractDbContext
    {
        public ConcreteDbContext(string ConnectionString)
            : base(ConnectionString)
        {

        }
    }
    public abstract class AbstractDbContext : System.Data.Entity.DbContext
    {
        private Database _db;
        protected Database db
        {
            get
            {
                if (_db == null)
                {
                    _db = new Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase(this.Database.Connection.ConnectionString);

                }
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        public AbstractDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }



        public int ExecuteNonQuery(string strQuery)
        {
            return DBTools.ExecuteNonQuery(db, strQuery);
        }
        public int ExecuteInt(string strQuery)
        {
            return DBTools.ExecuteInt(db, strQuery);
        }
        public string ExecuteString(string strQuery)
        {
            return DBTools.ExecuteString(db, strQuery);
        }
        public DataTable ExecuteDataTable(string strQuery, String TableName)
        {
            return DBTools.ExecuteDataTable(db, strQuery, TableName);
        }
        public DataTable ExecuteDataTable(string strQuery)
        {
            return DBTools.ExecuteDataTable(db, strQuery);
        }
        public DataSet ExecuteDataSet(string strQuery)
        {
            return DBTools.ExecuteDataSet(db, strQuery);
        }

        public int ExecuteNonQuery(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteNonQuery(db, strQuery, Parameters);
        }
        public int ExecuteInt(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteInt(db, strQuery, Parameters);
        }
        public string ExecuteString(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteString(db, strQuery, Parameters);
        }
        public DataTable ExecuteDataTable(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteDataTable(db, strQuery, Parameters);
        }
        public DataTable ExecuteDataTable(string strQuery, List<SqlParameter> Parameters, String TableName)
        {
            return DBTools.ExecuteDataTable(db, strQuery, Parameters, TableName);
        }
        public DataSet ExecuteDataSet(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteDataSet(db, strQuery, Parameters);
        }


        public int ExecuteIntWithParameter(string strQuery, string[] Parameter, string[] ParameterVal)
        {
            return DBTools.ExecuteIntWithParameter(db, strQuery, Parameter, ParameterVal);
        }

        public int ExecuteNonQueryWithParameter(string strQuery, string[] Parameter, string[] ParameterVal)
        {
            return DBTools.ExecuteNonQueryWithParameter(db, strQuery, Parameter, ParameterVal);
        }


    }
    public class SqlExecuter
    {
        String ConnectionString = "";
        public Database db { get; private set; }

        public SqlExecuter(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            this.db = new Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase(this.ConnectionString); ;
        }



        public int ExecuteNonQuery(string strQuery)
        {
            return DBTools.ExecuteNonQuery(db, strQuery);
        }
        public int ExecuteInt(string strQuery)
        {
            return DBTools.ExecuteInt(db, strQuery);
        }
        public string ExecuteString(string strQuery)
        {
            return DBTools.ExecuteString(db, strQuery);
        }
        public DataTable ExecuteDataTable(string strQuery, String TableName)
        {
            return DBTools.ExecuteDataTable(db, strQuery, TableName);
        }
        public DataTable ExecuteDataTable(string strQuery)
        {
            return DBTools.ExecuteDataTable(db, strQuery);
        }
        public DataSet ExecuteDataSet(string strQuery)
        {
            return DBTools.ExecuteDataSet(db, strQuery);
        }

        public int ExecuteNonQuery(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteNonQuery(db, strQuery, Parameters);
        }
        public int ExecuteInt(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteInt(db, strQuery, Parameters);
        }
        public string ExecuteString(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteString(db, strQuery, Parameters);
        }
        public DataTable ExecuteDataTable(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteDataTable(db, strQuery, Parameters);
        }
        public DataTable ExecuteDataTable(string strQuery, List<SqlParameter> Parameters, String TableName)
        {
            return DBTools.ExecuteDataTable(db, strQuery, Parameters, TableName);
        }
        public DataSet ExecuteDataSet(string strQuery, List<SqlParameter> Parameters)
        {
            return DBTools.ExecuteDataSet(db, strQuery, Parameters);
        }


        public int ExecuteIntWithParameter(string strQuery, string[] Parameter, string[] ParameterVal)
        {
            return DBTools.ExecuteIntWithParameter(db, strQuery, Parameter, ParameterVal);
        }
        public int ExecuteNonQueryWithParameter(string strQuery, string[] Parameter, string[] ParameterVal)
        {
            return DBTools.ExecuteNonQueryWithParameter(db, strQuery, Parameter, ParameterVal);
        }

        public List<T> Execute<T>(string sql, List<SqlParameter> Parameters)
        {
            DataTable dt = this.ExecuteDataTable(sql, Parameters);
            return ConvertTableToList<T>(dt);
        }
        public List<T> Execute<T>(string sql)
        {
            DataTable dt = this.ExecuteDataTable(sql);
            return ConvertTableToList<T>(dt);
        }
        List<T> ConvertTableToList<T>(DataTable table)
        {
            List<T> list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                list.Add((T)ConvertRowToObject<T>(row));
            }

            return list;
        }
        object ConvertRowToObject<T>(DataRow row)
        {
            object obj = Activator.CreateInstance(typeof(T));

            PropertyInfo[] infos = obj.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {
                //Type type = info.PropertyType;
                Type ColumnType = row[info.Name].GetType();
                Type DBNullType = typeof(DBNull);

                if (!ColumnType.Equals(DBNullType))
                {
                    info.SetValue(obj, row[info.Name], null);
                }
            }

            return obj;
        }

    }
    public class DBTools
    {
        public static string ValidString(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                text = "";
            }

            return text.Replace("'", "''");
        }
        public static string FormatString(string text, bool useWildcard = false)
        {
            if (useWildcard)
            {
                return "'%" + ValidString(text) + "%'";
            }
            else
            {
                return "'" + ValidString(text) + "'";
            }
        }
        public static string FormatDate(DateTime date)
        {

            string strDateFormat = "yyyy-MM-dd HH:mm:ss";
            return "'" + date.ToString(strDateFormat) + "'";
        }


        public static int ExecuteNonQuery(Database dbc, string strQuery)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            cmd.CommandTimeout = int.MaxValue;
            return dbc.ExecuteNonQuery(cmd);
        }
        public static int ExecuteInt(Database dbc, string strQuery)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            return int.Parse(dbc.ExecuteScalar(cmd).ToString());
        }
        public static string ExecuteString(Database dbc, string strQuery)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            return dbc.ExecuteScalar(cmd).ToString();
        }
        public static object ExecuteScalar(Database dbc, string strQuery)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            return dbc.ExecuteScalar(cmd);
        }
        public static DataTable ExecuteDataTable(Database dbc, string strQuery)
        {
            return ExecuteDataTable(dbc, strQuery, "");
        }
        public static DataTable ExecuteDataTable(Database dbc, string strQuery, string TableName)
        {
            DataTable obj = new DataTable();
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            DataSet objds = dbc.ExecuteDataSet(cmd);

            if (objds.Tables.Count > 0)
                obj = objds.Tables[0].Copy();

            if (!String.IsNullOrWhiteSpace(TableName))
            {
                obj.TableName = TableName;
            }

            return obj;
        }
        public static DataSet ExecuteDataSet(Database dbc, string strQuery)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            return dbc.ExecuteDataSet(cmd);
        }

        public static int ExecuteNonQuery(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            Parameters.ForEach(delegate(SqlParameter param) { dbc.AddInParameter(cmd, param.name, param.type, param.value); });
            cmd.CommandTimeout = int.MaxValue;
            return dbc.ExecuteNonQuery(cmd);
        }
        public static int ExecuteInt(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            Parameters.ForEach(delegate(SqlParameter param) { dbc.AddInParameter(cmd, param.name, param.type, param.value); });
            return int.Parse(dbc.ExecuteScalar(cmd).ToString());
        }
        public static string ExecuteString(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            Parameters.ForEach(delegate(SqlParameter param) { dbc.AddInParameter(cmd, param.name, param.type, param.value); });
            return dbc.ExecuteScalar(cmd).ToString();
        }
        public static object ExecuteScalar(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            Parameters.ForEach(delegate(SqlParameter param) { dbc.AddInParameter(cmd, param.name, param.type, param.value); });
            return dbc.ExecuteScalar(cmd);
        }
        public static DataTable ExecuteDataTable(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            return ExecuteDataTable(dbc, strQuery, Parameters, null);
        }
        public static DataTable ExecuteDataTable(Database dbc, string strQuery, List<SqlParameter> Parameters, string TableName)
        {
            DataTable obj = new DataTable();
            DataSet objds = ExecuteDataSet(dbc, strQuery, Parameters);

            if (objds.Tables.Count > 0)
                obj = objds.Tables[0].Copy();

            if (!String.IsNullOrWhiteSpace(TableName))
            {
                obj.TableName = TableName;
            }

            return obj;
        }
        public static DataSet ExecuteDataSet(Database dbc, string strQuery, List<SqlParameter> Parameters)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);
            Parameters.ForEach(delegate(SqlParameter param) { dbc.AddInParameter(cmd, param.name, param.type, param.value); });
            return dbc.ExecuteDataSet(cmd);
        }

        public static int ExecuteNonQueryWithParameter(Database dbc, string strQuery, string[] Parameter, string[] ParameterVal)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);

            int i = 0;
            foreach (string p in Parameter)
            {
                dbc.AddInParameter(cmd, p, DbType.String, ParameterVal[i]);
                i++;
            }

            cmd.CommandTimeout = int.MaxValue;
            return dbc.ExecuteNonQuery(cmd);
        }
        public static int ExecuteIntWithParameter(Database dbc, string strQuery, string[] Parameter, string[] ParameterVal)
        {
            DbCommand cmd = dbc.GetSqlStringCommand(strQuery);

            int i = 0;
            foreach (string p in Parameter)
            {
                dbc.AddInParameter(cmd, p, DbType.String, ParameterVal[i]);
                i++;
            }

            return int.Parse(dbc.ExecuteScalar(cmd).ToString());
        }

        public static T RetrieveDRValue<T>(DataRow dr, string columnName)
        {
            try
            {
                return (T)Convert.ChangeType(dr[columnName].ToString(), typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        public static T RetrieveJObjectValue<T>(JObject jobj, string keyname)
        {
            try
            {
                return (T)Convert.ChangeType(jobj[keyname].ToString(), typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }


}



