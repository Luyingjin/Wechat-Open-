using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using Config.Logic;

namespace Config
{
    /// <summary>
    /// 数据库操作工具类
    /// </summary>
    public class SQLHelper
    {
        #region 扩展

        #region 静态构造方法

        public static SQLHelper CreateSqlHelper(ConnEnum connName)
        {
            return CreateSqlHelper(connName.ToString());
        }

        private static Dictionary<string, object> _dic = new Dictionary<string, object>();
        public static SQLHelper CreateSqlHelper(string connName)
        {
            string key = string.Format("Conn_{0}", connName);

            SQLHelper sqlHelper = null;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                    return HttpContext.Current.Items[key] as SQLHelper;

                sqlHelper = new SQLHelper(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
                HttpContext.Current.Items[key] = sqlHelper;
            }
            else
            {
                if (_dic.Keys.Contains(key))
                    return _dic[key] as SQLHelper;

                sqlHelper = new SQLHelper(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
                _dic[key] = sqlHelper;
            }

            return sqlHelper;
        }


        #endregion

        #region ExecuteNonQueryWithTrans

        /// <summary>
        /// 将cmdText在一个事务中执行
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public string ExecuteNonQueryWithTrans(string cmdText)
        {
            string retVal = "0";
            using (Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr))
            {
                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (Oracle.DataAccess.Client.OracleTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            cmd.Transaction = trans;
                            retVal = cmd.ExecuteNonQuery().ToString();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            LogWriter.Error(ex, cmdText);
                            trans.Rollback();
                            throw;
                        }
                    }


                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp, cmdText);
                    //retVal = exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        #endregion

        #region 执行并将结果转换为对象

        public T ExecuteObject<T>(string sql, T obj = null) where T : class ,new()
        {
            DataTable dt = this.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                return RowToObj<T>(dt.Rows[0], obj);
            return obj;
        }

        public List<T> ExecuteList<T>(string sql) where T : class,new()
        {
            List<T> list = new List<T>();

            DataTable dt = this.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                T obj = RowToObj<T>(row, null);
                list.Add(obj);
            }

            return list;
        }

        #endregion

        #region 私有方法

        private T RowToObj<T>(DataRow row, T obj = null) where T : class,new()
        {
            if (obj == null)
                obj = new T();

            foreach (var pi in typeof(T).GetProperties())
            {
                if (!row.Table.Columns.Contains(pi.Name))
                    continue;
                if (row[pi.Name] != System.DBNull.Value)
                {
                    object value = row[pi.Name];
                    if (value is Int64 && pi.PropertyType.FullName.Contains("Int32"))
                        value = int.Parse(value.ToString());
                    if(pi.PropertyType.FullName.Contains("Double"))
                        value = double.Parse(value.ToString());
                    pi.SetValue(obj, value, null);
                }
            }

            return obj;
        }

        #endregion

        #endregion

        #region 常量
        // 数据库连接字符串
        private string connStr = "";
        private string _dbName = "";

        public string ConnStr
        {
            get
            {
                return connStr;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string ShortName
        {
            get
            {
                return Name.Remove(0, Name.LastIndexOf('_') + 1);
            }
        }

        public string DbName
        {
            get
            {
                if (_dbName == "")
                {
                    if (connStr == "")
                        _dbName = ""; ;
                    Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
                    _dbName = conn.Database;
                }
                return _dbName;
            }
        }
        #endregion

        #region 构造函数
        public SQLHelper(string connStr)
        {
            this.connStr = connStr;
        }
        #endregion

        #region SQL无返回值可操作的方法

        //带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(string cmdText, CommandType cmdtype)
        {
            cmdText = SqlTransfer(cmdText);
            string retVal = "0";
            using (Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr))
            {
                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteNonQuery().ToString();
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp, cmdText);
                    throw exp;
                    retVal = exp.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        //带错误输出，无返回值的SQL执行（默认是Text）
        public string ExecuteNonQuery(string cmdText)
        {
            cmdText = SqlTransfer(cmdText);
            string retVal = "0";
            using (Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr))
            {
                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteNonQuery().ToString();
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp,cmdText);
                    //retVal = exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }


        #endregion

        #region SQL有返回值可操作的方法


        //带参，有返回值的SQL执行
        public object ExecuteScalar(string cmdText, CommandType cmdtype)
        {
            cmdText = SqlTransfer(cmdText);

            object retVal = null;

            using (Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr))
            {
                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteScalar();
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp,cmdText);
                    throw exp;
                    retVal = (object)exp.Message;
                }
                finally
                {
                    conn.Close();
                }
            }

