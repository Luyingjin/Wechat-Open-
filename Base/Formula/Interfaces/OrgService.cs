using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula.Helper;

namespace Formula
{
    public class OrgService : IOrgService
    {
        public IList<UserInfo> GetOrgUsers(string orgId)
        {
            return (IList<UserInfo>)CacheHelper.Get("GetOrgUsers_" + orgId, () =>
            { 
                return Config.Logic.OrgService.GetOrgUsers(orgId);
            });
        }

        public IList<Org> GetOrgs(string orgId = "")
        {
            return (IList<Org>)CacheHelper.Get("GetOrgs_" + orgId, () =>
            { 
                return Config.Logic.OrgService.GetOrgs(orgId); 
            });
        }

        public string GetUserIDsInOrgs(string orgIDs)
        {
            return (string)CacheHelper.Get("GetUserIDsInOrgs_" + orgIDs, () => 
            { 
                return Config.Logic.OrgService.GetUserIDsInOrgs(orgIDs);
            });
        }


        public IList<Org> GetChildOrgs(string orgId, OrgType type)
        {
            return (IList<Org>)CacheHelper.Get("GetChildOrgs_" + orgId + "_" + type.ToString(), () =>
            {
                return Config.Logic.OrgService.GetChildOrgs(orgId, type);
            });
        }
    }
}
