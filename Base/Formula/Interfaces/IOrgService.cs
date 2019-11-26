using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.Logic;

namespace Formula
{
    /// <summary>
    /// 组织结构的服务接口
    /// </summary>
    public interface IOrgService : ISingleton
    {
        /// <summary>
        /// 获取指定OrgId的用户（直属，不包含子节点的用户）
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        IList<UserInfo> GetOrgUsers(string orgId);

        /// <summary>
        /// 获取指定OrgId的组织机构树（包含本节点及所有子节点）
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns></returns>
        IList<Org> GetOrgs(string orgId = "");

        /// <summary>
        /// 获取指定组织下的用户id字符串
        /// </summary>
        /// <param name="orgIDs"></param>
        /// <returns></returns>
        string GetUserIDsInOrgs(string orgIDs);
        
        IList<Org> GetChildOrgs(string orgId, OrgType type);

    }

}
