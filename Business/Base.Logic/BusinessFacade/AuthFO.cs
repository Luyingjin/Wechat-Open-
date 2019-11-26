using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base.Logic.Domain;
using Formula;
using Formula.Exceptions;
using Config;

namespace Base.Logic.BusinessFacade
{
    public class AuthFO
    {
        BaseEntities entities = FormulaHelper.GetEntities<BaseEntities>();

        #region 获取用户所属部门

        public string GetUserDeptNames(string userID)
        {
            var arr = entities.Set<S_A__OrgUser>().Where(c => c.UserID == userID && c.S_A_Org.Type.EndsWith("Dept") && c.S_A_Org.IsDeleted != "1").Select(c => c.S_A_Org.Name).ToArray();
            return string.Join(",", arr);
        }

        #endregion

        #region DeleteOrg

        public void DeleteOrg(string orgFullID)
        {
            if (string.IsNullOrEmpty(orgFullID))
                return;
            if (!orgFullID.Contains("."))
                throw new BusinessException("不能作废整个组织");
            var orgs = entities.S_A_Org.Where(c => c.FullID.StartsWith(orgFullID)).ToArray();

            foreach (var org in orgs)
            {
                foreach (var item in entities.Set<S_A_User>().Where(c => c.DeptID == org.ID))
                {
                    item.DeptID = "";
                    item.DeptName = "";
                    item.DeptFullID = "";
                }

                entities.Set<S_A_Org>().Remove(org);
            }
            entities.SaveChanges();
        }


        #endregion

        #region AbortOrg

        /// <summary>
        /// 作废部门
        /// </summary>
        /// <param name="orgFullID"></param>
        public void AbortOrg(string orgFullID)
        {
            if (string.IsNullOrEmpty(orgFullID))
                return;
            if (!orgFullID.Contains("."))
                throw new BusinessException("不能作废整个组织");
            var orgs = entities.S_A_Org.Where(c => c.FullID.StartsWith(orgFullID)).ToArray();

            foreach (var org in orgs)
            {
                org.IsDeleted = "1";
                org.DeleteTime = DateTime.Now;
                //清空权限
                entities.S_A__OrgRes.Delete(c => c.OrgID == org.ID);
                //清空角色
                entities.S_A__OrgRole.Delete(c => c.OrgID == org.ID);

                foreach (var item in entities.Set<S_A_User>().Where(c => c.DeptID == org.ID))
                {
                    item.DeptID = "";
                    item.DeptName = "";
                    item.DeptFullID = "";
                }
            }

            entities.SaveChanges();
        }

        #endregion

        #region 恢复部门
        public void RecoverOrg(string orgNodeID)
        {
            if (string.IsNullOrEmpty(orgNodeID))
                return;
            var org = entities.S_A_Org.Where(c => c.ID.Equals(orgNodeID)).FirstOrDefault();
            org.IsDeleted = "0";
            org.DeleteTime = null;
            entities.SaveChanges();
        }

        #endregion

        #region RetireUser

        /// <summary>
        /// 人员离退
        /// </summary>
        /// <param name="userID"></param>
        public void RetireUser(string userID)
        {
            var user = entities.S_A_User.Where(c => c.ID == userID).SingleOrDefault();
            user.IsDeleted = "1";
            user.DeleteTime = DateTime.Now;
            //user.DeptID = "";
            //user.DeptName = "";
            //user.DeptFullID = "";
            //user.PrjID = "";
            //user.PrjName = "";

            user.S_A__OrgUser.Clear();
            user.S_A__RoleUser.Clear();
            entities.SaveChanges();
        }

        #endregion

        public List<S_A_Res> GetResByUserID(string userID)
        {
            string sql = string.Format(@"
--组织权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRes on UserID='{0}' and S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
union
--系统角色权限
select S_A_Res.* from S_A__RoleUser 
join S_A__RoleRes on UserID='{0}' and S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
union
--组织角色权限
select S_A_Res.* from S_A__OrgUser 
join S_A__OrgRole on UserID='{0}' and S_A__OrgUser.OrgID=S_A__OrgRole.OrgID 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID", userID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return sqlHelper.ExecuteList<S_A_Res>(sql);
        }

        #region 安全审计日志

        public static void Log(string operation, string operationTarget, string relateData)
        {
            S_A_AuthLog log = new S_A_AuthLog();
            log.ID = FormulaHelper.CreateGuid();
            log.Operation = operation;
            log.OperationTarget = operationTarget;
            log.RelateData = relateData;
            log.ModifyUserName = FormulaHelper.GetUserInfo().UserName;
            log.ModifyTime = DateTime.Now;
            log.ClientIP = GetUserIP();

            var entities = FormulaHelper.GetEntities<BaseEntities>();
            entities.Set<S_A_AuthLog>().Add(log);
        }

        private static string GetUserIP()
        {
            string result = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result != System.Web.HttpContext.Current.Request.UserHostAddress)
                result += "," + System.Web.HttpContext.Current.Request.UserHostAddress;

            return result;
        }

        #endregion
    }
}
