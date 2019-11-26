using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;

namespace WeChat.Controllers
{
    public class MpMediaController : BaseController
    {
        //public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        //{
        //    var mediatype = GetQueryString("mediatype");
        //    var userinfo = Formula.FormulaHelper.GetUserInfo();
        //    var result = entities.Set<MpMedia>().Where(c => c.MediaType == mediatype && c.MpID == userinfo.MpID && c.IsDelete == 0).WhereToGridData(qb);
        //    return Json(result);
        //}
    }
}
