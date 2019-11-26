using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Formula;
using Config;
using System.Data;

namespace Base.Areas.PortalBlock.Controllers
{
    public class FriendLinkController : BaseController<S_I_FriendLink>
    {
        //
        // GET: /PortalBlock/FriendLink/
        public ActionResult ListView()
        {
            return View();
        }

        public JsonResult GetListView(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            UserInfo user = FormulaHelper.GetUserInfo();
            string[] arrOrgFullID = user.UserFullOrgID.Split('.');
            string whereSql = string.Empty;
            if (Config.Constant.IsOracleDb)
            {
                string whereReceiveDeptId = string.Empty;
                for (int i = 0; i < arrOrgFullID.Length; i++)
                {
                    whereReceiveDeptId += "INSTR(DeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                    if (i < arrOrgFullID.Length - 1)
                        whereReceiveDeptId += " or ";
                }
                whereSql = " and ((nvl(UserId,'empty') = 'empty' and nvl(DeptId,'empty') = 'empty') or (nvl(UserId,'empty') <> 'empty' and INSTR(DeptId,'" + user.UserID + "',1,1) > 0) or (nvl(DeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))) ";
            }
            else
            {
                string whereReceiveDeptId = string.Empty;
                for (int i = 0; i < arrOrgFullID.Length; i++)
                {
                    whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',DeptId) > 0";
                    if (i < arrOrgFullID.Length - 1)
                        whereReceiveDeptId += " or ";
                }
                whereSql = " and ((isnull(UserId,'') = '' and isnull(DeptId,'') = '') or (isnull(UserId,'') <> '' and charindex('" + user.UserID + "',UserId) > 0) or (isnull(DeptId,'') <> '' and (" + whereReceiveDeptId + "))) ";
            }
            GridData data = sqlHelper.ExecuteGridData("select * from S_I_FriendLink where 1=1 " + whereSql, qb);
            return Json(data);
        }
    }
}
