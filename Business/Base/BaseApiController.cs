using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Base.Logic.Domain;
using Formula;

namespace Base.Controllers
{
    public class BaseApiController<T> : MvcAdapter.BaseApiController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<BaseEntities>();
                }
                return _entities;
            }
        }
    }
}