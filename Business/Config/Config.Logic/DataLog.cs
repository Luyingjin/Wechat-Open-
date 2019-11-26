using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Config.Logic
{
    public class DataLog
    {

        #region LogDataModify

        public static void LogDataModify(string connName, string tableName, string modifyMode, string entityKey, string CurrentValue, string OriginalValue)
        {
            string sql = @"
INSERT INTO [S_D_ModifyLog]
           ([ID],[ConnName],[TableName],[ModifyMode],[EntityKey],[CurrentValue],[OriginalValue],[ModifyUserName],[ModifyTime],[ClientIP],[UserHostAddress])
     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')
";

            if (Config.Constant.IsOracleDb)
            {
                sql = @"
INSERT INTO S_D_ModifyLog
(ID,ConnName,TableName,ModifyMode,EntityKey,CurrentValue,OriginalValue,ModifyUserName,ModifyTime,ClientIP,UserHostAddress) 
VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',to_date('{8}','yyyy-MM-dd hh24:mi:ss'),'{9}','{10}')";
            }
 
            sql = string.Format(sql
                ,GuidHelper.CreateGuid()
                , connName
                , tableName
                , modifyMode
                , entityKey
                , CurrentValue
                , OriginalValue
                , UserService.GetCurrentUserLoginName()
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                , GetUserIP()
                , GetUserHostAddress()
                );

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(sql);

        }

        #endregion

        #region 私有方法

        private static string GetUserIP()
        {
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        private static string GetUserHostAddress()
        {
            return System.Web.HttpContext.Current.Request.UserHostAddress;
        }

        #endregion

    }
}
