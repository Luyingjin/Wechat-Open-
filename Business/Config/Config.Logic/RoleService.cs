using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Config.Logic
{
    public class RoleService
    {
        public static IList<Role> GetRoles(RoleType type)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format("select * from dbo.S_A_Role where Type='{0}'", type.ToString());

            var roles = new List<Role>();
            var dt = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                var role = new Role();

                role.ID = row["ID"].ToString();
                role.Code = row["Code"].ToString();
                role.Name = row["Name"].ToString();
                role.Type = row["Type"].ToString();

                roles.Add(role);
            }
            return roles;
        }

        public static string GetUserIDsInRoles(string roleIDs, string orgIDs)
        {
            if (orgIDs == null)
                orgIDs = "";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            //获取系统角色下的人
            string sql = string.Format(@"select S_A_User.* from S_A_User join S_A__RoleUser on S_A_User.ID=UserID and RoleID in('{0}') join S_A_Role on RoleID=S_A_Role.ID and S_A_Role.Type='SysRole'"
                  , roleIDs.Replace(",", "','"));
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            string userIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());

            //获取继承的组织角色下的人
            foreach (string orgID in orgIDs.Split(','))
            {
                if (orgID == "")
                    continue;
                object fullID = sqlHelper.ExecuteScalar(string.Format("select FullID from S_A_Org where ID='{0}'", orgID));
                if (fullID == null)
                    continue;
                sql = string.Format(@"select S_A_User.ID from S_A_Org join S_A__OrgRole on FullID like '{0}%' and ID=OrgID  and roleID in('{1}') 
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A_Org.ID join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
union
select S_A_User.ID from S_A_Org join S_A__OrgRoleUser on FullID like '{0}' +'%' and ID=OrgID  and roleID in('{1}') 
 join S_A_User on S_A_User.ID=UserID
", fullID, roleIDs.Replace(",", "','"));

                dt = sqlHelper.ExecuteDataTable(sql);

                //如果当前部门找不到组织角色用户，则查找上级部门
                if (dt.Rows.Count == 0 && orgIDs.Contains(',') == false && fullID.ToString().Contains('.')) //组织角色下没有用户，并且只提供一个组织时，递归找上一级组织角色下的人员
                {
                    dt = GetUpperOrgRole(fullID.ToString(), roleIDs);
                }
              

                userIDs += "," + string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
            }

            //消除重复
            userIDs = string.Join(",", userIDs.Split(',').Distinct()).Trim(',');

            return userIDs;
        }

        /// <summary>
        /// 查找上级部门的组织角色
        /// </summary>
        /// <param name="fullID"></param>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        private static DataTable GetUpperOrgRole(string fullID, string roleIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "";
            if (fullID.Contains('.') == false)
            {
                return sqlHelper.ExecuteDataTable("select * from S_A_User where 1=2");
            }            

            string upperDeptFullID = fullID.Remove(fullID.LastIndexOf('.'));//上级部门ID

            sql = string.Format(@"select S_A_User.ID from S_A_Org join S_A__OrgRole on FullID like '{2}%' and Type='Post' and len(FullID)<{3} and ID=OrgID  and roleID in('{1}') 
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A_Org.ID join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
union
select S_A_User.ID from S_A_Org join S_A__OrgRoleUser on OrgID in ('{0}') and ID=OrgID  and roleID in('{1}') 
 join S_A_User on S_A_User.ID=UserID
", upperDeptFullID.ToString().Split('.').Last()
, roleIDs.Replace(",", "','")
, upperDeptFullID
, upperDeptFullID.ToString().Length + 38   //只向下查找一级岗位
);

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            if (dt.Rows.Count == 0)
                return GetUpperOrgRole(upperDeptFullID, roleIDs); //递归
            else
                return dt;
        }
    }
}
