using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Config;
using System.Text;
using Formula.Helper;
using Formula;

namespace MvcConfig.Areas.UI.Controllers
{
    public static class DbHelper
    {
        private static DataTable GetFieldTable(string connName, string tableName)
        {
            //string sql = string.Format("select FieldCode=a.name from syscolumns a  inner join sysobjects d on a.id=d.id and d.name='{0}'", tableName);
            string sql = string.Format(@"
SELECT syscolumns.name FieldCode,systypes.name Type,syscolumns.isnullable Nullable,
syscolumns.length
FROM syscolumns, systypes
WHERE syscolumns.xusertype = systypes.xusertype
AND syscolumns.id =
object_id('{0}')
", tableName);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return dt;
        }

        public static string CreateInsertSql(this Dictionary<string, string> dic, string connName, string tableName, string ID)
        {
            var dt = GetFieldTable(connName, tableName).AsEnumerable();
            var fields = dt.Select(c => c[0].ToString()).ToArray();


            StringBuilder sbField = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();

            foreach (string key in dic.Keys)
            {
                if (key == "ID")
                    continue;
                if (!fields.Contains(key))
                    continue;

                string value = dic[key];
                value = GetValue(dt, key, value);

                sbField.AppendFormat(",{0}", key);
                sbValue.AppendFormat(",{0}", value);
            }

            var user = FormulaHelper.GetUserInfo();
            if (fields.Contains("CreateTime") && !dic.Keys.Contains("CreateTime"))
            {
                sbField.AppendFormat(",{0}", "CreateTime");
                sbValue.AppendFormat(",'{0}'", DateTime.Now);
            }
            if (fields.Contains("CreateDate") && !dic.Keys.Contains("CreateDate"))
            {
                sbField.AppendFormat(",{0}", "CreateDate");
                sbValue.AppendFormat(",'{0}'", DateTime.Now);
            }
            if (fields.Contains("CreateUserID") && !dic.Keys.Contains("CreateUserID"))
            {
                sbField.AppendFormat(",{0}", "CreateUserID");
                sbValue.AppendFormat(",'{0}'", user.UserID);
            }
            if (fields.Contains("CreateUserName") && !dic.Keys.Contains("CreateUserName"))
            {
                sbField.AppendFormat(",{0}", "CreateUserName");
                sbValue.AppendFormat(",'{0}'", user.UserName);
            }
            if (fields.Contains("CreateUser") && !dic.Keys.Contains("CreateUser"))
            {
                sbField.AppendFormat(",{0}", "CreateUser");
                sbValue.AppendFormat(",'{0}'", user.UserName);
            }
            if (fields.Contains("OrgID") && !dic.Keys.Contains("OrgID"))
            {
                sbField.AppendFormat(",{0}", "OrgID");
                sbValue.AppendFormat(",'{0}'", user.UserOrgID);
            }
            if (fields.Contains("PrjID") && !dic.Keys.Contains("PrjID"))
            {
                sbField.AppendFormat(",{0}", "PrjID");
                sbValue.AppendFormat(",'{0}'", user.UserPrjID);
            }

            if (fields.Contains("FlowPhase") && !dic.Keys.Contains("FlowPhase"))
            {
                sbField.AppendFormat(",{0}", "FlowPhase");
                sbValue.AppendFormat(",'{0}'", "Start");
            }

            if (fields.Contains("ModifyTime") && !dic.Keys.Contains("ModifyTime"))
            {
                sbField.AppendFormat(",{0}", "ModifyTime");
                sbValue.AppendFormat(",'{0}'", DateTime.Now);
            }
            if (fields.Contains("ModifyDate") && !dic.Keys.Contains("ModifyDate"))
            {
                sbField.AppendFormat(",{0}", "ModifyDate");
                sbValue.AppendFormat(",'{0}'", DateTime.Now);
            }
            if (fields.Contains("ModifyUserID") && !dic.Keys.Contains("ModifyUserID"))
            {
                sbField.AppendFormat(",{0}", "ModifyUserID");
                sbValue.AppendFormat(",'{0}'", user.UserID);
            }
            if (fields.Contains("ModifyUserName") && !dic.Keys.Contains("ModifyUserName"))
            {
                sbField.AppendFormat(",{0}", "ModifyUserName");
                sbValue.AppendFormat(",'{0}'", user.UserName);
            }
            if (fields.Contains("ModifyUser") && !dic.Keys.Contains("ModifyUser"))
            {
                sbField.AppendFormat(",{0}", "ModifyUser");
                sbValue.AppendFormat(",'{0}'", user.UserName);
            }

            string sql = string.Format(@"INSERT INTO {0} (ID{2}) VALUES ('{1}'{3})", tableName, ID, sbField, sbValue);

            return sql;
        }

        public static string CreateUpdateSql(this Dictionary<string, string> dic, Dictionary<string, string> oldDic, Dictionary<string, string> currentDic, string connName, string tableName, string ID)
        {
            var dt = GetFieldTable(connName, tableName).AsEnumerable();
            var fields = dt.Select(c => c[0].ToString()).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (string key in dic.Keys)
            {
                if (key == "ID")
                    continue;
                if (!fields.Contains(key))
                    continue;
                if (key == "SerialNumber")//流水号不能修改
                    continue;
                else if (CompareDicItem(dic, oldDic, key) == false && CompareDicItem(oldDic, currentDic, key) == true)
                    sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
                else if (CompareDicItem(dic, oldDic, key) == false && CompareDicItem(oldDic, currentDic, key) == false && CompareDicItem(dic, currentDic, key) == false)
                    throw new Exception(string.Format("您正在修改的数据已经发生变更，无法保存"));
            }

            if (sb.ToString().Trim() == "")
                return "";

            var user = FormulaHelper.GetUserInfo();
            if (fields.Contains("ModifyTime") && !dic.Keys.Contains("ModifyTime"))
            {
                sb.AppendFormat(",ModifyTime='{0}'", DateTime.Now);
            }
            if (fields.Contains("ModifyDate") && !dic.Keys.Contains("ModifyDate"))
            {
                sb.AppendFormat(",ModifyDate='{0}'", DateTime.Now);
            }
            if (fields.Contains("ModifyUserID") && !dic.Keys.Contains("ModifyUserID"))
            {
                sb.AppendFormat(",ModifyUserID='{0}'", user.UserID);
            }
            if (fields.Contains("ModifyUserName") && !dic.Keys.Contains("ModifyUserName"))
            {
                sb.AppendFormat(",ModifyUserName='{0}'", user.UserName);
            }
            if (fields.Contains("ModifyUser") && !dic.Keys.Contains("ModifyUser"))
            {
                sb.AppendFormat(",ModifyUser='{0}'", user.UserName);
            }
            string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}'", tableName, ID, sb.ToString().Trim(','));
            return sql;
        }

