using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Base.Logic;
using Config;

namespace Base.Areas.Auth.Controllers
{
    public class AuthInfoController : BaseController<S_A_AuthInfo>
    {
        public override JsonResult Save()
        {
            var authInfo = UpdateEntity<S_A_AuthInfo>();
            if (string.IsNullOrEmpty(authInfo.OrgRootFullID))
            {
                var org = UpdateNode<S_A_Org>(authInfo.ID);
                org.FullID = authInfo.ID;
                org.Type = OrgType.Org.ToString();
                authInfo.OrgRootFullID = authInfo.ID;
            }
           
            if (string.IsNullOrEmpty(authInfo.ResRootFullID))
            {
                var res = UpdateNode<S_A_Res>(authInfo.ID);
                res.FullID = authInfo.ID;
                res.Type = ResType.Menu.ToString();
                authInfo.ResRootFullID = authInfo.ID;
            }
            if (string.IsNullOrEmpty(authInfo.RoleGroupID))
            {               
                authInfo.RoleGroupID = authInfo.ID;
            }
            if (string.IsNullOrEmpty(authInfo.UserGroupID))
                authInfo.UserGroupID = authInfo.ID;

            return base.JsonSave<S_A_AuthInfo>(authInfo);
        }
    }
}
