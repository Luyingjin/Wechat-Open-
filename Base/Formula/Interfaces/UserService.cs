using System.Collections.Generic;
using Config;
using Formula.Helper;
using System.Web;

namespace Formula
{
    /// <summary>
    /// 用户服务的默认实现，具体实现在config中
    /// </summary>
    public class UserService : IUserService
    {
        public string GetCurrentUserLoginName()
        {     
            //如果上下文中有代理用户，则取代理用户
            if (FormulaHelper.ContextContainsKey("AgentUserLoginName"))
                return FormulaHelper.ContextGetValueString("AgentUserLoginName");

            return Config.Logic.UserService.GetCurrentUserLoginName();
        }

        public UserInfo GetUserInfoBySysName(string systemName)
        {
            return (UserInfo)CacheHelper.Get("GetUserInfoBySysName_" + systemName, () =>
            {
                return Config.Logic.UserService.GetUserInfoBySysName(systemName);
            });
        }

        public UserInfo GetUserInfoByID(string userID)
        {
            return (UserInfo)CacheHelper.Get("GetUserInfoByID" + userID, () =>
            {
                return Config.Logic.UserService.GetUserInfoByID(userID);
            });
        }

        public string GetRolesForUser(string userID, string orgID)
        {
            return (string)CacheHelper.Get("GetRolesForUser_" + userID, () =>
            {
                return Config.Logic.UserService.GetRolesForUser(userID, orgID);
            });
        }

        public IList<UserInfo> GetAllUsers()
        {
            return (IList<UserInfo>)CacheHelper.Get("GetAllUsers", () =>
            {
                return Config.Logic.UserService.GetAllUsers();
            });
        }

        public byte[] GetSignImg(string userID)
        {
            return (byte[])CacheHelper.Get("GetSignImg_" + userID, () =>
            {
                return Config.Logic.UserService.GetSignImg(userID);
            });
        }

        public byte[] GetUserImg(string userID)
        {
            return (byte[])CacheHelper.Get("GetUserImg_" + userID, () =>
            {
                return Config.Logic.UserService.GetUserImg(userID);
            });
        }

        public string GetUserNames(string userIDs)
        {
            return Config.Logic.UserService.GetUserNames(userIDs);
        }

        public bool InRole(string userID, string roleIDs, string orgIDs)
        {
            return Config.Logic.UserService.InRole(userID, roleIDs, orgIDs);
        }

        public bool InOrg(string userID, string orgIDs)
        {
            return Config.Logic.UserService.InOrg(userID, orgIDs);
        }

        public string GetUserOrgsByRoleID(string userID, string orgIDs)
        {
            return Config.Logic.UserService.GetUserOrgsByRoleID(userID, orgIDs);
        }

        public string GetRoleCodesForUser(string userID, string orgID)
        {
            return (string)CacheHelper.Get("GetRoleCodesForUser_" + userID + "_" + orgID, () =>
            {
                return Config.Logic.UserService.GetRoleCodesForUser(userID, orgID);
            });
        }
    }
}
