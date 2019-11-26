using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Formula.DynConditionObject;
using System.Reflection;
using System.Data.Entity;
using MvcAdapter;

namespace System.Linq
{
    public static class QueryableExtend
    {   
        #region 按照QueryBuilder查询

        /// <summary>
        /// where qb,结果直接转化为GridData
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="qb"></param>
        /// <returns></returns>
        public static GridData WhereToGridData<TEntity>(this IQueryable<TEntity> source, QueryBuilder qb)
        {
            var list = source.Where(qb);
            GridData gridData = new GridData(list);
            gridData.total = qb.TotolCount;
            return gridData;
        }

        #endregion
    }
}
