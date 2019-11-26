using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula.Helper;

namespace Formula
{
    public class RoleService : IRoleService
    {
        public IList<Role> GetSysRoles()
        {
            return (IList<Role>)CacheHelper.Get("GetSysRoles", () =>
            { 
                return Config.Logic.RoleService.GetRoles(RoleType.SysRole);
            });
        }

        public string GetUserIDsInRoles(string roleIDs, string orgIDs)
        {
            string key = "GetUserIDsInRoles_" + string.Format("{0}_{1}", roleIDs, orgIDs).GetHashCode().ToString();
            return (string)CacheHelper.Get(key, () =>
            { 
                return Config.Logic.RoleService.GetUserIDsInRoles(roleIDs, orgIDs); 
            });
        }


    }
}
