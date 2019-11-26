using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Formula;

namespace MvcAdapter
{
    [ModelBinder(typeof(QueryBuilderBinder))]
    public class QueryBuilder : BaseQueryBuilder
    {
      
    }

  
}
