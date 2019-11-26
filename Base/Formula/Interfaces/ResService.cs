using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Formula.Helper;
using System.Data;

namespace Formula
{
    public class ResService : IResService
    {
        public List<Res> GetRes(string url, string type)
        {
            string key = "GetRes_" + string.Format("{0}_{1}", url, type).GetHashCode().ToString();
            return (List<Res>)CacheHelper.Get(key, () =>
            {
                return Config.Logic.ResService.GetRes(url, type);
            });
        }

        public List<Res> GetRes(string url, string type, string userID)
        {
            string key = string.Format("{0}_GetRes_{1}_{2}", userID, type, url.GetHashCode());

            return (List<Res>)CacheHelper.Get(key, () =>
            {
                var user = FormulaHelper.GetUserInfoByID(userID);

                return Config.Logic.ResService.GetRes(url, type, userID, user.UserOrgID, user.UserPrjID);
            });
        }

        public DataTable GetResTree(string resRootID, string userID)
        {
            string key = string.Format("{0}_GetResTree_{1}", userID, resRootID);

            return (DataTable)CacheHelper.Get(key, () =>
            {
                var user = FormulaHelper.GetUserInfoByID(userID);

                return Config.Logic.ResService.GetResTree(resRootID, userID, user.UserOrgID, user.UserPrjID);
            });
        }
    }
}
