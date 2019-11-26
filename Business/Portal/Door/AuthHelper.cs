using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Config;
using System.Configuration;
using Formula;

namespace Portal
{
    public class AuthHelper
    {
        public static string HomeBoardUrl
        {
            get
            {
                string url = string.Empty;
                string authMode = string.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthMode"]) ? "Tree" : ConfigurationManager.AppSettings["AuthMode"];
                switch (authMode.ToLower())
                {
                    case "office":
                        url = "HomeBoardIframe2.aspx";
                        break;
                    case "menu":
                        url = "HomeBoardIframe3.aspx";
                        break;
                    default:
                        url = "HomeBoarIframe.aspx";
                        break;
                }
                return url;
            }
        }


        public static string getUserMenuRootID(string userID)
        {
            return Config.Constant.MenuRooID;
        }

        public static DataRow getMenuByID(string menuID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string sql = string.Format(@"select * from S_A_Res where ID='{0}'", menuID);

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            return dt.Rows[0];
        }


        public static DataTable getUserMenu(string userID)
        {
            var service = FormulaHelper.GetService<IResService>();
            DataTable dt = service.GetResTree(Config.Constant.MenuRooID, userID);
            return dt;
        }


        public static DataTable getUserMenu(string userID, string menuParentID, bool hasChildMenu = false)
        {
            if (string.IsNullOrEmpty(menuParentID))
                menuParentID = getUserMenuRootID(userID);
            DataTable dt = getUserMenu(userID);

            var parentMenu = dt.AsEnumerable().Where(c => c.Field<string>("ID") == menuParentID).SingleOrDefault();

            if (parentMenu == null)
            {
                dt.Rows.Clear();
                return dt;
            }

            string parentFullID = parentMenu.Field<string>("FullID");

            if (hasChildMenu)
            {
                return dt.AsEnumerable().Where(c => c.Field<string>("FullID").StartsWith(parentFullID) && c.Field<string>("FullID") != parentFullID).CopyToDataTable();
            }
            else
            {
                DataTable result=null;
                var query = dt.AsEnumerable().Where(c => c.Field<string>("ParentID") == menuParentID);
                if (query.Count() == 0)
                {
                    result = dt.Clone();
                    result.Clear();
                }
                else
                {
                    result = query.CopyToDataTable();
                }
                

                result.Columns.Add("ChildCount");
                foreach (DataRow row in result.Rows)
                {
                    int childCount = dt.AsEnumerable().Where(c => c["ParentID"].ToString() == row["ID"].ToString()).Count();
                    row["ChildCount"] = childCount;
                }

                return result;
            }

        }

        public static DataTable getMenuIcon()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (Config.Constant.IsOracleDb)
                return sqlHelper.ExecuteDataTable("select ID,IconUrl from S_A_Res where IconUrl is not null and FullID like '" + Config.Constant.MenuRooID + "%'");
            else
                return sqlHelper.ExecuteDataTable("select ID,IconUrl from S_A_Res where IconUrl is not null and IconUrl<>'' and FullID like '" + Config.Constant.MenuRooID + "%'");
        }

        public static DataTable GetDeptartment()
        {
            string sql = "select * from S_A_Org where ParentID = 'a1b10168-61a9-44b5-92ca-c5659456deb5' order by SortIndex";
            DataTable dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql);
            return dt;
        }
    }
}