            return retVal;
        }

        //有返回值的SQL执行
        public object ExecuteScalar(string cmdText)
        {
            cmdText = SqlTransfer(cmdText);

            object retVal = null;

            using (Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr))
            {
                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteScalar();
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp, cmdText);
                    //retVal = (object)exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }

            return retVal;
        }

        #endregion

        #region 返回数据读取器(DataReader)可操作的方法




        //带SQL执行类型，有返回值的SQL执行
        public System.Data.Common.DbDataReader ExecuteReader(string cmdText, CommandType cmdtype)
        {
            cmdText = SqlTransfer(cmdText);

            Oracle.DataAccess.Client.OracleDataReader reader;

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
            Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
            cmd.CommandType = cmdtype;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, cmdText);
                throw exp;
            }
        }


        //有返回值的SQL执行
        public System.Data.Common.DbDataReader ExecuteReader(string cmdText)
        {
            cmdText = SqlTransfer(cmdText);

            Oracle.DataAccess.Client.OracleDataReader reader;

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
            Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(cmdText, conn);
            cmd.CommandType = CommandType.Text;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, cmdText);
                throw exp;
            }
        }


        #endregion

        #region 返回DataTable可操作的方法

        //带参，带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText, int statRowNum, int maxRowNum, CommandType cmdtype)
        {
            cmdText = SqlTransfer(cmdText);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int dtflag = 0;

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
            Oracle.DataAccess.Client.OracleDataAdapter apt = new Oracle.DataAccess.Client.OracleDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;

            try
            {
                if (statRowNum == 0 && maxRowNum == 0)
                {
                    apt.Fill(dt);
                }
                else
                {
                    apt.Fill(ds, statRowNum, maxRowNum, "ThisTable");
                    dtflag = 1;
                }
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, cmdText);
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            if (dtflag == 1)
            {
                dt = ds.Tables["ThisTable"];
            }


            DataTableTransfer(cmdText, dt);
            return dt;
        }

        //带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdtype)
        {
            cmdText = SqlTransfer(cmdText);

            DataTable dt = new DataTable();

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
            Oracle.DataAccess.Client.OracleDataAdapter apt = new Oracle.DataAccess.Client.OracleDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;
            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, cmdText);
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            DataTableTransfer(cmdText, dt);
            return dt;
        }


        //返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText)
        {
            cmdText = SqlTransfer(cmdText);

            DataTable dt = new DataTable();
            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connStr);
            Oracle.DataAccess.Client.OracleDataAdapter apt = new Oracle.DataAccess.Client.OracleDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = CommandType.Text;
            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, cmdText);
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            DataTableTransfer(cmdText, dt);
            return dt;
        }


        #endregion

        //使得DataTable的字段区分大小写
        private void DataTableTransfer(string sql, DataTable dt)
        {
            string[] dic = sql.Split(' ', ',', '.', '[', ']', '\r', '\n', '(', ')', '|');
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName != "ID" && col.ColumnName == col.ColumnName.ToUpper() && !dic.Contains(col.ColumnName))
                {
                    foreach (string item in dic)
                    {
                        if (item.ToUpper() == col.ColumnName)
                        {
                            col.ColumnName = item;
                            break;
                        }
                    }
                }
            }
        }

        private string SqlTransfer(string sql)
        {
            Regex regex = new Regex("isnull", RegexOptions.IgnoreCase);

            string result = regex.Replace(sql, (Match m) =>
            {
                return "nvl";
            });


            return result;
        }


    }
}
