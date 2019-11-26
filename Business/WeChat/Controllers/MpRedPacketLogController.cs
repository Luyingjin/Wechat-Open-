using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;

namespace WeChat.Controllers
{
    public class MpRedPacketLogController : BaseController<MpRedPacketLog>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var RPID = GetQueryString("RPID");
            var result = entities.Set<MpRedPacketLog>().Where(c => c.RPID == RPID).WhereToGridData(qb);
            return Json(result);
        }
    }
}
