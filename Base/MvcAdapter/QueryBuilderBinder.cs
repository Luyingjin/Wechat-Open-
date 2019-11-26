using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Formula;
using Formula.Helper;

namespace MvcAdapter
{
    public class QueryBuilderBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = (QueryBuilder)(bindingContext.Model ?? new QueryBuilder());
            var dict = controllerContext.HttpContext.Request.Params;

            return QueryBuilderHelper.BindModel(model, dict);
        }

      
    }
}
