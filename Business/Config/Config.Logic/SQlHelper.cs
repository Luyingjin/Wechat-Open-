using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;


namespace Config
{
    /// <summary>
    /// 数据库操作工具类
    /// </summary>
    public class SQLHelper
    {
        #region CreateInsertSql

        public static string CreateInsertSql(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                StringBuilder sbFields = new StringBuilder();
                StringBuilder sbValues = new StringBuilder();
                foreach (DataColumn col in dt.Columns)
                {
                    sbFields.AppendFormat(",{0}", col.ColumnName);
                    sbValues.AppendFormat(",'{0}'", row[col].ToString().Replace("'", "''"));
                }
                string result = @"
INSERT INTO [{2}]
           ({0}) 
          VALUES
           ({1})
";
                result = string.Format(result, sbFields.ToString().Trim(','), sbValues.ToString().Trim(','), tableName);
                sb.AppendLine(result);
            }

            return sb.ToString();
        }

        #endregion

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

                if (System.Configuration.ConfigurationManager.ConnectionStrings[connName] == null)
                    throw new Exception(string.Format("配置文件中不包含数据库连接字符串：{0}", connName));

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
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            cmd.Transaction = trans;
                            retVal = cmd.ExecuteNonQuery().ToString();
                            trans.Commit();
                        }
                        catch { trans.Rollback(); throw; }
                    }

                   
                }
                catch (Exception exp)
                {
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
               T obj =  RowToObj<T>(row, null);
               list.Add(obj);
            }

            return list;
        }

        #endregion

        #region 私有方法

        public T RowToObj<T>(DataRow row, T obj = null) where T : class,new()
        {
            if (obj == null)
                obj = new T();

            foreach (var pi in typeof(T).GetProperties())
            {
                if (!row.Table.Columns.Contains(pi.Name))
                    continue;
                if (row[pi.Name] != System.DBNull.Value)
                    pi.SetValue(obj, row[pi.Name], null);
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
                    SqlConnection conn = new SqlConnection(connStr);
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

        //带事务，带参，带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(SqlTransaction tran, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            string retVal = "0";

            SqlCommand cmd = new SqlCommand(cmdText);
            cmd.Connection = tran.Connection;
            cmd.Transaction = tran;
            cmd.CommandType = cmdtype;
            cmd.CommandTimeout = 600;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
            retVal = cmd.ExecuteNonQuery().ToString();

            return retVal;
        }


        //带连接字符串，带参，带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(string constr, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                if (parms != null)
                {
                    //添加参数
                    foreach (SqlParameter parm in parms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
                try
                {
                    conn.Open();
                    retVal = cmd.ExecuteNonQuery().ToString();
                }
                catch (Exception exp)
                {
                    retVal = exp.Message;
                }
                finally
                {
                    conn.Close();
                }
                return retVal;
            }
        }


        //带连接，带参，带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(SqlConnection conn, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            string retVal = "0";
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                retVal = cmd.ExecuteNonQuery().ToString();
            }
            catch (Exception exp)
            {
                retVal = exp.Message;
            }
            finally
            {
                conn.Close();
            }
            return retVal;
        }


        //带参，带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                if (parms != null)
                {
                    //添加参数
                    foreach (SqlParameter parm in parms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
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
                    //retVal = exp.Message;
                    throw exp;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return retVal;
        }


        //带SQL执行类型，无返回值的SQL执行
        public string ExecuteNonQuery(string cmdText, CommandType cmdtype)
        {
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
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
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
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

        //带连接，带参，带SQL执行类型，有返回值的SQL执行
        public object ExecuteScalar(SqlConnection conn, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            object retVal = null;

            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    cmd.Parameters.Add(parm);
                }
            }

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
                retVal = (object)exp.Message;
            }
            finally
            {
                conn.Close();
            }
            return retVal;
        }

        //带连接字符串，带参，带SQL执行类型，有返回值的SQL执行
        public object ExecuteScalar(string constr, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            object retVal = null;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                if (parms != null)
                {
                    //添加参数
                    foreach (SqlParameter parm in parms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }

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
                    retVal = (object)exp.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        //带参，带SQL执行类型，有返回值的SQL执行
        public object ExecuteScalar(string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            object retVal = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = cmdtype;

                if (parms != null)
                {
                    //添加参数
                    foreach (SqlParameter parm in parms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }

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
                    retVal = (object)exp.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        //带参，有返回值的SQL执行
        public object ExecuteScalar(string cmdText, CommandType cmdtype)
        {
            object retVal = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
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
            object retVal = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
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

        //带连接字符串，带参，带SQL执行类型，有返回值的SQL执行
        public SqlDataReader ExecuteReader(string constr, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            SqlDataReader reader;
            SqlConnection conn = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                    cmd.Parameters.Add(parm);
            }
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //将关闭数据库的操作交给Read操作，即关闭了read就关闭了数据库
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        //带连接，带参，带SQL执行类型，有返回值的SQL执行
        public SqlDataReader ExecuteReader(SqlConnection conn, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                    cmd.Parameters.Add(parm);
            }
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //将关闭数据库的操作交给Read操作，即关闭了read就关闭了数据库
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        //带参，带SQL执行类型，有返回值的SQL执行
        public SqlDataReader ExecuteReader(string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            SqlDataReader reader;
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                    cmd.Parameters.Add(parm);
            }
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //将关闭数据库的操作交给Read操作，即关闭了read就关闭了数据库
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        //带SQL执行类型，有返回值的SQL执行
        public SqlDataReader ExecuteReader(string cmdText, CommandType cmdtype)
        {
            SqlDataReader reader;

            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
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
                throw exp;
            }
        }


        //有返回值的SQL执行
        public SqlDataReader ExecuteReader(string cmdText)
        {
            SqlDataReader reader;

            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
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
                throw exp;
            }
        }


        #endregion

        #region 返回DataTable可操作的方法

        //带参，带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText, int statRowNum, int maxRowNum, CommandType cmdtype)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int dtflag = 0;

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
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
            return dt;
        }

        //带参，带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    apt.SelectCommand.Parameters.Add(parm);
                }
            }

            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }


        //带连接字符串，带参，带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string constr, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(constr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    apt.SelectCommand.Parameters.Add(parm);
                }
            }

            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }


        //带连接，带参，带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(SqlConnection conn, string cmdText, SqlParameter[] parms, CommandType cmdtype)
        {
            DataTable dt = new DataTable();

            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;

            if (parms != null)
            {
                //添加参数
                foreach (SqlParameter parm in parms)
                {
                    apt.SelectCommand.Parameters.Add(parm);
                }
            }

            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }


        //带SQL执行类型，返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdtype)
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = cmdtype;
            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }


        //返回DataTable的SQL执行
        public DataTable ExecuteDataTable(string cmdText)
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = CommandType.Text;
            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }


        #endregion

    }
}