        private static bool CompareDicItem(Dictionary<string, string> dic1, Dictionary<string, string> dic2, string key)
        {
            if (dic1.ContainsKey(key) && dic2.ContainsKey(key))
            {
                return dic1[key] == dic2[key];
            }
            else if (!dic1.ContainsKey(key) && !dic2.ContainsKey(key))
            {
                return true;
            }
            else
                return false;
        }

        public static string CreateUpdateSql(this Dictionary<string, string> dic, string connName, string tableName, string ID)
        {
            var dt = GetFieldTable(connName, tableName).AsEnumerable();
            var fields = dt.Select(c => c[0].ToString()).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (string key in dic.Keys)
            {
                if (key == "ID")
                    continue;
                if (!fields.Contains(key))
                    continue;
                if (key == "SerialNumber")//流水号不能修改
                    continue;

                sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
            }

            if (sb.ToString().Trim() == "")
                return "";
            string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}'", tableName, ID, sb.ToString().Trim(','));
            return sql;
        }


        private static string GetValue(EnumerableRowCollection<DataRow> fieldRows, string fieldCode, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                var field = fieldRows.SingleOrDefault(c => c.Field<string>("FieldCode") == fieldCode);
                if (field["Type"].ToString() != "nvarchar" && field["Nullable"].ToString() == "1")
                    value = "NULL";
                else
                    value = "''";
            }
            else
            {
                value = "'" + value + "'";
            }

            return value;
        }

    }

}