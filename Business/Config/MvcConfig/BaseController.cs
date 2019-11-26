using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcConfig
{
    public class BaseController : MvcAdapter.BaseController
    {

        protected override System.Data.Entity.DbContext entities
        {
            get { throw new NotImplementedException(); }
        }
    }
}
