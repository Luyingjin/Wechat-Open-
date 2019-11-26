using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Formula.DynConditionObject;
using Formula;
using Formula.Helper;
using System.Data.Entity;
using System.Web;

namespace MvcAdapter
{
    public abstract class BaseApiController<T> : ApiController where T : class, new()
    {
        #region entities

        /// <summary>
        /// 实体对象库
        /// </summary>
        protected abstract DbContext entities
        {
            get;
        }

        #endregion

        // GET api/<controller>
        public virtual IEnumerable<T> Get()
        {
            var qb = QueryBuilderHelper.BindModel(new QueryBuilder(), Request.RequestUri.ParseQueryString());
            var result = entities.Set<T>().Where(qb).ToArray();
            return result;
        }

        // GET api/<controller>/5
        public virtual T Get(string id)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            return obj;
        }
        // POST api/<controller>        
        public virtual void Post([FromBody]T obj)
        {
            entities.Set<T>().Add(obj);
            entities.SaveChanges();
        }

        // PUT api/<controller>/5        
        public virtual void Put(string id, [FromBody]T src)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T dest = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            FormulaHelper.UpdateModel(dest, src);
            entities.SaveChanges();
        }

        // DELETE api/<controller>/5
        public virtual void Delete(int id)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            entities.Set<T>().Remove(obj);
            entities.SaveChanges();
        }

    }

    public abstract class BaseApiController<T, DTO> : ApiController
        where T : class, new()
        where DTO : class,new()
    {
        #region entities

        /// <summary>
        /// 实体对象库
        /// </summary>
        protected abstract DbContext entities
        {
            get;
        }

        #endregion

        // GET api/<controller>
        public virtual IEnumerable<T> Get()
        {
            var qb = QueryBuilderHelper.BindModel(new QueryBuilder(), Request.RequestUri.ParseQueryString());
            var result = entities.Set<T>().Where(qb).ToArray();
            return result;
        }

        // GET api/<controller>/5
        public virtual T Get(string id)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            return obj;
        }
        // POST api/<controller>        
        public virtual void Post([FromBody]T obj)
        {
            entities.Set<T>().Add(obj);
            entities.SaveChanges();
        }

        // PUT api/<controller>/5        
        public virtual void Put(string id, [FromBody]T src)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T dest = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            FormulaHelper.UpdateModel(dest, src);
            entities.SaveChanges();
        }

        // DELETE api/<controller>/5
        public virtual void Delete(int id)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            entities.Set<T>().Remove(obj);
            entities.SaveChanges();
        }

    }

}