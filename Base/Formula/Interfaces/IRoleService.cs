using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;

namespace Formula
{
    /// <summary>
    /// 角色的服务接口
    /// </summary>
    public interface IRoleService : ISingleton
    {
        /// <summary>
        /// 获取系统角色
        /// </summary>
        /// <returns></returns>
        IList<Role> GetSysRoles();

        /// <summary>
        /// 获取指定角色下的用户id串，注意：orgIDs为空找的就是系统角色
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <param name="orgIDs"></param>
        /// <returns></returns>
        string GetUserIDsInRoles(string roleIDs, string orgIDs);
    }


}
