using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;

namespace Base.Areas.GenerateCode.Controllers
{
    public class RuleCodeController : BaseController<S_RC_RuleCode>
    {
        public JsonResult GetRuleList(QueryBuilder qb)
        {
            return base.JsonGetList<S_RC_RuleCode>(qb);
        }

    }
}
