using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;

namespace Config.Logic
{
    public class UserService
    {
        #region 获取当前登录名

        public static string GetCurrentUserLoginName()
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null)
                return HttpContext.Current.User.Identity.Name;
            return "";
        }

        #endregion

        #region 根据登录名获取用户信息

        public static UserInfo GetUserInfoBySysName(string systemName)
        {
            string sql = string.Format("select * from S_A_User where Code='{0}'", systemName);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return null;

            return ParseDataRow(dt.Rows[0]);
        }

        #endregion

        #region 根据ID获取用户信息

        public static UserInfo GetUserInfoByID(string userID)
        {
            string sql = string.Format("select * from S_A_User where ID='{0}'", userID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return null;
            return ParseDataRow(dt.Rows[0]);
        }

        #endregion

        #region 获取用户角色，逗号隔开

        public static string GetRolesForUser(string userID, string orgID)
        {
            string roles = "";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //系统角色
            string sql = string.Format("select S_A_Role.* from S_A__RoleUser join S_A_Role on UserID='{0}' and RoleID=S_A_Role.ID and S_A_Role.Type='SysRole'",
                userID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            roles = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());

            if (!string.IsNullOrEmpty(orgID))
            {
                //组织角色
                sql = string.Format(@"
select S_A_Role.* from S_A__OrgUser join S_A_Org on userID='{0}' and FullID like '%{1}%' and OrgID=ID join S_A__OrgRole on S_A__OrgRole.OrgID=ID join S_A_Role on S_A_Role.ID=RoleID
union
select S_A_Role.* from S_A__OrgRoleUser join S_A_Role on RoleID=S_A_Role.ID and UserID='{0}' join S_A_Org on S_A_Org.ID=OrgID and FullID like '%{1}%'", userID, orgID);
                dt = sqlHelper.ExecuteDataTable(sql);
                roles += "," + string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
            }

            roles = string.Join(",", roles.Split(',').Distinct()).Trim(',');

            return roles;
        }

        public static string GetRoleCodesForUser(string userID, string orgID)
        {
            string roles = "";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //系统角色
            string sql = string.Format("select S_A_Role.* from S_A__RoleUser join S_A_Role on UserID='{0}' and RoleID=S_A_Role.ID and S_A_Role.Type='SysRole'",
                userID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            roles = string.Join(",", dt.AsEnumerable().Select(c => c["Code"].ToString()).ToArray());

            if (!string.IsNullOrEmpty(orgID))
            {
                //组织角色
                sql = string.Format(@"
select S_A_Role.* from S_A__OrgUser join S_A_Org on userID='{0}' and FullID like '%{1}%' and OrgID=ID join S_A__OrgRole on S_A__OrgRole.OrgID=ID join S_A_Role on S_A_Role.ID=RoleID
union
select S_A_Role.* from S_A__OrgRoleUser join S_A_Role on RoleID=S_A_Role.ID and UserID='{0}' join S_A_Org on S_A_Org.ID=OrgID and FullID like '%{1}%'", userID, orgID);
                dt = sqlHelper.ExecuteDataTable(sql);
                roles += "," +  string.Join(",", dt.AsEnumerable().Select(c => c["Code"].ToString()).ToArray());
            }

            roles = string.Join(",", roles.Split(',').Distinct()).Trim(',');

            return roles;
        }

        #endregion

        #region 查询用户

        public static IList<UserInfo> GetAllUsers()
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = "select S_A_User.* from dbo.S_A_User where S_A_User.IsDeleted = '0' order by SortIndex";

            var users = new List<UserInfo>();
            var dt = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                users.Add(ParseDataRow(row));
            }
            return users;
        }

        #endregion

        #region 获取用户签名图

        public static byte[] GetSignImg(string userID)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format("SELECT SIGNIMG FROM S_A_USERIMG WHERE USERID='{0}'", userID);
            var obj = sqlHelper.ExecuteScalar(sql);
            if (!System.DBNull.Value.Equals(obj))
                return (byte[])obj;
            else
                return null;
        }

        #endregion

        #region 获取用户照片

        public static byte[] GetUserImg(string userID)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format("SELECT PICTURE FROM S_A_USERIMG WHERE USERID='{0}'", userID);
            var obj = sqlHelper.ExecuteScalar(sql);
            if (!System.DBNull.Value.Equals(obj))
                return (byte[])obj;
            else
                return null;
        }

        #endregion

        #region 根据用ID获取用户名

        public static string GetUserNames(string userIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string sql = @"Select ID,Name from S_A_User where ID in ('{0}')";
            sql = string.Format(sql, userIDs.Replace(",", "','"));

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            string userName = "";
            var userIDArr = userIDs.Split(',');
            for (int i = 0; i < userIDArr.Length; i++)
            {
                if (userIDArr[i] == "")
                    userName += ",";
                else
                    userName += "," + dt.AsEnumerable().Where(c => c["ID"].ToString().ToLower() == userIDArr[i].ToLower()).SingleOrDefault()["Name"].ToString();
            }
            userName = userName.Trim(',');
            return userName;
        }

        #endregion

        #region InRole

        public static bool InRole(string userID, string roleIDs, string orgIDs)
        {
            if (string.IsNullOrEmpty(orgIDs))
                orgIDs = Config.Constant.OrgRootID;

            #region 判断系统角色

            string sql = @"
select count(1) from S_A_User join S_A__RoleUser on  S_A_User.ID=UserID
where S_A_User.ID='{0}' and RoleID in ('{1}')";
            sql = string.Format(sql, userID, roleIDs.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            object obj = sqlHelper.ExecuteScalar(sql);
            if (Convert.ToInt32(obj) > 0)
                return true;

            #endregion

            #region 判断组织角色

            foreach (string orgID in orgIDs.Split(','))
            {
                if (orgID == "")
                    continue;

                string fullID = sqlHelper.ExecuteScalar(string.Format("select FullID from S_A_Org where ID='{0}'", orgID)).ToString();

                sql = @"
select count(1) from
(
select S_A_User.ID from S_A_User 
join S_A__OrgUser on  S_A_User.ID=UserID
join S_A__OrgRole on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID and FullID like '{1}%'
where UserID='{0}'  and S_A__OrgRole.RoleID in ('{2}')
union
select S_A_User.ID from S_A_User
join S_A__OrgRoleUser on UserID=S_A_User.ID
join S_A_Org on S_A_Org.ID=OrgID
where UserID='{0}' and FullID like '{1}'+'%' and RoleID in ('{2}')
) a";

                sql = string.Format(sql, userID, fullID, roleIDs.Replace(",", "','"));

                obj = sqlHelper.ExecuteScalar(sql);
                if (Convert.ToInt32(obj) > 0)
                    return true;

            }

            #endregion

            return false;
        }

        #endregion

        #region InOrg

        public static bool InOrg(string userID, string orgIDs)
        {
            if (orgIDs == null)
                orgIDs = "";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            foreach (string orgID in orgIDs.Split(','))
            {
                if (orgID == "")
                    continue;

                string fullID = sqlHelper.ExecuteScalar(string.Format("select FullID from S_A_Org where ID='{0}'", orgID)).ToString();

                string sql = @"
select count(1) from S_A_User 
join S_A__OrgUser on  S_A_User.ID=UserID
join S_A_Org on S_A_Org.ID=OrgID and FullID like '{1}%'
where S_A_User.ID='{0}' ";

                sql = string.Format(sql, userID, fullID);

                object obj = sqlHelper.ExecuteScalar(sql);
                if (Convert.ToInt32(obj) > 0)
                    return true;

            }

            return false;
        }

        #endregion

        #region 获取用户指定角色的部门ID
        /// <summary>
        /// 获取用户指定角色的所有组织ID，多个用逗号分隔.
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public static string GetUserOrgsByRoleID(string userID, string roleID)
        {
            string sql = string.Format(@"SELECT ParentID FROM S_A_Org where Type='Post' 
AND ID IN (select OrgID FROM S_A__OrgUser where UserID='{0}') 
AND ID IN (select OrgID FROM S_A__OrgRole where RoleID='{1}')", userID, roleID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            string orgIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ParentID"].ToString()).ToArray());
            return orgIDs;
        }
        #endregion

        #region 私有帮助方法
        private static UserInfo ParseDataRow(DataRow row)
        {
            UserInfo user = new UserInfo();

            user.UserID = row["ID"].ToString();
            user.Code = row["Code"].ToString();
            user.WorkNo = row["WorkNo"].ToString();
            user.Sex = DBNull.Value.Equals(row["Sex"]) ? "" : row["Sex"].ToString();
            user.UserName = row["Name"].ToString();
            user.UserEmail = row["Email"].ToString();
            user.UserPhone = row["Phone"].ToString();
            user.MobilePhone = row["MobilePhone"].ToString();
            user.Address = DBNull.Value.Equals(row["Address"]) ? "" : row["Address"].ToString();
            user.UserOrgID = row["DeptID"].ToString();
            user.UserFullOrgID = DBNull.Value.Equals(row["DeptFullID"]) ? "" : row["DeptFullID"].ToString();
            user.UserOrgName = row["DeptName"].ToString();
            user.UserPrjID = row["PrjID"].ToString();
            user.UserPrjName = row["PrjName"].ToString();
            user.SortIndex = row["SortIndex"] is DBNull ? 0.0 : Convert.ToDouble(row["SortIndex"]);
            //user.MpID = row["MpID"].ToString();
            //user.AppID = row["AppID"].ToString();
            
            
            //user.UserFullOrgIDs = GetOrgsForUser(row["ID"].ToString(), string.Empty);
            //代码结构调整，并增加UserOrgIDs 2014-12-3
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select S_A_Org.* from S_A__OrgUser join S_A_Org on OrgID=ID and UserID='{0}'";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, user.UserID));
            user.UserFullOrgIDs = string.Join(",", dt.AsEnumerable().Select(c => c["FullID"].ToString()).ToArray());
            user.UserOrgIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());

            //获取用户微信账号信息
            SQLHelper mpsqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WeChat);
            string mpsql = @"select a.MpID,b.AppID from MpAccountUserRelation a inner join MpAccount b on a.MpID=b.ID where UserID = '{0}' and IsDefault='T'";
            DataTable mpdt = mpsqlHelper.ExecuteDataTable(string.Format(mpsql, user.UserID));
            user.MpID = mpdt.AsEnumerable().Select(c => c["MpID"].ToString()).FirstOrDefault();
            user.AppID = mpdt.AsEnumerable().Select(c => c["AppID"].ToString()).FirstOrDefault();
            return user;
        }

        #endregion

    }
}
