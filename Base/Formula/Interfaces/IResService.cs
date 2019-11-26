using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Data;

namespace Formula
{
    /// <summary>
    /// 权限的服务接口
    /// </summary>
    public interface IResService : ISingleton
    {
        /// <summary>
        /// 获取指定Url的所有数据权限
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        List<Res> GetRes(string url, string type);

        /// <summary>
        /// 获取指定Url的所有该用户拥有的数据权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="url">地址</param>
        /// <returns></returns>
        List<Res> GetRes(string url, string type, string userID);

        /// <summary>
        /// 获取用户有权限的ResTree
        /// </summary>
        /// <param name="resRootID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        DataTable GetResTree(string resRootID, string userID);

    }


}
