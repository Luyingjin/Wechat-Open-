using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Config.Logic
{
    public class OrgService
    {
        #region 根据组织取所有用户的完整信息

        public static IList<UserInfo> GetOrgUsers(string orgId)
        {
            var users = new List<UserInfo>();
            if (!string.IsNullOrWhiteSpace(orgId))
            {
                var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var sql = string.Format("select S_A_User.* from S_A_User join S_A__OrgUser on ID=UserID and OrgID='{0}' where S_A_User.IsDeleted = '0' ", orgId);
                var dt = sqlHelper.ExecuteDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    var user = new UserInfo();
                    user.UserID = row["ID"].ToString();
                    user.Code = row["Code"].ToString();
                    user.WorkNo = row["WorkNo"].ToString();
                    user.UserName = row["Name"].ToString();
                    user.UserEmail = row["Email"].ToString();
                    user.UserPhone = row["Phone"].ToString();
                    user.MobilePhone = row["MobilePhone"].ToString();
                    user.UserOrgID = row["DeptID"].ToString();
                    user.UserOrgName = row["DeptName"].ToString();
                    user.UserPrjID = row["PrjID"].ToString();
                    user.UserPrjName = row["PrjName"].ToString();

                    users.Add(user);
                }
            }
            return users;
        }

        #endregion

        #region 获取指定组织id的所有直属上级节点及所有下级节点

        public static IList<Org> GetOrgs(string orgId)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format("select * from S_A_Org where FullID like '%{0}%' and IsDeleted = '0'", orgId);
            var dt = sqlHelper.ExecuteDataTable(sql);

            var orgs = new List<Org>();
            foreach (DataRow row in dt.Rows)
            {
                var org = new Org();

                org.ID = row["ID"].ToString();
                org.Code = row["Code"].ToString();
                org.Name = row["Name"].ToString();
                org.ParentID = row["ParentID"].ToString();
                org.FullID = row["FullID"].ToString();
                org.SortIndex = row["SortIndex"] is DBNull ? 0 : float.Parse(row["SortIndex"].ToString());

                orgs.Add(org);
            }
            return orgs;
        }

        #endregion

        #region 获取组织下的用户ID字符串

        public static string GetUserIDsInOrgs(string orgIDs)
        {
            if (orgIDs == null)
                orgIDs = "";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string userIDs = "";
            foreach (string orgID in orgIDs.Split(','))
            {
                if (orgID == "")
                    continue;
                object fullID = sqlHelper.ExecuteScalar(string.Format("select FullID from S_A_Org where ID='{0}'", orgID));

                string sql = string.Format(@"select UserID as ID from S_A_Org join S_A__OrgUser on FullID like '{0}%' and ID=OrgID "
                    , fullID);

                DataTable dt = sqlHelper.ExecuteDataTable(sql);

                userIDs += "," + string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
            }
            //消除重复
            userIDs = string.Join(",", userIDs.Split(',').Distinct()).Trim(',');
            return userIDs;
        }

        #endregion

        public static IList<Org> GetChildOrgs(string orgId, OrgType type)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format("select * from S_A_Org where ParentID = '{0}' and Type='{1}' and IsDeleted = '0'", orgId, type.ToString());
            var dt = sqlHelper.ExecuteDataTable(sql);

            var orgs = new List<Org>();
            foreach (DataRow row in dt.Rows)
            {
                var org = new Org();

                org.ID = row["ID"].ToString();
                org.Code = row["Code"].ToString();
                org.Name = row["Name"].ToString();
                org.ParentID = row["ParentID"].ToString();
                org.FullID = row["FullID"].ToString();
                org.SortIndex = row["SortIndex"] is DBNull?0:float.Parse(row["SortIndex"].ToString());

                orgs.Add(org);
            }
            return orgs;
        }
    }



}
