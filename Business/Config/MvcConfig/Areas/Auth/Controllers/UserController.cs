using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using MvcAdapter;
using Formula;
using Formula.Helper;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class UserController : BaseController
    {
        protected string OwnerDeptOrg = "System_OwnerDept";

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult SingleSelector()
        {
            return View();
        }

        public ActionResult MultiSelector()
        {
            return View();
        }

        [ValidateInput(false)]
        public JsonResult DropNode()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;

            Dictionary<string, object> dragNode = JsonHelper.ToObject<Dictionary<string, object>>(Request["dragNode"]);
            Dictionary<string, object> dropNode = JsonHelper.ToObject<Dictionary<string, object>>(Request["dropNode"]);
            string dragAction = Request["dragAction"];

            string sql = "";
            var dropSortIndex = Convert.ToDouble(dropNode["SortIndex"]);
            double newSortIndex;
            if (dragAction == "after")
            {
                sql = string.Format("select min(SortIndex) from S_A_UserLinkMan where UserID='{0}' and SortIndex>'{1}'", userID, dropSortIndex);
                object obj = sqlHelper.ExecuteScalar(sql);
                if (obj == null)
                    newSortIndex = Math.Ceiling(dropSortIndex) + 1;
                else
                    newSortIndex = (Convert.ToDouble(obj) + dropSortIndex) / 2;
            }
            else
            {
                sql = string.Format("select max(SortIndex) from S_A_UserLinkMan where UserID='{0}' and SortIndex<'{1}'", userID, dropSortIndex);
                object obj = sqlHelper.ExecuteScalar(sql);
                if (obj == null || obj.ToString() == "")
                    newSortIndex = Math.Floor(dropSortIndex) - 1;
                else
                    newSortIndex = (Convert.ToDouble(obj) + dropSortIndex) / 2;
            }

            sql = string.Format("Update S_A_UserLinkMan Set SortIndex='{0}' Where UserID='{1}' and LinkManID='{2}'", newSortIndex, userID, dragNode["ID"]);

            sqlHelper.ExecuteNonQuery(sql);

            return Json("");

        }

        public JsonResult DeleteLinkMan(string id)
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            sqlHelper.ExecuteDataTable(string.Format("delete  S_A_UserLinkMan Where UserID='{0}' and LinkManID='{1}'", userID, id));

            return Json("");
        }

        public JsonResult GetOrgUserList(string OrgID, QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            if (string.IsNullOrEmpty(OrgID))
                OrgID = Config.Constant.OrgRootID;


            string key = "";
            if (qb.Items.Count() > 0)
                key = qb.Items[0].Value.ToString();
            string sql = getUserSql(key, OrgID, "");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            qb.Items.Clear();
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }


        public JsonResult SaveUserLinkMan(string linkManID)
        {
            linkManID = linkManID.Substring(1, linkManID.Length - 2);
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt1 = sqlHelper.ExecuteDataTable(string.Format("select ID from S_A_UserLinkMan  where UserID='{0}' and LinkManID='{1}' ", userID, linkManID));
            if (dt1.Rows.Count > 0)
                return Json(JsonAjaxResult.Successful("联系人已存在！"));

            string newID = FormulaHelper.CreateGuid();
            object obj = sqlHelper.ExecuteScalar(string.Format("select Max(SortIndex) as SortIndex from S_A_UserLinkMan where UserID='{0}'", userID));
            double maxSort = 0;
            if (obj is DBNull)
                maxSort = 0;
            else
                maxSort = Convert.ToDouble(obj);

            sqlHelper.ExecuteDataTable(string.Format("Insert Into S_A_UserLinkMan (ID,UserID,LinkManID,SortIndex) Values ('{0}','{1}','{2}',{3})", newID, userID, linkManID, Math.Ceiling(maxSort) + 1));
            return Json(JsonAjaxResult.Successful("添加成功！"));
        }


        public JsonResult GetLinkManTree()
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select S_A_User.ID,S_A_User.Name,S_A_User.Name as UserName,S_A_User.DeptName,S_A_User.DeptID,S_A_User.WorkNo,'Root' as ParentID,S_A_UserLinkMan.SortIndex from S_A_UserLinkMan Join S_A_User on S_A_UserLinkMan.LinkManID=S_A_User.ID where S_A_UserLinkMan.UserID='{0}' Order By SortIndex ASC", userID));
            foreach (DataRow dr in dt.Rows)
            {
                dr["Name"] += string.Format("(<span style=\"hand:cursor\" onclick='LinkManSelect(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\")'><font color='blue'><u>选择</u></font></span>&nbsp;<span style=\"cursor:hand\" onclick='RemoveLinkMan(\"{0}\")' title='从我的联系人移除'><font color='#A6A52A'>移除</font></span>)", dr["ID"], dr["UserName"], dr["WorkNo"], dr["DeptName"], dr["DeptID"]);
            }
            var dtRow = dt.NewRow();
            dtRow["ID"] = "Root";
            dtRow["Name"] = "我的联系人";
            dtRow["ParentID"] = "-1";
            dtRow["SortIndex"] = "-1";
            dt.Rows.Add(dtRow);
            return Json(dt);
        }

        public JsonResult GetMyLinkManIDs()
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select S_A_User.ID from S_A_UserLinkMan Join S_A_User on S_A_UserLinkMan.LinkManID=S_A_User.ID where S_A_UserLinkMan.UserID='{0}' ", userID));

            return Json(dt);
        }

        public ActionResult SingleScopeSelector()
        {
            return View();
        }

        public ActionResult MultiScopeSelector()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetUsers(string ids)
        {
            string sql = "select * from S_A_User where ID in ('" + ids.Replace(",", "','") + "')";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (Config.Constant.IsOracleDb)
            {
                sql = "SELECT ID, WORKNO as \"WorkNo\", NAME as \"Name\" FROM S_A_USER WHERE ID IN ('" + ids.Replace(",", "','") + "')";
            }
            return Json(sqlHelper.ExecuteDataTable(sql), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDobackUserList(string userIDs, QueryBuilder qb)
        {
            string sql = string.Format("select * from S_A_User where ID in ('{0}')", userIDs.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }


        [AllowAnonymous]
        public JsonResult GetScopeUserList(string orgIDs, string roleIDs, QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            //roleIDs可能为编号,转换为roleIDs
            if (!string.IsNullOrEmpty(roleIDs))
            {
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID from S_A_Role where Code in ('{0}')", roleIDs.Replace(",", "','")));
                if (dt.Rows.Count > 0)
                    roleIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
            }


            string key = "";

            if (qb.Items.Count() > 0)
                key = qb.Items[0].Value.ToString();
            string sql = getUserSql(key, orgIDs, roleIDs);
            qb.Items.Clear();
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        [AllowAnonymous]
        public JsonResult SelectUsers(string key, string value, string orgIDs, string roleIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                int firstCode = (int)key[0];
                if (48 <= firstCode && firstCode <= 57)	//number
                    whereStr = " WorkNo like '" + key + "%'";
                else if ((65 <= firstCode && firstCode <= 90) || (97 <= firstCode && firstCode <= 122))	//letter
                {
                    string[,] hz = GetHanziScope(key);
                    for (int i = 0; i < hz.GetLength(0); i++)
                    {
                        if (!string.IsNullOrEmpty(whereStr))
                            whereStr += " and ";
                        if (Config.Constant.IsOracleDb)
                            whereStr += "nlssort(SUBSTR(\"Name\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') >= nlssort('" + hz[i, 0] + "','NLS_SORT=SCHINESE_PINYIN_M') AND nlssort(SUBSTR(\"Name\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') <= nlssort('" + hz[i, 1] + "','NLS_SORT=SCHINESE_PINYIN_M')";
                        else
                            whereStr += "SUBSTRING(Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(Name, " + (i + 1) + ", 1) <= '" + hz[i, 1] + "'";
                    }
                }
                else if (firstCode >= 255)
                {	//chinese
                    if (Config.Constant.IsOracleDb)
                        whereStr = "\"Name\" like '%" + key + "%'";
                    else
                        whereStr = "Name like '%" + key + "%'";
                }
            }
            if (!string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(whereStr))
                    whereStr += " and ";
                whereStr += "ID not in ('" + value.Replace(",", "','") + "')";
            }

            string sql = "";
            if (Config.Constant.IsOracleDb)
                sql = "select * from (select ID, WORKNO as \"WorkNo\", NAME as \"Name\",DEPTNAME as \"DeptName\" from S_A_User where nvl(IsDeleted,0) = 0) Users ";
            else
                sql = "select * from (select * from S_A_User where isnull(IsDeleted,0) = 0) Users ";

            if (!string.IsNullOrEmpty(orgIDs) || !string.IsNullOrEmpty(roleIDs))
            {
                sql = "select * from (" + GetScopeSql(orgIDs, roleIDs) + ") Users ";
            }
            if (!string.IsNullOrEmpty(whereStr))
                sql += " where " + whereStr;

            if (Config.Constant.IsOracleDb)
            {
                sql = "select * from (" + sql + ") FilterUsers Where rownum <= 10";

            }
            else
            {
                sql = "select top 10 * from (" + sql + ") FilterUsers";
            }
            return Json(sqlHelper.ExecuteDataTable(sql), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetRetiredUserList(QueryBuilder qb)
        {
            string sql = "select * from S_A_User where IsDeleted='1'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(JsonHelper.ToJson(sqlHelper.ExecuteGridData(sql, qb)));


            //if (qb.DefaultSort)
            //{
            //    qb.SortField = "SortIndex,WorkNo";
            //    qb.SortOrder = "asc,asc";
            //}          

            //string key = "";
            //if (qb.Items.Count() > 0)
            //    key = qb.Items[0].Value.ToString();
            //string sql = getUserSql(key, "", "");

            //sql = sql.Replace("IsDeleted='0'", "IsDeleted='1'");

            //SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            //qb.Items.Clear();
            //return Json(sqlHelper.ExecuteGridData(sql, qb));
        }



        #region 私有方法

        private string getUserSql(string key, string orgIDs, string roleIDs)
        {
            if (string.IsNullOrEmpty(orgIDs))
                orgIDs = Config.Constant.OrgRootID;

            if (string.IsNullOrEmpty(key))
            {
                return GetScopeSql(orgIDs, roleIDs);
            }
            else
            {
                string whereStr = "";
                if (!string.IsNullOrEmpty(key))
                {
                    int firstCode = (int)key[0];

                    string[,] hz = GetHanziScope(key);
                    for (int i = 0; i < hz.GetLength(0); i++)
                    {
                        if (whereStr != "")
                            whereStr += " and";
                        if (Config.Constant.IsOracleDb)
                            whereStr += " nlssort(SUBSTR(Name, " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') >= nlssort('" + hz[i, 0] + "','NLS_SORT=SCHINESE_PINYIN_M') AND nlssort(SUBSTR(Name, " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') <= nlssort('" + hz[i, 1] + "','NLS_SORT=SCHINESE_PINYIN_M')";
                        else
                            whereStr += " SUBSTRING(Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(Name, " + (i + 1) + ", 1) < '" + hz[i, 1] + "'";
                    }
                    if (Config.Constant.IsOracleDb)
                        whereStr = string.Format(" where ({0}) or regexp_like(WorkNo,'{1}','i') or regexp_like(Name,'{1}','i')", whereStr, key);
                    else
                        whereStr = string.Format(" where ({0}) or WorkNo like '{1}%' or Name like '{1}%'", whereStr, key);

                }

                string sql = "select * from (" + GetScopeSql(orgIDs, roleIDs) + ") Users ";
                sql += whereStr;

                return sql;
            }

        }

        private string[,] GetHanziScope(string pinyinIndex)
        {
            pinyinIndex = pinyinIndex.ToLower();
            string[,] hz = new string[pinyinIndex.Length, 2];
            for (int i = 0; i < pinyinIndex.Length; i++)
            {
                string index = pinyinIndex.Substring(i, 1);
                if (index == "a") { hz[i, 0] = "吖"; hz[i, 1] = "驁"; }
                else if (index == "b") { hz[i, 0] = "八"; hz[i, 1] = "簿"; }
                else if (index == "c") { hz[i, 0] = "嚓"; hz[i, 1] = "錯"; }
                else if (index == "d") { hz[i, 0] = "咑"; hz[i, 1] = "鵽"; }
                else if (index == "e") { hz[i, 0] = "妸"; hz[i, 1] = "樲"; }
                else if (index == "f") { hz[i, 0] = "发"; hz[i, 1] = "猤"; }
                else if (index == "g") { hz[i, 0] = "旮"; hz[i, 1] = "腂"; }
                else if (index == "h") { hz[i, 0] = "妎"; hz[i, 1] = "夻"; }
                else if (index == "j") { hz[i, 0] = "丌"; hz[i, 1] = "攈"; }
                else if (index == "k") { hz[i, 0] = "咔"; hz[i, 1] = "穒"; }
                else if (index == "l") { hz[i, 0] = "垃"; hz[i, 1] = "鱳"; }
                else if (index == "m") { hz[i, 0] = "嘸"; hz[i, 1] = "椧"; }
                else if (index == "n") { hz[i, 0] = "拏"; hz[i, 1] = "桛"; }
                else if (index == "o") { hz[i, 0] = "噢"; hz[i, 1] = "漚"; }
                else if (index == "p") { hz[i, 0] = "妑"; hz[i, 1] = "曝"; }
                else if (index == "q") { hz[i, 0] = "七"; hz[i, 1] = "裠"; }
                else if (index == "r") { hz[i, 0] = "亽"; hz[i, 1] = "鶸"; }
                else if (index == "s") { hz[i, 0] = "仨"; hz[i, 1] = "蜶"; }
                else if (index == "t") { hz[i, 0] = "他"; hz[i, 1] = "籜"; }
                else if (index == "w") { hz[i, 0] = "屲"; hz[i, 1] = "鶩"; }
                else if (index == "x") { hz[i, 0] = "夕"; hz[i, 1] = "鑂"; }
                else if (index == "y") { hz[i, 0] = "丫"; hz[i, 1] = "韻"; }
                else if (index == "z") { hz[i, 0] = "帀"; hz[i, 1] = "咗"; }
                else { hz[i, 0] = index; hz[i, 1] = index; }
            }
            return hz;
        }

        private string GetScopeSql(string orgIDs, string roleIDs)
        {
            string sql = "";

            if (!string.IsNullOrEmpty(orgIDs))
            {
                if (!string.IsNullOrEmpty(roleIDs))
                {
                    string str = "";
                    foreach (string orgID in orgIDs.Split(','))
                    {
                        if (orgID == "") continue;
                        str += string.Format(" or FullID like '%{0}%'", orgID);
                    }
                    str = str.Substring(3);

                    sql = string.Format(@"
--继承的组织角色
select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,S_A_User.DeleteTime from S_A_User 
join S_A__OrgUser on UserID=S_A_User.ID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID and ({0})
join S_A__OrgRole on roleID in('{1}') and S_A__OrgRole.OrgID= S_A__OrgUser.OrgID 
union
--系统角色
select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,DeleteTime from S_A_User join S_A__RoleUser on RoleID in('{1}') and UserID=S_A_User.ID 
--直接组织角色
union
select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,S_A_User.DeleteTime 
from S_A_User join S_A__OrgRoleUser on RoleID in('{1}') and UserID=S_A_User.ID 
join S_A_Org on OrgID=S_A_Org.ID and {0}
"
                        , str
                        , roleIDs.Replace(",", "','"));
                }
                else
                {
                    sql = string.Format("select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,DeleteTime from S_A_User join S_A__OrgUser on orgID in('{0}') and UserID=S_A_User.ID ", orgIDs.Replace(",", "','"));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(roleIDs))
                {
                    sql = string.Format("select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,DeleteTime from S_A_User join S_A__RoleUser on RoleID in('{0}') and UserID=S_A_User.ID ", roleIDs.Replace(",", "','"));
                }
                else
                {
                    sql = "select S_A_User.ID,S_A_User.Code,S_A_User.Name,S_A_User.WorkNo,S_A_User.SortIndex,DeptName,DeptID,DeleteTime from S_A_User ";
                }
            }

            sql += string.Format(" where (S_A_User.IsDeleted is null or S_A_User.IsDeleted='0')");

            return sql;
        }
        #endregion
    }
}
