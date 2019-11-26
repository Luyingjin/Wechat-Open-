using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    /// <summary>
    /// 用户的服务接口
    /// </summary>
    public interface IUserService : ISingleton
    {
        /// <summary>
        /// 获取当前用户登录名
        /// </summary>
        /// <returns></returns>
        string GetCurrentUserLoginName();

        /// <summary>
        /// 根据用户名取用户信息
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        UserInfo GetUserInfoBySysName(string systemName);

        /// <summary>
        /// 根据用户id取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UserInfo GetUserInfoByID(string userID);


        /// <summary>
        /// 获取用户的系统角色+指定组织的组织角色id字符串
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="orgID"></param>
        /// <returns></returns>
        string GetRolesForUser(string userID, string orgID);

        /// <summary>
        /// 获取用户的系统角色+指定组织的组织角色Code字符串
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="orgID"></param>
        /// <returns></returns>
        string GetRoleCodesForUser(string userID, string orgID);

        /// <summary>
        /// 获取指定用户Id的签名图片
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        byte[] GetSignImg(string userId);

        /// <summary>
        /// 获取指定用户Id的头像
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        byte[] GetUserImg(string userId);

        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        IList<UserInfo> GetAllUsers();

        /// <summary>
        /// 根据用户id取用户名称
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        string GetUserNames(string userIDs);

        /// <summary>
        /// 用户是否在角色及组织角色中
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleIDs"></param>
        /// <param name="orgIDs"></param>
        /// <returns></returns>
        bool InRole(string userID, string roleIDs, string orgIDs);

        /// <summary>
        /// 用户是否在组织中
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="orgIDs"></param>
        /// <returns></returns>
        bool InOrg(string userID, string orgIDs);

        /// <summary>
        /// 获取用户指定角色的所有组织ID，多个用逗号分隔.
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        string GetUserOrgsByRoleID(string userID, string roleID);
    }

}
